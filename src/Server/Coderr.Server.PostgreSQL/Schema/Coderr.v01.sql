CREATE TABLE IF NOT EXISTS Settings (
		Section varchar(50) NOT NULL,
		Name varchar(50) NOT NULL,
		Value varchar(512)
	 );
       
CREATE TABLE IF NOT EXISTS Accounts (
		Id int GENERATED ALWAYS AS IDENTITY NOT NULL,
		UserName varchar(50) NOT NULL,
		HashedPassword varchar(512) NOT NULL,
		CreatedAtUtc timestamp  NOT NULL,
		Email varchar(255) NOT NULL,
		Salt varchar(512) NOT NULL,
		AccountState varchar(20) NOT NULL,
		TrackingId varchar(40) NULL,
		LoginAttempts int NOT NULL,
		LastLoginAtUtc timestamp  NULL,
		ActivationKey varchar(50) NULL,
		PromotionCode varchar(50) NULL,
		UpdatedAtUtc timestamp  NULL 
	 ); 

CREATE TABLE IF NOT EXISTS InvalidReports(
		Id int GENERATED ALWAYS AS IDENTITY NOT NULL,
		AppKey varchar(36) NOT NULL,
		Signature varchar(36) NOT NULL,
		ReportBody text NOT NULL,
		ErrorMessage varchar(2000) NOT NULL,
		CreatedAtUtc timestamp  NOT NULL 
	);


CREATE TABLE IF NOT EXISTS  Invitations(
		Id int GENERATED ALWAYS AS IDENTITY NOT NULL,
		Email varchar(2000) NOT NULL,
		InvitationKey char(32) NOT NULL,
		CreatedAtUtc timestamp  NOT NULL,
		InvitedBy varchar(50) NOT NULL,
		Invitations varchar(2500) NOT NULL);

CREATE TABLE IF NOT EXISTS  Applications (
       Id int GENERATED ALWAYS AS IDENTITY NOT NULL,
        Name            VARCHAR (50) NOT NULL,
        AppKey          VARCHAR (36)  NOT NULL,
        CreatedById     INT           NOT NULL,
        CreatedAtUtc    timestamp       NOT NULL,
        ApplicationType VARCHAR (40)  NOT NULL,
        SharedSecret    VARCHAR (36)  NOT NULL
	);

CREATE TABLE IF NOT EXISTS  CollectionMetadata (
        Id int GENERATED ALWAYS AS IDENTITY NOT NULL,
        Name          VARCHAR (50) NOT NULL,
        ApplicationId INT           NOT NULL,
        Properties    text         NOT NULL
	); 

CREATE TABLE IF NOT EXISTS  ErrorOrigins (
        Id int GENERATED ALWAYS AS IDENTITY NOT NULL,
        IpAddress      VARCHAR (20)   NOT NULL,
        CountryCode    VARCHAR (5)    NULL,
        CountryName    VARCHAR (30)   NULL,
        RegionCode     VARCHAR (5)    NULL,
        RegionName     VARCHAR (30)   NULL,
        City           VARCHAR (30)  NULL,
        ZipCode        VARCHAR (10)   NULL,
        Latitude       DECIMAL (9, 6) NOT NULL,
        Longitude      DECIMAL (9, 6) NOT NULL,
        CreatedAtUtc   timestamp        NOT NULL
	); 
CREATE TABLE IF NOT EXISTS  ErrorReportOrigins (
        ErrorOriginId             INT NOT NULL,
        IncidentId             INT NOT NULL,
        ReportId    INT  NOT NULL,
        ApplicationId    INT  NOT NULL,
        CreatedAtUtc   timestamp        NOT NULL
	); 


CREATE TABLE IF NOT EXISTS  ErrorReports (
        Id int GENERATED ALWAYS AS IDENTITY NOT NULL,
        IncidentId         INT            NOT NULL,
        ErrorId            VARCHAR (36)   NOT NULL,
        ApplicationId      INT            NOT NULL,
        ReportHashCode     VARCHAR (20)   NOT NULL,
        CreatedAtUtc       timestamp        NOT NULL,
        SolvedAtUtc        timestamp        NULL,
        Title						     VARCHAR(100)  NULL,
        RemoteAddress      VARCHAR (45)   NULL, --SEE http://stackoverflow.com/questions/166132/maximum-length-of-the-textual-representation-of-an-ipv6-address
        Exception          TEXT          NOT NULL,
        ContextInfo        TEXT          NOT NULL
	); 

CREATE TABLE IF NOT EXISTS  IncidentFeedback (
        Id int GENERATED ALWAYS AS IDENTITY NOT NULL,
        ApplicationId      INT            NULL,
        IncidentId         INT            NULL,
        ReportId           INT            NULL,
        CreatedAtUtc       timestamp        NOT NULL,
        RemoteAddress      VARCHAR (20)   NOT NULL,
        Description        TEXT          NOT NULL,
        EmailAddress       VARCHAR (512) NULL,
        Conversation       TEXT          NOT NULL,
        ConversationLength INT            NOT NULL,
        ErrorReportId      VARCHAR (40)   NOT NULL,
        Replied            INT NOT NULL default 0
	); 


CREATE TABLE IF NOT EXISTS  Incidents (
		Id int GENERATED ALWAYS AS IDENTITY NOT NULL,
		ReportHashCode          VARCHAR (20)   NOT NULL,
		ApplicationId           INT            NOT NULL,
		CreatedAtUtc            timestamp        NOT NULL,
		HashCodeIdentifier      VARCHAR (1024) NOT NULL,
		ReportCount             INT            NOT NULL,
		UpdatedAtUtc            timestamp        NULL,
		Description             TEXT          NOT NULL,
		FullName                VARCHAR (255) NOT NULL,
		Solution                TEXT          NULL,
		IsSolved                BYTEA     NOT NULL ,
		IsSolutionShared        BYTEA     NOT NULL ,
		SolvedAtUtc             timestamp        NULL,
		StackTrace              TEXT          NULL,
		IsReOpened              BIT            NOT NULL ,
		ReOpenedAtUtc           timestamp        NULL,
		PreviousSolutionAtUtc   timestamp        NULL,
		IgnoreReports           BIT            NOT NULL ,
		IgnoringReportsSinceUtc timestamp        NULL,
		IgnoringRequestedBy     VARCHAR (50)  NULL,
		LastSolutionAtUtc       timestamp        NULL
	); 

CREATE TABLE IF NOT EXISTS  IncidentTags (
        Id int GENERATED ALWAYS AS IDENTITY NOT NULL,
        IncidentId  INT          NOT NULL,
        TagName     VARCHAR (40) NOT NULL,
        OrderNumber INT          NOT NULL
	); 

CREATE TABLE IF NOT EXISTS  ReportContextInfo (
        Id int GENERATED ALWAYS AS IDENTITY NOT NULL,
        IncidentId   INT             NOT NULL,
        ReportId     INT             NOT NULL,
        CreatedAtUtc timestamp         NOT NULL,
        UpdatedAtUtc timestamp         NULL,
        Name         VARCHAR (1024) NULL,
        Value        VARCHAR (20)   NULL,
        LargeValue   TEXT           NOT NULL
	); 

CREATE TABLE IF NOT EXISTS  IncidentContextCollections (
        Id int GENERATED ALWAYS AS IDENTITY NOT NULL,
        IncidentId              INT          NOT NULL,
        Name                    VARCHAR (250) NOT NULL,
        Properties              text NOT NULL
	); 
 

CREATE TABLE IF NOT EXISTS  Triggers (
        Id int GENERATED ALWAYS AS IDENTITY NOT NULL,
        Name                    VARCHAR (50)  NOT NULL,
        Description             VARCHAR (512) NOT NULL,
        ApplicationId           INT            NOT NULL,
        Rules                   TEXT          NOT NULL,
        Actions                 TEXT          NOT NULL,
        LastTriggerAction       VARCHAR (50)  NOT NULL,
        RunForNewIncidents      BIT            NOT NULL,
        RunForExistingIncidents BIT            NOT NULL,
        RunForReOpenedIncidents BIT            NOT NULL
	); 


CREATE TABLE IF NOT EXISTS  UserNotificationSettings (
		AccountId        INT          NOT NULL,
		ApplicationId    INT          NOT NULL,
		NewIncident      VARCHAR (20) NOT NULL default 'Disabled',
		NewReport        VARCHAR (20) NOT NULL default 'Disabled',
		ReOpenedIncident VARCHAR (20) NOT NULL default 'Disabled',
		WeeklySummary    VARCHAR (20) NOT NULL default 'Disabled',
		ApplicationSpike VARCHAR (20) NOT NULL default 'Disabled',
		UserFeedback     VARCHAR (20) NOT NULL default 'Disabled'
	); 

ALTER TABLE UserNotificationSettings
        ADD CONSTRAINT pk_UserNotificationSettings PRIMARY KEY (AccountId, ApplicationId);

CREATE TABLE IF NOT EXISTS  Users
(
        AccountId				INT NOT NULL primary key,
        EmailAddress		varchar(255) not null,
        FirstName				varchar(100),
        LastName				varchar(100),
        UserName				varchar(100),
		MobileNumber		varchar(100)
	);


CREATE TABLE IF NOT EXISTS  ApplicationMembers ( 
        AccountId     INT           NULL,
        ApplicationId INT           NOT NULL,
		EmailAddress  varchar(255) not null,
        AddedAtUtc    timestamp       NOT NULL,
        AddedByName   VARCHAR (50)  NOT NULL,
        Roles         VARCHAR (255) NOT NULL
	); 

--ALTER TABLE ApplicationMembers  ADD CONSTRAINT constraint_name FOREIGN KEY (AccountId) REFERENCES Accounts (Id);
--ALTER TABLE ApplicationMembers  ADD CONSTRAINT constraint_name FOREIGN KEY (ApplicationId) REFERENCES Applications (Id);

CREATE TABLE IF NOT EXISTS  QueueEvents (
        Id int GENERATED ALWAYS AS IDENTITY NOT NULL,
		ApplicationId INT NOT NULL, 
        CreatedAtUtc    timestamp       NOT NULL,
        AssemblyQualifiedTypeName   VARCHAR (255)  NOT NULL,
        Body         text NOT NULL
	); 

CREATE TABLE IF NOT EXISTS  QueueReports (
        Id int GENERATED ALWAYS AS IDENTITY NOT NULL,
		ApplicationId INT NOT NULL, 
        CreatedAtUtc    timestamp       NOT NULL,
        AssemblyQualifiedTypeName   VARCHAR (255)  NOT NULL,
        Body         text NOT NULL
		
	); 

CREATE TABLE IF NOT EXISTS  QueueFeedback (
        Id int GENERATED ALWAYS AS IDENTITY NOT NULL,
		ApplicationId INT NOT NULL, 
        CreatedAtUtc    timestamp       NOT NULL,
        AssemblyQualifiedTypeName   VARCHAR (255)  NOT NULL,
        Body         text NOT NULL
		
	); 