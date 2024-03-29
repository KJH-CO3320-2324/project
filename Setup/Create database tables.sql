CREATE TABLE dbo.ApplicationEnvironments
(
	ApplicationEnvironmentId int NOT NULL,
	Name nvarchar(255) NOT NULL,
	CONSTRAINT PK_ApplicationEnvironments PRIMARY KEY CLUSTERED (ApplicationEnvironmentId),
	CONSTRAINT UQ_ApplicationEnvironments_Name UNIQUE (Name)
)
GO

INSERT INTO ApplicationEnvironments (ApplicationEnvironmentId, Name)
VALUES (1, 'Local'), (2, 'Live')
GO

CREATE TABLE dbo.ApplicationErrors
(
	ApplicationErrorId uniqueidentifier NOT NULL DEFAULT (newid()),
	CreatedUtc datetime NOT NULL DEFAULT (getutcdate()),
	Message nvarchar(max) NOT NULL,
	Type nvarchar(255) NULL,
	ApplicationEnvironmentId int NOT NULL DEFAULT (2),
	CONSTRAINT PK_ApplicationErrors PRIMARY KEY CLUSTERED (ApplicationErrorId),
	CONSTRAINT FK_ApplicationErrors_ApplicationEnvironments_ApplicationEnvironmentId FOREIGN KEY(ApplicationEnvironmentId) 
		REFERENCES ApplicationEnvironments (ApplicationEnvironmentId)
)
GO

CREATE TABLE dbo.ApplicationErrorProperties
(
	ApplicationErrorId uniqueidentifier NOT NULL DEFAULT (newid()),
	Name nvarchar(255) NOT NULL,
	Value nvarchar(max) NOT NULL,
	CONSTRAINT FK_ApplicationErrorProperties_ApplicationErrors_ApplicationErrorId FOREIGN KEY(ApplicationErrorId) 
		REFERENCES ApplicationErrors (ApplicationErrorId) ON DELETE CASCADE
)
GO

CREATE TABLE dbo.ContactRequests
(
	ContactRequestId uniqueidentifier NOT NULL DEFAULT (newid()),
	CreatedUtc datetime NOT NULL DEFAULT (getutcdate()),
	Email nvarchar(255) NOT NULL,
	Message nvarchar(max) NOT NULL,
	CONSTRAINT PK_ContactRequests PRIMARY KEY CLUSTERED (ContactRequestId)
)
GO

CREATE TABLE dbo.Notifications
(
	NotificationId uniqueidentifier NOT NULL DEFAULT (newid()),
	CreatedUtc datetime NOT NULL DEFAULT (getutcdate()),
	Active bit NOT NULL DEFAULT (0),
	HideForDays int NULL,
	Heading nvarchar(255) NULL,
	ContentHtml nvarchar(max) NOT NULL,
	CONSTRAINT PK_Notifications PRIMARY KEY CLUSTERED (NotificationId)
)
GO

CREATE TABLE dbo.Questionnaires
(
	QuestionnaireId uniqueidentifier NOT NULL DEFAULT (newid()),
	Name nvarchar(255) NOT NULL,
	CreatedUtc datetime NOT NULL DEFAULT (getutcdate()),
	ExpiresUtc datetime NULL,
	Active bit NOT NULL DEFAULT (0),
	LinkText nvarchar(255) NOT NULL,
	IntroText nvarchar(max) NOT NULL,
	CONSTRAINT PK_Questionnaires PRIMARY KEY CLUSTERED (QuestionnaireId),
	CONSTRAINT UQ_Questionnaires_Name UNIQUE NONCLUSTERED (Name)
)
GO

CREATE TABLE dbo.QuestionTypes
(
	QuestionTypeId int NOT NULL,
	Name nvarchar(255) NOT NULL,
	CONSTRAINT PK_QuestionTypes PRIMARY KEY CLUSTERED (QuestionTypeId),
	CONSTRAINT UQ_QuestionTypes_Name UNIQUE NONCLUSTERED (Name)
)
GO

INSERT INTO QuestionTypes (QuestionTypeId, Name)
VALUES (1, 'ShortText'), (2, 'LongText'), (3, 'Number'), (4, 'Boolean')
GO

CREATE TABLE dbo.QuestionnaireQuestions
(
	QuestionId uniqueidentifier NOT NULL DEFAULT (newid()),
	QuestionnaireId uniqueidentifier NOT NULL,
	DisplayOrder int NOT NULL,
	QuestionText nvarchar(255) NOT NULL,
	QuestionTypeId int NOT NULL,
	Required bit NOT NULL,
	Range nvarchar(255) NULL,
	MinValue int NULL,
	MaxValue int NULL,
	CONSTRAINT PK_QuestionId PRIMARY KEY NONCLUSTERED (QuestionId),
	CONSTRAINT UQ_QuestionnaireQuestions_QuestionnaireId_DisplayOrder UNIQUE NONCLUSTERED (QuestionnaireId, DisplayOrder),
	CONSTRAINT FK_QuestionnaireQuestions_QuestionTypes_QuestionTypeId FOREIGN KEY(QuestionTypeId)REFERENCES QuestionTypes (QuestionTypeId),
	CONSTRAINT FK_QuestionnaireQuestions_Questionnaires_QuestionnaireId FOREIGN KEY(QuestionnaireId) REFERENCES Questionnaires (QuestionnaireId) ON DELETE CASCADE
)
GO

CREATE CLUSTERED INDEX CIX_QuestionnaireQuestions_QuestionnaireId ON QuestionnaireQuestions (QuestionnaireId)
GO

CREATE TABLE dbo.RegistrationCodes
(
	RegistrationCodeId uniqueidentifier NOT NULL DEFAULT (newid()),
	CreatedUtc datetime NOT NULL DEFAULT (getutcdate()),
	ExpiresUtc datetime NULL,
	Code nvarchar(255) NOT NULL,
	Note nvarchar(max) NULL,
	CONSTRAINT PK_RegistrationCodes PRIMARY KEY CLUSTERED (RegistrationCodeId),
	CONSTRAINT UQ_RegistrationCodes_Code UNIQUE NONCLUSTERED (Code)
)
GO

CREATE TABLE dbo.Users
(
	UserId uniqueidentifier NOT NULL DEFAULT (newid()),
	CreatedUtc datetime NOT NULL DEFAULT (getutcdate()),
	Email nvarchar(255) NOT NULL,
	ActivatedUtc datetime NULL,
	PreventEmails bit NOT NULL DEFAULT (0),
	IsAdmin bit NOT NULL DEFAULT (0),
	CONSTRAINT PK_Users PRIMARY KEY CLUSTERED (UserId),
	CONSTRAINT UQ_Users_Email UNIQUE NONCLUSTERED (Email)	
)
GO

CREATE TABLE dbo.UserActivations
(
	UserActivationId uniqueidentifier NOT NULL DEFAULT (newid()),
	UserId uniqueidentifier NOT NULL,
	CreatedUtc datetime NOT NULL DEFAULT (getutcdate()),
	ExpiresUtc datetime NOT NULL,
	Code nvarchar(255) NOT NULL,
	CONSTRAINT PK_UserActivations PRIMARY KEY CLUSTERED (UserActivationId),
	CONSTRAINT FK_UserActivations_Users_UserId FOREIGN KEY(UserId) REFERENCES Users (UserId) ON DELETE CASCADE
)
GO

CREATE NONCLUSTERED INDEX IX_UserActivations_UserId ON UserActivations (UserId)
GO

CREATE TABLE dbo.UserLoginTokens
(
	UserLoginTokenId uniqueidentifier NOT NULL DEFAULT (newid()),
	UserId uniqueidentifier NOT NULL,
	CreatedUtc datetime NOT NULL DEFAULT (getutcdate()),
	ExpiresUtc datetime NOT NULL,
	Token nvarchar(255) NOT NULL,
	CONSTRAINT PK_UserLoginTokens PRIMARY KEY CLUSTERED (UserLoginTokenId),
	CONSTRAINT FK_UserLoginTokens_Users_UserId FOREIGN KEY(UserId) REFERENCES Users (UserId) ON DELETE CASCADE
)
GO

CREATE NONCLUSTERED INDEX IX_UserLoginTokens_UserId ON UserLoginTokens (UserId)
GO

CREATE TABLE dbo.UserNotifications
(
	UserNotificationId uniqueidentifier NOT NULL DEFAULT (newid()),
	UserId uniqueidentifier NOT NULL,
	NotificationId uniqueidentifier NOT NULL,
	CreatedUtc datetime NOT NULL DEFAULT (getutcdate()),
	HiddenUtc datetime NULL,
	Dismissed bit NOT NULL DEFAULT (0),
	CONSTRAINT PK_UserNotifications PRIMARY KEY CLUSTERED (UserNotificationId),
	CONSTRAINT UQ_UserNotifications_UserId_NotificationId UNIQUE NONCLUSTERED (UserId, NotificationId),
	CONSTRAINT FK_UserNotifications_Users_UserId FOREIGN KEY(UserId) REFERENCES Users (UserId) ON DELETE CASCADE,
	CONSTRAINT FK_UserNotifications_Notifications_NotificationId FOREIGN KEY(NotificationId) REFERENCES Notifications (NotificationId) ON DELETE CASCADE
)
GO

CREATE TABLE dbo.UserPasswordResetCodes
(
	UserPasswordResetCodeId uniqueidentifier NOT NULL DEFAULT (newid()),
	UserId uniqueidentifier NOT NULL,
	CreatedUtc datetime NOT NULL DEFAULT (getutcdate()),
	ExpiresUtc datetime NOT NULL,
	Code nvarchar(255) NOT NULL,
	CONSTRAINT PK_UserPasswordResetCodes PRIMARY KEY NONCLUSTERED (UserPasswordResetCodeId),
	CONSTRAINT FK_UserPasswordResetCodes_Users_UserId FOREIGN KEY(UserId) REFERENCES Users (UserId) ON DELETE CASCADE
)
GO

CREATE CLUSTERED INDEX CIX_UserPasswordResetCodes_UserId ON UserPasswordResetCodes (UserId)
GO

CREATE TABLE dbo.UserPasswords
(
	UserPasswordId uniqueidentifier NOT NULL DEFAULT (newid()),
	UserId uniqueidentifier NOT NULL,
	Hash nvarchar(255) NOT NULL,
	Salt nvarchar(255) NOT NULL,
	ResetCode nvarchar(255) NULL,
	ResetCodeExpiresUtc datetime NULL,
	CONSTRAINT PK_UserPasswords PRIMARY KEY CLUSTERED (UserPasswordId),
	CONSTRAINT FK_UserPasswords_Users_UserId FOREIGN KEY(UserId) REFERENCES Users (UserId) ON DELETE CASCADE
)
GO

CREATE UNIQUE NONCLUSTERED INDEX UIX_UserPasswords_UserId ON UserPasswords (UserId)
GO

CREATE TABLE dbo.UserPreferenceTypes
(
	UserPreferenceTypeId int NOT NULL,
	Name nvarchar(255) NOT NULL,
	CONSTRAINT PK_UserPreferenceTypes PRIMARY KEY CLUSTERED (UserPreferenceTypeId),
	CONSTRAINT UQ_UserPreferenceTypes_Name UNIQUE NONCLUSTERED (Name)
)
GO

INSERT INTO UserPreferenceTypes (UserPreferenceTypeId, Name)
VALUES (1, 'Accidental'), (2, 'Intervals')
GO

CREATE TABLE dbo.UserPreferences
(
	UserId uniqueidentifier NOT NULL,
	UserPreferenceTypeId int NOT NULL,
	Value nvarchar(255) NOT NULL,
	CONSTRAINT PK_UserPreferences PRIMARY KEY NONCLUSTERED (UserId, UserPreferenceTypeId),
	CONSTRAINT FK_UserPreferences_Users_UserId FOREIGN KEY(UserId) REFERENCES Users (UserId) ON DELETE CASCADE,
	CONSTRAINT FK_UserPreferences_UserPreferenceTypes_UserPreferenceTypeId FOREIGN KEY(UserPreferenceTypeId) REFERENCES UserPreferenceTypes (UserPreferenceTypeId)
)
GO

CREATE CLUSTERED INDEX CIX_UserPreferences_UserId ON UserPreferences (UserId)
GO

CREATE TABLE dbo.UserQuestionResponses
(
	ResponseId uniqueidentifier NOT NULL DEFAULT (newid()),
	CreatedUtc datetime NOT NULL DEFAULT (getutcdate()),
	UserId uniqueidentifier NOT NULL,
	QuestionId uniqueidentifier NOT NULL,
	Value nvarchar(max) NULL,
	CONSTRAINT PK_UserQuestionResponses PRIMARY KEY NONCLUSTERED (ResponseId),
	CONSTRAINT FK_UserQuestionResponses_Users_UserId FOREIGN KEY(UserId) REFERENCES Users (UserId) ON DELETE CASCADE,
	CONSTRAINT FK_UserQuestionResponses_QuestionnaireQuestions_QuestionId FOREIGN KEY(QuestionId) REFERENCES QuestionnaireQuestions (QuestionId)
)
GO

CREATE CLUSTERED INDEX CIX_UserQuestionnaireResponse_UserId ON UserQuestionResponses (UserId)
GO

CREATE NONCLUSTERED INDEX IX_UserQuestionnaireResponses_QuestionId ON UserQuestionResponses (QuestionId)
GO

CREATE TABLE dbo.UserRegistrationCodes
(
	UserRegistrationCodeId uniqueidentifier NOT NULL DEFAULT (newid()),
	CreatedUtc datetime NOT NULL DEFAULT (getutcdate()),
	UserId uniqueidentifier NOT NULL,
	RegistrationCodeId uniqueidentifier NOT NULL,
	CONSTRAINT PK_UserRegistrationCodes PRIMARY KEY CLUSTERED (UserRegistrationCodeId),
	CONSTRAINT UQ_UserRegistrationCodes_UserId UNIQUE NONCLUSTERED (UserId),
	CONSTRAINT FK_UserRegistrationCodes_Users_UserId FOREIGN KEY(UserId) REFERENCES Users (UserId) ON DELETE CASCADE,
	CONSTRAINT FK_UserRegistrationCodes_RegistrationCodes_RegistrationCodeId FOREIGN KEY(RegistrationCodeId) REFERENCES RegistrationCodes (RegistrationCodeId)
)
GO