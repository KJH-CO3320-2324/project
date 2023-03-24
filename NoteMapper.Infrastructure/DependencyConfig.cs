using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using NoteMapper.Core.Users;
using NoteMapper.Data.Core.Contact;
using NoteMapper.Data.Core.Errors;
using NoteMapper.Data.Core.Instruments;
using NoteMapper.Data.Core.Notifications;
using NoteMapper.Data.Core.Questionnaires;
using NoteMapper.Data.Core.Users;
using NoteMapper.Data.Cosmos;
using NoteMapper.Data.Cosmos.Repositories;
using NoteMapper.Data.Mongo;
using NoteMapper.Data.Mongo.Repositories;
using NoteMapper.Data.Sql.Repositories;
using NoteMapper.Data.Sql.Repositories.Contact;
using NoteMapper.Data.Sql.Repositories.Errors;
using NoteMapper.Data.Sql.Repositories.Notifications;
using NoteMapper.Data.Sql.Repositories.Questionnaires;
using NoteMapper.Data.Sql.Repositories.Users;
using NoteMapper.Emails;
using NoteMapper.Identity;
using NoteMapper.Identity.Microsoft;
using NoteMapper.Infrastructure.Extensions;
using NoteMapper.Services;
using NoteMapper.Services.Emails;
using NoteMapper.Services.Feedback;
using NoteMapper.Services.Instruments;
using NoteMapper.Services.Logging;
using NoteMapper.Services.Notifications;
using NoteMapper.Services.Questionnaires;
using NoteMapper.Services.Users;
using NoteMapper.Services.Web.Caching;
using NoteMapper.Services.Web.Contact;
using NoteMapper.Services.Web.Instruments;
using NoteMapper.Services.Web.NoteMap;
using NoteMapper.Services.Web.Notifications;
using NoteMapper.Services.Web.Questionnaires;
using NoteMapper.Services.Web.Security;

namespace NoteMapper.Infrastructure
{
    public static class DependencyConfig
    {
        public static void RegisterDependencies(IDependencyContainer container, IConfiguration config)
        {
            RegisterData(container, config);
            RegisterIdentity(container, config);
            RegisterServices(container, config);
            RegisterWebServices(container, config);
        }

        private static void RegisterData(IDependencyContainer container, IConfiguration config)
        {
            string sqlConnectionStringName = config.GetValue("Data.Sql.ConnectionStringName");

            container
                .AddSingleton(new SqlRepositorySettings
                {
                    ConnectionString = config.GetConnectionString(sqlConnectionStringName) ?? "",
                    CurrentEnvironment = config.GetEnum<ApplicationEnvironment>("Environment")
                })
                .AddScoped<IApplicationErrorRepository, ApplicationErrorSqlRepository>()
                .AddScoped<IContactRepository, ContactSqlRepository>()
                .AddScoped<INotificationRepository, NotificationSqlRepository>()
                .AddScoped<IQuestionnaireQuestionRepository, QuestionnaireQuestionSqlRepository>()
                .AddScoped<IQuestionnaireRepository, QuestionnaireSqlRepository>()
                .AddScoped<IRegistrationCodeRepository, RegistrationCodeRepository>()
                .AddScoped<IUserActivationRepository, UserActivationSqlRepository>()
                .AddScoped<IUserLoginTokenRepository, UserLoginTokenSqlRepository>()
                .AddScoped<IUserNotificationRepository, UserNotificationSqlRepository>()
                .AddScoped<IUserPasswordRepository, UserPasswordSqlRepository>()
                .AddScoped<IUserPasswordResetCodeRepository, UserPasswordResetCodeSqlRepository>()
                .AddScoped<IUserPreferenceRepository, UserPreferenceSqlRepository>()
                .AddScoped<IUserQuestionResponseRepository, UserQuestionResponseSqlRepository>()
                .AddScoped<IUserRegistrationCodeRepository, UserRegistrationCodeSqlRepository>()
                .AddScoped<IUserRepository, UserSqlRepository>();

            string documentStorageProvider = config.GetValue("Data.DocumentStorage.Provider");
            string connectionStringName = config.GetValue("Data.DocumentStorage.ConnectionStringName");
            string connectionString = config.GetConnectionString(connectionStringName) ?? "";

            string applicationName = config.GetValue("Application.Name");
            ApplicationEnvironment environment = config.GetEnum<ApplicationEnvironment>("Environment");

            if (string.Equals(documentStorageProvider, "Mongo", StringComparison.InvariantCultureIgnoreCase))
            {                
                container
                    .AddSingleton(new MongoRepositorySettings
                    {
                        ApplicationName = applicationName,
                        ConnectionString = connectionString,
                        CurrentEnvironment = environment,
                        DatabaseId = config.GetValue("Data.Mongo.DatabaseId"),
                        DefaultUserId = config.GetValue("Data.Mongo.DefaultUserId")
                    })
                    .AddScoped<IUserInstrumentRepository, UserInstrumentMongoRepository>();                
            }            
            else
            {
                // default to Azure Cosmos
                container
                    .AddSingleton(new AzureCosmosRepositorySettings
                    {
                        ApplicationName = applicationName,
                        ConnectionString = connectionString,
                        CurrentEnvironment = environment,
                        DatabaseId = config.GetValue("Data.Azure.Cosmos.DatabaseId"),
                        DefaultUserId = config.GetValue("Data.Azure.Cosmos.DefaultUserId")
                    })
                    .AddScoped<IUserInstrumentRepository, UserInstrumentAzureCosmosRepository>();
            }            
        }

        private static void RegisterIdentity(IDependencyContainer container, IConfiguration config)
        {
            string baseUrl = config.GetValue("BaseUrl");

            container
                .AddScoped<IIdentityService, MicrosoftIdentityService>()
                .AddSingleton(new MicrosoftIdentityServiceSettings
                {
                    ActivationCodeExpiresAfterMinutes = config.GetInt("Account.ActivationCodeExpiresAfterMinutes"),
                    ActivationUrl = baseUrl + config.GetValue("Account.ActivationUrl"),
                    ApplicationName = config.GetValue("Application.Name"),
                    LoginTokenExpiresAfterSeconds = config.GetInt("Account.LoginTokenExpiresAfterSeconds"),
                    PasswordResetCodeExpiresAfterHours = config.GetInt("Account.PasswordResetCodeExpiresAfterHours"),
                    PasswordResetUrl = baseUrl + config.GetValue("Account.PasswordResetUrl"),
                    RegistrationType = config.GetEnum<RegistrationType>("Account.RegistrationType")
                })
                .AddScoped<IPasswordHasher, CustomPasswordHasher>()
                .AddSingleton(new CustomPasswordHasherSettings
                {
                    HashByteSize = config.GetInt("Security.Passwords.HashByteSize"),
                    HashIterations = config.GetInt("Security.Passwords.HashIterations"),
                    SaltByteSize = config.GetInt("Security.Passwords.SaltByteSize")
                })
                .AddScoped<IUserStore<IdentityUser>, CustomUserStore>();
        }

        private static void RegisterServices(IDependencyContainer container, IConfiguration config)
        {
            container
                .AddScoped<IEmailSenderService, MailjetEmailSenderService>()
                .AddSingleton(new MailjetEmailSenderServiceSettings
                {
                    ApiKey = config.GetValue("Emails.Mailjet.ApiKey"),
                    ApiSecret = config.GetValue("Emails.Mailjet.ApiSecret"),
                    FromEmail = config.GetValue("Emails.Mailjet.FromEmail"),
                    FromName = config.GetValue("Emails.Mailjet.FromName"),
                })
                .AddScoped<IErrorLoggingService, ErrorLoggingService>()
                .AddSingleton(new ErrorLoggingServiceSettings
                {
                    CurrentEnvironment = config.GetEnum<ApplicationEnvironment>("Environment"),
                    Enabled = config.GetBool("Logging.Enabled")
                })
                // use singleton IFeedbackService to implement pub-sub pattern
                .AddSingleton<IFeedbackService>(new FeedbackService())
                .AddScoped<IInstrumentFactory, InstrumentFactory>()
                .AddScoped<IMusicTheoryService, MusicTheoryService>()
                .AddScoped<INotificationService, NotificationService>()
                .AddScoped<IQuestionnaireService, QuestionnaireService>()
                .AddScoped<IRecaptchaService, RecaptchaService>()
                .AddSingleton(new RecaptchaServiceSettings(
                    config.GetValue("Recaptcha.SecretKey"),
                    config.GetValue("Recaptcha.SiteKey"),
                    config.GetValue("Recaptcha.VerifyUrl")))
                .AddScoped<IUserAdminService, UserAdminService>()
                .AddScoped<IUserInstrumentService, UserInstrumentService>()
                .AddScoped<IUserService, UserService>();
        }

        private static void RegisterWebServices(IDependencyContainer container, IConfiguration config)
        {
            container
                .AddScoped<IClientStorageService, ClientStorageService>()
                .AddScoped<IContactService, ContactService>()
                .AddSingleton(new ContactServiceSettings
                {
                    ApplicationName = config.GetValue("Application.Name"),
                    ContactEmailAddress = config.GetValue("Contact.EmailAddress"),
                    Enabled = config.GetBool("Contact.Enabled")
                })
                .AddScoped<INoteMapViewModelService, NoteMapViewModelService>()
                .AddScoped<INotificationViewModelService, NotificationViewModelService>()
                .AddScoped<IQuestionnaireViewModelService, QuestionnaireViewModelService>()
                .AddSingleton(new QuestionnaireViewModelServiceSettings
                {
                    ApplicationName = config.GetValue("Application.Name"),
                    NotificationEmailAddress = config.GetValue("Contact.EmailAddress"),
                })
                // use singleton StateContainer to persist data across views in the same request
                .AddSingleton<IStateContainer>(new StateContainer())
                .AddScoped<IUserInstrumentViewModelService, UserInstrumentViewModelService>();
        }
    }
}
