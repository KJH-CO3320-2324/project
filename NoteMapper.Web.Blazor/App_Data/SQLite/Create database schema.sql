CREATE TABLE ApplicationEnvironments
(
	ApplicationEnvironmentId int NOT NULL,
	Name nvarchar(255) NOT NULL,
	CONSTRAINT PK_ApplicationEnvironments PRIMARY KEY (ApplicationEnvironmentId),
	CONSTRAINT UQ_ApplicationEnvironments_Name UNIQUE (Name)
);

INSERT INTO ApplicationEnvironments (ApplicationEnvironmentId, Name)
VALUES (1, 'Local'), (2, 'Live');

CREATE TABLE ApplicationErrors
(
	ApplicationErrorId uniqueidentifier NOT NULL,
	CreatedUtc datetime NOT NULL,
	Message nvarchar NOT NULL,
	Type nvarchar(255) NULL,
	ApplicationEnvironmentId int NOT NULL DEFAULT (2),
	CONSTRAINT PK_ApplicationErrors PRIMARY KEY (ApplicationErrorId),
	CONSTRAINT FK_ApplicationErrors_ApplicationEnvironments_ApplicationEnvironmentId FOREIGN KEY(ApplicationEnvironmentId) 
		REFERENCES ApplicationEnvironments (ApplicationEnvironmentId)
);

CREATE TABLE ApplicationErrorProperties
(
	ApplicationErrorId uniqueidentifier NOT NULL,
	Name nvarchar(255) NOT NULL,
	Value nvarchar NOT NULL,
	CONSTRAINT FK_ApplicationErrorProperties_ApplicationErrors_ApplicationErrorId FOREIGN KEY(ApplicationErrorId) 
		REFERENCES ApplicationErrors (ApplicationErrorId) ON DELETE CASCADE
);

CREATE TABLE ContactRequests
(
	ContactRequestId uniqueidentifier NOT NULL,
	CreatedUtc datetime NOT NULL,
	Email nvarchar(255) NOT NULL,
	Message nvarchar NOT NULL,
	CONSTRAINT PK_ContactRequests PRIMARY KEY (ContactRequestId)
);

CREATE TABLE Notifications
(
	NotificationId uniqueidentifier NOT NULL,
	CreatedUtc datetime NOT NULL,
	Active bit NOT NULL DEFAULT (0),
	HideForDays int NULL,
	Heading nvarchar(255) NULL,
	ContentHtml nvarchar NOT NULL,
	CONSTRAINT PK_Notifications PRIMARY KEY (NotificationId)
);

CREATE TABLE Questionnaires
(
	QuestionnaireId uniqueidentifier NOT NULL,
	Name nvarchar(255) NOT NULL,
	CreatedUtc datetime NOT NULL,
	ExpiresUtc datetime NULL,
	Active bit NOT NULL DEFAULT (0),
	LinkText nvarchar(255) NOT NULL,
	IntroText nvarchar NOT NULL,
	CONSTRAINT PK_Questionnaires PRIMARY KEY (QuestionnaireId),
	CONSTRAINT UQ_Questionnaires_Name UNIQUE (Name)
);

CREATE TABLE QuestionTypes
(
	QuestionTypeId int NOT NULL,
	Name nvarchar(255) NOT NULL,
	CONSTRAINT PK_QuestionTypes PRIMARY KEY (QuestionTypeId),
	CONSTRAINT UQ_QuestionTypes_Name UNIQUE (Name)
);

INSERT INTO QuestionTypes (QuestionTypeId, Name)
VALUES (1, 'ShortText'), (2, 'LongText'), (3, 'Number'), (4, 'Boolean');

CREATE TABLE QuestionnaireQuestions
(
	QuestionId uniqueidentifier NOT NULL,
	QuestionnaireId uniqueidentifier NOT NULL,
	DisplayOrder int NOT NULL,
	QuestionText nvarchar(255) NOT NULL,
	QuestionTypeId int NOT NULL,
	Required bit NOT NULL,
	Range nvarchar(255) NULL,
	MinValue int NULL,
	MaxValue int NULL,
	CONSTRAINT PK_QuestionId PRIMARY KEY (QuestionId),
	CONSTRAINT UQ_QuestionnaireQuestions_QuestionnaireId_DisplayOrder UNIQUE (QuestionnaireId, DisplayOrder),
	CONSTRAINT FK_QuestionnaireQuestions_QuestionTypes_QuestionTypeId FOREIGN KEY(QuestionTypeId)REFERENCES QuestionTypes (QuestionTypeId),
	CONSTRAINT FK_QuestionnaireQuestions_Questionnaires_QuestionnaireId FOREIGN KEY(QuestionnaireId) REFERENCES Questionnaires (QuestionnaireId) ON DELETE CASCADE
);

CREATE INDEX CIX_QuestionnaireQuestions_QuestionnaireId ON QuestionnaireQuestions (QuestionnaireId);

CREATE TABLE RegistrationCodes
(
	RegistrationCodeId uniqueidentifier NOT NULL,
	CreatedUtc datetime NOT NULL,
	ExpiresUtc datetime NULL,
	Code nvarchar(255) NOT NULL,
	Note nvarchar NULL,
	CONSTRAINT PK_RegistrationCodes PRIMARY KEY (RegistrationCodeId),
	CONSTRAINT UQ_RegistrationCodes_Code UNIQUE (Code)
);

CREATE TABLE Users
(
	UserId uniqueidentifier NOT NULL,
	CreatedUtc datetime NOT NULL,
	Email nvarchar(255) NOT NULL,
	ActivatedUtc datetime NULL,
	PreventEmails bit NOT NULL DEFAULT (0),
	IsAdmin bit NOT NULL DEFAULT (0),
	CONSTRAINT PK_Users PRIMARY KEY (UserId),
	CONSTRAINT UQ_Users_Email UNIQUE (Email)	
);

CREATE TABLE UserActivations
(
	UserActivationId uniqueidentifier NOT NULL,
	UserId uniqueidentifier NOT NULL,
	CreatedUtc datetime NOT NULL,
	ExpiresUtc datetime NOT NULL,
	Code nvarchar(255) NOT NULL,
	CONSTRAINT PK_UserActivations PRIMARY KEY (UserActivationId),
	CONSTRAINT FK_UserActivations_Users_UserId FOREIGN KEY(UserId) REFERENCES Users (UserId) ON DELETE CASCADE
);

CREATE INDEX IX_UserActivations_UserId ON UserActivations (UserId);

CREATE TABLE UserLoginTokens
(
	UserLoginTokenId uniqueidentifier NOT NULL,
	UserId uniqueidentifier NOT NULL,
	CreatedUtc datetime NOT NULL,
	ExpiresUtc datetime NOT NULL,
	Token nvarchar(255) NOT NULL,
	CONSTRAINT PK_UserLoginTokens PRIMARY KEY (UserLoginTokenId),
	CONSTRAINT FK_UserLoginTokens_Users_UserId FOREIGN KEY(UserId) REFERENCES Users (UserId) ON DELETE CASCADE
);

CREATE INDEX IX_UserLoginTokens_UserId ON UserLoginTokens (UserId);

CREATE TABLE UserNotifications
(
	UserNotificationId uniqueidentifier NOT NULL,
	UserId uniqueidentifier NOT NULL,
	NotificationId uniqueidentifier NOT NULL,
	CreatedUtc datetime NOT NULL,
	HiddenUtc datetime NULL,
	Dismissed bit NOT NULL DEFAULT (0),
	CONSTRAINT PK_UserNotifications PRIMARY KEY (UserNotificationId),
	CONSTRAINT UQ_UserNotifications_UserId_NotificationId UNIQUE (UserId, NotificationId),
	CONSTRAINT FK_UserNotifications_Users_UserId FOREIGN KEY(UserId) REFERENCES Users (UserId) ON DELETE CASCADE,
	CONSTRAINT FK_UserNotifications_Notifications_NotificationId FOREIGN KEY(NotificationId) REFERENCES Notifications (NotificationId) ON DELETE CASCADE
);

CREATE TABLE UserPasswordResetCodes
(
	UserPasswordResetCodeId uniqueidentifier NOT NULL,
	UserId uniqueidentifier NOT NULL,
	CreatedUtc datetime NOT NULL,
	ExpiresUtc datetime NOT NULL,
	Code nvarchar(255) NOT NULL,
	CONSTRAINT PK_UserPasswordResetCodes PRIMARY KEY (UserPasswordResetCodeId),
	CONSTRAINT FK_UserPasswordResetCodes_Users_UserId FOREIGN KEY(UserId) REFERENCES Users (UserId) ON DELETE CASCADE
);

CREATE INDEX CIX_UserPasswordResetCodes_UserId ON UserPasswordResetCodes (UserId);

CREATE TABLE UserPasswords
(
	UserPasswordId uniqueidentifier NOT NULL,
	UserId uniqueidentifier NOT NULL,
	Hash nvarchar(255) NOT NULL,
	Salt nvarchar(255) NOT NULL,
	ResetCode nvarchar(255) NULL,
	ResetCodeExpiresUtc datetime NULL,
	CONSTRAINT PK_UserPasswords PRIMARY KEY (UserPasswordId),
	CONSTRAINT FK_UserPasswords_Users_UserId FOREIGN KEY(UserId) REFERENCES Users (UserId) ON DELETE CASCADE
);

CREATE UNIQUE INDEX UIX_UserPasswords_UserId ON UserPasswords (UserId);

CREATE TABLE UserPreferenceTypes
(
	UserPreferenceTypeId int NOT NULL,
	Name nvarchar(255) NOT NULL,
	CONSTRAINT PK_UserPreferenceTypes PRIMARY KEY (UserPreferenceTypeId),
	CONSTRAINT UQ_UserPreferenceTypes_Name UNIQUE (Name)
);

INSERT INTO UserPreferenceTypes (UserPreferenceTypeId, Name)
VALUES (1, 'Accidental'), (2, 'Intervals');

CREATE TABLE UserPreferences
(
	UserId uniqueidentifier NOT NULL,
	UserPreferenceTypeId int NOT NULL,
	Value nvarchar(255) NOT NULL,
	CONSTRAINT PK_UserPreferences PRIMARY KEY (UserId, UserPreferenceTypeId),
	CONSTRAINT FK_UserPreferences_Users_UserId FOREIGN KEY(UserId) REFERENCES Users (UserId) ON DELETE CASCADE,
	CONSTRAINT FK_UserPreferences_UserPreferenceTypes_UserPreferenceTypeId FOREIGN KEY(UserPreferenceTypeId) REFERENCES UserPreferenceTypes (UserPreferenceTypeId)
);

CREATE INDEX CIX_UserPreferences_UserId ON UserPreferences (UserId);

CREATE TABLE UserQuestionResponses
(
	ResponseId uniqueidentifier NOT NULL,
	CreatedUtc datetime NOT NULL,
	UserId uniqueidentifier NOT NULL,
	QuestionId uniqueidentifier NOT NULL,
	Value nvarchar NULL,
	CONSTRAINT PK_UserQuestionResponses PRIMARY KEY (ResponseId),
	CONSTRAINT FK_UserQuestionResponses_Users_UserId FOREIGN KEY(UserId) REFERENCES Users (UserId) ON DELETE CASCADE,
	CONSTRAINT FK_UserQuestionResponses_QuestionnaireQuestions_QuestionId FOREIGN KEY(QuestionId) REFERENCES QuestionnaireQuestions (QuestionId)
);

CREATE INDEX CIX_UserQuestionnaireResponse_UserId ON UserQuestionResponses (UserId);

CREATE INDEX IX_UserQuestionnaireResponses_QuestionId ON UserQuestionResponses (QuestionId);

CREATE TABLE UserRegistrationCodes
(
	UserRegistrationCodeId uniqueidentifier NOT NULL,
	CreatedUtc datetime NOT NULL,
	UserId uniqueidentifier NOT NULL,
	RegistrationCodeId uniqueidentifier NOT NULL,
	CONSTRAINT PK_UserRegistrationCodes PRIMARY KEY (UserRegistrationCodeId),
	CONSTRAINT UQ_UserRegistrationCodes_UserId UNIQUE (UserId),
	CONSTRAINT FK_UserRegistrationCodes_Users_UserId FOREIGN KEY(UserId) REFERENCES Users (UserId) ON DELETE CASCADE,
	CONSTRAINT FK_UserRegistrationCodes_RegistrationCodes_RegistrationCodeId FOREIGN KEY(RegistrationCodeId) REFERENCES RegistrationCodes (RegistrationCodeId)
);