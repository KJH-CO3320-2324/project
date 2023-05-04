using NoteMapper.Core;
using NoteMapper.Data.Core.Questionnaires;
using NoteMapper.Services.Emails;
using NoteMapper.Services.Web.ViewModels.Questionnaires;

namespace NoteMapper.Services.Web.Questionnaires
{
    public class QuestionnaireViewModelService : IQuestionnaireViewModelService
    {
        private readonly IEmailSenderService _emailSenderService;
        private readonly IQuestionnaireRepository _questionnaireRepository;
        private readonly IQuestionnaireQuestionRepository _questionRepository;
        private readonly IUserQuestionResponseRepository _responseRepository;
        private readonly QuestionnaireViewModelServiceSettings _settings;

        public QuestionnaireViewModelService(IQuestionnaireRepository questionnaireRepository,
            IQuestionnaireQuestionRepository questionRepository,
            IUserQuestionResponseRepository responseRepository,
            QuestionnaireViewModelServiceSettings settings, IEmailSenderService emailSenderService)
        {
            _emailSenderService = emailSenderService;
            _questionnaireRepository = questionnaireRepository;
            _questionRepository = questionRepository;
            _responseRepository = responseRepository;
            _settings = settings;
        }

        public async Task<ServiceResult> CreateQuestionnaireAsync(EditQuestionnaireViewModel viewModel)
        {
            Questionnaire? questionnaire = MapEditViewModelToQuestionnaire(Guid.Empty, viewModel);

            questionnaire = await _questionnaireRepository.CreateAsync(questionnaire);

            if (questionnaire == null)
            {
                return ServiceResult.Failure("Error creating questionnaire");
            }

            IReadOnlyCollection<QuestionnaireQuestion> questions = viewModel.Questions
                .Select((x, i) => MapEditViewModelToQuestion(Guid.Empty, questionnaire.QuestionnaireId, x, i))
                .ToArray();

            ServiceResult result = await _questionRepository.UpdateQuestionsAsync(questions, 
                Array.Empty<QuestionnaireQuestion>(), 
                Array.Empty<QuestionnaireQuestion>());

            return result.Success
                ? ServiceResult.Successful("Questionnaire created")
                : ServiceResult.Failure("Error creating questions");
        }

        public async Task<EditQuestionnaireViewModel?> GetEditQuestionnaireViewModel(Guid questionnaireId)
        {
            Questionnaire? questionnaire = await _questionnaireRepository.FindAsync(questionnaireId);
            if (questionnaire == null)
            {
                return null;
            }

            IReadOnlyCollection<QuestionnaireQuestion> questions = await _questionRepository.GetQuestionsAsync(questionnaireId);

            return new EditQuestionnaireViewModel(questionnaire, questions);
        }

        public async Task<QuestionnaireResponsesViewModel?> GetLatestQuestionnaireResponsesAsync(Guid userId)
        {
            Questionnaire? questionnaire = await _questionnaireRepository.GetCurrentAsync();
            if (questionnaire == null)
            {
                return null;
            }

            IReadOnlyCollection<QuestionnaireQuestion> questions = await _questionRepository.GetQuestionsAsync(
                questionnaire.QuestionnaireId);

            IReadOnlyCollection<UserQuestionResponse> responses = await _responseRepository.GetAsync(
                userId, questionnaire.QuestionnaireId);

            List<QuestionnaireResponseViewModel> responseViewModels = new();

            foreach (QuestionnaireQuestion question in questions)
            {
                UserQuestionResponse? response = responses.FirstOrDefault(x => x.QuestionId == question.QuestionId);
                responseViewModels.Add(new QuestionnaireResponseViewModel(question, response));
            }

            return new QuestionnaireResponsesViewModel(questionnaire,
                responseViewModels);
        }

        public async Task<IReadOnlyCollection<ListQuestionnaireViewModel>> GetQuestionnairesAsync()
        {
            IReadOnlyCollection<Questionnaire> questionnaires = await _questionnaireRepository.GetAllAsync();
            IDictionary<Guid, int> respondentCounts = await _questionnaireRepository.GetQuestionnaireRespondentCountsAsync();

            return questionnaires
                .Select(x => new ListQuestionnaireViewModel(x, respondentCounts[x.QuestionnaireId]))
                .ToArray();
        }

        public async Task<ServiceResult> SaveResponsesAsync(Guid userId,
            QuestionnaireResponsesViewModel viewModel)
        {
            Questionnaire? questionnaire = await _questionnaireRepository.FindAsync(viewModel.QuestionnaireId);
            if (questionnaire == null)
            {
                return ServiceResult.Failure("Error saving responses");
            }

            IReadOnlyCollection<QuestionnaireQuestion> questions = await _questionRepository.GetQuestionsAsync(
                questionnaire.QuestionnaireId);

            List<UserQuestionResponse> responses = new(await _responseRepository.GetAsync(
                userId, questionnaire.QuestionnaireId));

            foreach (QuestionnaireResponseViewModel responseViewModel in viewModel.Responses)
            {
                QuestionnaireQuestion? question = questions.FirstOrDefault(x => x.QuestionId == responseViewModel.QuestionId);
                if (question == null)
                {
                    continue;
                }

                UserQuestionResponse? response = responses.FirstOrDefault(x => x.QuestionId == question.QuestionId);
                if (response == null)
                {
                    response = new UserQuestionResponse(Guid.Empty, userId, question.QuestionId, responseViewModel.Value);
                    responses.Add(response);
                }
                else
                {
                    response.Value = responseViewModel.Value;
                }
            }

            ServiceResult result = await _responseRepository.SaveAsync(responses);

            if (result.Success)
            {
                await SendNotificationEmailAsync(userId, questions, responses);
            }

            return result.Success
                ? ServiceResult.Successful("Responses saved. Thank you")
                : ServiceResult.Failure("Error saving responses");
        }

        public async Task<ServiceResult> UpdateQuestionnaireAsync(Guid questionnaireId,
            EditQuestionnaireViewModel viewModel)
        {
            Questionnaire? existing = await _questionnaireRepository.FindAsync(questionnaireId);
            if (existing == null)
            {
                return ServiceResult.Failure("Not found");
            }

            IReadOnlyCollection<QuestionnaireQuestion> questions = await _questionRepository
                .GetQuestionsAsync(questionnaireId);

            IDictionary<Guid, QuestionnaireQuestion> questionDictionary = questions
                .ToDictionary(x => x.QuestionId);

            Questionnaire updateQuestionnaire = MapEditViewModelToQuestionnaire(questionnaireId, viewModel);

            List<QuestionnaireQuestion> insertQuestions = new();

            List<QuestionnaireQuestion> updateQuestions = new();

            for (int i = 0; i < viewModel.Questions.Count; i++)
            {
                EditQuestionViewModel questionViewModel = viewModel.Questions[i];
                if (questionViewModel.QuestionId == null || !questionDictionary.ContainsKey(questionViewModel.QuestionId.Value))
                {
                    QuestionnaireQuestion insertQuestion = MapEditViewModelToQuestion(Guid.Empty, questionnaireId, questionViewModel, i);
                    insertQuestions.Add(insertQuestion);
                }
                else
                {
                    QuestionnaireQuestion updateQuestion = MapEditViewModelToQuestion(questionViewModel.QuestionId.Value, questionnaireId, questionViewModel, i);
                    updateQuestions.Add(updateQuestion);
                }                
            }

            IReadOnlyCollection<QuestionnaireQuestion> deleteQuestions = questions
                .Where(x => !viewModel.Questions.Any(q => q.QuestionId == x.QuestionId))
                .ToArray();

            ServiceResult updateQuestionnaireResult = await _questionnaireRepository.UpdateAsync(updateQuestionnaire);
            if (!updateQuestionnaireResult.Success)
            {
                return ServiceResult.Failure("Error updating questionnaire");
            }

            ServiceResult updateQuestionsResult = await _questionRepository.UpdateQuestionsAsync(insertQuestions, updateQuestions, deleteQuestions);
            return updateQuestionsResult.Success
                ? ServiceResult.Successful("Questionnaire updated")
                : ServiceResult.Failure("Error updating questions");
        }

        private static QuestionnaireQuestion MapEditViewModelToQuestion(Guid id, Guid questionnaireId, EditQuestionViewModel viewModel, int index)
        {
            return new QuestionnaireQuestion(id, questionnaireId, viewModel.QuestionText,
                viewModel.Type, viewModel.Required, viewModel.MinValue, viewModel.MaxValue,
                index + 1);
        }

        private static Questionnaire MapEditViewModelToQuestionnaire(Guid id, EditQuestionnaireViewModel viewModel)
        {
            return new Questionnaire(id, 
                viewModel.Name, 
                viewModel.ExpiresUtc, 
                viewModel.Active, 
                viewModel.LinkText, 
                viewModel.IntroText);
        }

        private async Task SendNotificationEmailAsync(Guid userId, IReadOnlyCollection<QuestionnaireQuestion> questions,
            IReadOnlyCollection<UserQuestionResponse> responses)
        {
            string subject = $"{_settings.ApplicationName}: New survey response";
            string bodyPlain = $"User: {userId}" + Environment.NewLine;

            foreach (UserQuestionResponse response in responses)
            {
                QuestionnaireQuestion? question = questions.FirstOrDefault(x => x.QuestionId == response.QuestionId);
                if (question == null)
                {
                    continue;
                }

                bodyPlain += Environment.NewLine + question.QuestionText + Environment.NewLine + response.Value;
            }

            Email email = new(_settings.NotificationEmailAddress, subject, "", bodyPlain);
            await _emailSenderService.SendEmailAsync(email);
        }
    }
}
