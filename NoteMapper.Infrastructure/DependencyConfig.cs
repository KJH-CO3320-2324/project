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
using NoteMapper.Data.Json;
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
            string? sqlConnectionString = config.GetConnectionString(sqlConnectionStringName)
                ?.Replace("{rootdirectory}", AppContext.BaseDirectory);

            container
                .AddSingleton(new SqlRepositorySettings
                {
                    ConnectionString = sqlConnectionString ?? "",
                    CurrentEnvironment = config.GetEnum<ApplicationEnvironment>("Environment"),
                    Provider = config.GetValue("Data.Sql.Provider")
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

            RegisterDocumentStorageData(container, config);
        }

        private static void RegisterDocumentStorageData(IDependencyContainer container, IConfiguration config)
        {
            string provider = config.GetValue("Data.DocumentStorage.Provider");
            string connectionStringName = config.GetValue("Data.DocumentStorage.ConnectionStringName");
            string connectionString = config.GetConnectionString(connectionStringName) ?? "";
            string defaultUserId = config.GetValue("Data.DocumentStorage.DefaultUserId");
            
            ApplicationEnvironment environment = config.GetEnum<ApplicationEnvironment>("Environment");

            if (string.Equals(provider, "Cosmos", StringComparison.InvariantCultureIgnoreCase))
            {
                string applicationName = config.GetValue("Application.Name");

                container
                    .AddSingleton(new AzureCosmosRepositorySettings
                    {
                        ApplicationName = applicationName,
                        ConnectionString = connectionString,
                        CurrentEnvironment = environment,
                        DatabaseId = config.GetValue("Data.DocumentStorage.Cosmos.DatabaseId"),
                        DefaultUserId = defaultUserId
                    })
                    .AddScoped<IUserInstrumentRepository, UserInstrumentAzureCosmosRepository>();
            }
            else if (string.Equals(provider, "Mongo", StringComparison.InvariantCultureIgnoreCase))
            {
                container
                    .AddSingleton(new MongoRepositorySettings
                    {
                        ConnectionString = connectionString,
                        CurrentEnvironment = environment,
                        DatabaseId = config.GetValue("Data.DocumentStorage.Mongo.DatabaseId"),
                        DefaultUserId = defaultUserId
                    })
                    .AddScoped<IUserInstrumentRepository, UserInstrumentMongoRepository>();
            }
            else if (string.Equals(provider, "Json", StringComparison.InvariantCultureIgnoreCase))
            {
                container
                    .AddSingleton(new JsonRepositorySettings
                    {
                        CurrentEnvironment = environment,
                        DefaultUserId = defaultUserId,
                        FilePath = config.GetValue("Data.DocumentStorage.Json.Path")
                            .Replace("{rootdirectory}", AppContext.BaseDirectory)
                    })
                    .AddScoped<IUserInstrumentRepository, UserInstrumentJsonRepository>();
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
