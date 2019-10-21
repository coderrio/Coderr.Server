IF OBJECT_ID(N'dbo.[Settings]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[Settings](
		[Section] [varchar](50) NOT NULL,
		[Name] [varchar](50) NOT NULL,
		[Value] [varchar](512),
	 ) ON [PRIMARY]
 END


IF OBJECT_ID(N'dbo.[Accounts]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[Accounts](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[UserName] [varchar](50) NOT NULL,
		[HashedPassword] [varchar](512) NOT NULL,
		[CreatedAtUtc] [datetime] NOT NULL,
		[Email] [varchar](255) NOT NULL,
		[Salt] [varchar](512) NOT NULL,
		[AccountState] [varchar](20) NOT NULL,
		[TrackingId] [varchar](40) NULL,
		[LoginAttempts] [int] NOT NULL,
		[LastLoginAtUtc] [datetime] NULL,
		[ActivationKey] [varchar](50) NULL,
		[PromotionCode] [varchar](50) NULL,
		[UpdatedAtUtc] [datetime] NULL,
	 CONSTRAINT [accounts_pkey] PRIMARY KEY CLUSTERED ([Id] ASC)
	 ) ON [PRIMARY]
 END

 IF OBJECT_ID(N'dbo.[InvalidReports]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[InvalidReports](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[AppKey] [varchar](36) NOT NULL,
		[Signature] [varchar](36) NOT NULL,
		[ReportBody] [ntext] NOT NULL,
		[ErrorMessage] [varchar](2000) NOT NULL,
		[CreatedAtUtc] [datetime] NOT NULL,
	 CONSTRAINT [invalidreports_pkey] PRIMARY KEY CLUSTERED ([Id] ASC)
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END


IF OBJECT_ID(N'dbo.[Invitations]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[Invitations](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[Email] [varchar](2000) NOT NULL,
		[InvitationKey] [char](32) NOT NULL,
		[CreatedAtUtc] [datetime] NOT NULL,
		[InvitedBy] varchar(50) NOT NULL,
		[Invitations] varchar(2500) NOT NULL,
	
	 CONSTRAINT [invitations_pkey] PRIMARY KEY CLUSTERED  ([Id] ASC)
	) ON [PRIMARY]
END

IF OBJECT_ID(N'dbo.[Applications]', N'U') IS NULL
BEGIN
		CREATE TABLE [dbo].[Applications] (
        [Id]              INT           IDENTITY (1, 1) NOT NULL primary key,
        [Name]            NVARCHAR (50) NOT NULL,
        [AppKey]          VARCHAR (36)  NOT NULL,
        [CreatedById]     INT           NOT NULL,
        [CreatedAtUtc]    DATETIME      NOT NULL,
        [ApplicationType] VARCHAR (40)  NOT NULL,
        [SharedSecret]    VARCHAR (36)  NOT NULL
	);
END

IF OBJECT_ID(N'dbo.[CollectionMetadata]', N'U') IS NULL
BEGIN
		CREATE TABLE [dbo].[CollectionMetadata] (
        [Id]            INT           IDENTITY (1, 1) NOT NULL primary key,
        [Name]          NVARCHAR (50) NOT NULL,
        [ApplicationId] INT           NOT NULL,
        [Properties]    NTEXT         NOT NULL
	);
END

IF OBJECT_ID(N'dbo.[ErrorOrigins]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[ErrorOrigins] (
        [Id]             INT            IDENTITY (1, 1) NOT NULL primary key,
        [IpAddress]      VARCHAR (20)   NOT NULL,
        [CountryCode]    VARCHAR (5)    NULL,
        [CountryName]    VARCHAR (30)   NULL,
        [RegionCode]     VARCHAR (5)    NULL,
        [RegionName]     VARCHAR (30)   NULL,
        [City]           NVARCHAR (30)  NULL,
        [ZipCode]        VARCHAR (10)   NULL,
        [Latitude]       DECIMAL (9, 6) NOT NULL,
        [Longitude]      DECIMAL (9, 6) NOT NULL,
        [CreatedAtUtc]   DATETIME       NOT NULL
	);
END

IF OBJECT_ID(N'dbo.[ErrorReportOrigins]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[ErrorReportOrigins] (
        [ErrorOriginId]             INT NOT NULL,
        [IncidentId]             INT NOT NULL,
        [ReportId]    INT  NOT NULL,
        [ApplicationId]    INT  NOT NULL,
        [CreatedAtUtc]   DATETIME       NOT NULL
	);
END


IF OBJECT_ID(N'dbo.[ErrorReports]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[ErrorReports] (
        [Id]                 INT            IDENTITY (1, 1) NOT NULL primary key,
        [IncidentId]         INT            NOT NULL,
        [ErrorId]            VARCHAR (36)   NOT NULL,
        [ApplicationId]      INT            NOT NULL,
        [ReportHashCode]     VARCHAR (20)   NOT NULL,
        [CreatedAtUtc]       DATETIME       NOT NULL,
        [SolvedAtUtc]        DATETIME       NULL,
        [Title]						     NVARCHAR(100)  NULL,
        [RemoteAddress]      VARCHAR (45)   NULL, --SEE http://stackoverflow.com/questions/166132/maximum-length-of-the-textual-representation-of-an-ipv6-address
        [Exception]          NTEXT          NOT NULL,
        [ContextInfo]        NTEXT          NOT NULL
	);
END


IF OBJECT_ID(N'dbo.[ErrorReports_IncidentId]', N'I') IS NULL
BEGIN
	CREATE NONCLUSTERED INDEX [ErrorReports_IncidentId]
        ON [dbo].[ErrorReports]([IncidentId] ASC, [CreatedAtUtc] DESC);
END

IF OBJECT_ID(N'dbo.[Application_GetWeeklyStats]', N'I') IS NULL
BEGIN
	CREATE NONCLUSTERED INDEX [Application_GetWeeklyStats]
        ON [dbo].[ErrorReports]([ApplicationId] ASC, [CreatedAtUtc] DESC);
END

IF OBJECT_ID(N'dbo.[IncidentFeedback]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[IncidentFeedback] (
        [Id]                 INT            IDENTITY (1, 1) NOT NULL primary key,
        [ApplicationId]      INT            NULL,
        [IncidentId]         INT            NULL,
        [ReportId]           INT            NULL,
        [CreatedAtUtc]       DATETIME       NOT NULL,
        [RemoteAddress]      VARCHAR (20)   NOT NULL,
        [Description]        NTEXT          NOT NULL,
        [EmailAddress]       NVARCHAR (512) NULL,
        [Conversation]       NTEXT          NOT NULL,
        [ConversationLength] INT            NOT NULL,
        [ErrorReportId]      VARCHAR (40)   NOT NULL,
        [Replied]            INT NOT NULL default 0
	);
END


IF OBJECT_ID(N'dbo.[Incidents]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[Incidents] (
		[Id]                      INT            IDENTITY (1, 1) NOT NULL,
		[ReportHashCode]          VARCHAR (20)   NOT NULL,
		[ApplicationId]           INT            NOT NULL,
		[CreatedAtUtc]            DATETIME       NOT NULL,
		[HashCodeIdentifier]      VARCHAR (1024) NOT NULL,
		[ReportCount]             INT            NOT NULL,
		[UpdatedAtUtc]            DATETIME       NULL,
		[Description]             NTEXT          NOT NULL,
		[FullName]                NVARCHAR (255) NOT NULL,
		[Solution]                NTEXT          NULL,
		[IsSolved]                BINARY (1)     NOT NULL default(0),
		[IsSolutionShared]        BINARY (1)     NOT NULL default(0),
		[SolvedAtUtc]             DATETIME       NULL,
		[StackTrace]              NTEXT          NULL,
		[IsReOpened]              BIT            NOT NULL default(0),
		[ReOpenedAtUtc]           DATETIME       NULL,
		[PreviousSolutionAtUtc]   DATETIME       NULL,
		[IgnoreReports]           BIT            NOT NULL default(0),
		[IgnoringReportsSinceUtc] DATETIME       NULL,
		[IgnoringRequestedBy]     NVARCHAR (50)  NULL,
		[LastSolutionAtUtc]       DATETIME       NULL
	);
END

IF OBJECT_ID(N'dbo.[IncidentTags]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[IncidentTags] (
        [id]          INT          IDENTITY (1, 1) NOT NULL primary key,
        [IncidentId]  INT          NOT NULL,
        [TagName]     VARCHAR (40) NOT NULL,
        [OrderNumber] INT          NOT NULL
	);
END


IF OBJECT_ID(N'dbo.[IncidentTags_FromIncident]', N'U') IS NULL
BEGIN
	CREATE NONCLUSTERED INDEX [IncidentTags_FromIncident]
        ON [dbo].[IncidentTags]([IncidentId] ASC, [OrderNumber] ASC);
END

IF OBJECT_ID(N'dbo.[ReportContextInfo]', N'U') IS NULL
BEGIN
		CREATE TABLE [dbo].[ReportContextInfo] (
        [Id]           INT             IDENTITY (1, 1) NOT NULL primary key,
        [IncidentId]   INT             NOT NULL,
        [ReportId]     INT             NOT NULL,
        [CreatedAtUtc] DATETIME        NOT NULL,
        [UpdatedAtUtc] DATETIME        NULL,
        [Name]         NVARCHAR (1024) NULL,
        [Value]        NVARCHAR (20)   NULL,
        [LargeValue]   NTEXT           NOT NULL
	);
END

IF OBJECT_ID(N'dbo.[IncidentContextCollections]', N'U') IS NULL
BEGIN
		CREATE TABLE [dbo].[IncidentContextCollections] (
        [Id]                      INT          IDENTITY (1, 1) NOT NULL primary key,
        [IncidentId]              INT          NOT NULL,
        [Name]                    VARCHAR (250) NOT NULL,
        [Properties]              text NOT NULL
	);
END

IF OBJECT_ID(N'dbo.[IncidentContextCollections_IncidentId]', N'U') IS NULL
BEGIN
	CREATE NONCLUSTERED INDEX [IncidentContextCollections_IncidentId]
        ON [dbo].[IncidentContextCollections]([IncidentId] ASC);
END


IF OBJECT_ID(N'dbo.[Triggers]', N'U') IS NULL
BEGIN
		CREATE TABLE [dbo].[Triggers] (
        [Id]                      INT            IDENTITY (1, 1) NOT NULL primary key,
        [Name]                    NVARCHAR (50)  NOT NULL,
        [Description]             NVARCHAR (512) NOT NULL,
        [ApplicationId]           INT            NOT NULL,
        [Rules]                   NTEXT          NOT NULL,
        [Actions]                 NTEXT          NOT NULL,
        [LastTriggerAction]       NVARCHAR (50)  NOT NULL,
        [RunForNewIncidents]      BIT            NOT NULL,
        [RunForExistingIncidents] BIT            NOT NULL,
        [RunForReOpenedIncidents] BIT            NOT NULL
	);
END


IF OBJECT_ID(N'dbo.[UserNotificationSettings]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[UserNotificationSettings] (
		[AccountId]        INT          NOT NULL,
		[ApplicationId]    INT          NOT NULL,
		[NewIncident]      VARCHAR (20) NOT NULL default 'Disabled',
		[NewReport]        VARCHAR (20) NOT NULL default 'Disabled',
		[ReOpenedIncident] VARCHAR (20) NOT NULL default 'Disabled',
		[WeeklySummary]    VARCHAR (20) NOT NULL default 'Disabled',
		[ApplicationSpike] VARCHAR (20) NOT NULL default 'Disabled',
		[UserFeedback]     VARCHAR (20) NOT NULL default 'Disabled'
	);
END
ALTER TABLE [UserNotificationSettings]
        ADD CONSTRAINT pk_UserNotificationSettings PRIMARY KEY (AccountId, ApplicationId);

CREATE TABLE dbo.Users
(
        AccountId				INT NOT NULL primary key,
        EmailAddress		varchar(255) not null,
        FirstName				varchar(100),
        LastName				varchar(100),
        UserName				varchar(100),
		MobileNumber		varchar(100)
	);


IF OBJECT_ID(N'dbo.[ApplicationMembers]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[ApplicationMembers] (
        [AccountId]     INT           NULL foreign key references Accounts (Id),
        [ApplicationId] INT           NOT NULL foreign key references Applications (Id),
		[EmailAddress]  nvarchar(255) not null,
        [AddedAtUtc]    DATETIME      NOT NULL,
        [AddedByName]   VARCHAR (50)  NOT NULL,
        [Roles]         VARCHAR (255) NOT NULL
	);
END

IF OBJECT_ID(N'dbo.[QueueEvents]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[QueueEvents] (
        [Id]     INT identity      not    NULL primary key,
		[ApplicationId] INT NOT NULL, 
        [CreatedAtUtc]    DATETIME      NOT NULL,
        [AssemblyQualifiedTypeName]   VARCHAR (255)  NOT NULL,
        [Body]         text NOT NULL,
	);
END

IF OBJECT_ID(N'dbo.[QueueReports]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[QueueReports] (
        [Id]     INT identity      not    NULL primary key,
		[ApplicationId] INT NOT NULL, 
        [CreatedAtUtc]    DATETIME      NOT NULL,
        [AssemblyQualifiedTypeName]   VARCHAR (255)  NOT NULL,
        [Body]         text NOT NULL,
		
	);
END


IF OBJECT_ID(N'dbo.[QueueFeedback]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[QueueFeedback] (
        [Id]     INT identity      not    NULL primary key,
		[ApplicationId] INT NOT NULL, 
        [CreatedAtUtc]    DATETIME      NOT NULL,
        [AssemblyQualifiedTypeName]   VARCHAR (255)  NOT NULL,
        [Body]         text NOT NULL,
		
	);
END
