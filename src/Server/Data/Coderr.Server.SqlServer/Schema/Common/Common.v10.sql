create table AzureDevOpsSettings
(
    ApplicationId int not null primary key,
    CreatedById int not null constraint FK_AzureDevOpsSettings_Users foreign key references Users(AccountId),
    Url varchar(60) not null,
    PersonalAccessToken varchar(60) not null,
    ProjectId varchar(40),
    ProjectName varchar(40),
    AreaPath varchar(255),
    AreaPathId varchar(40),
    IterationId varchar(40),
    IterationName varchar(255)
);

create table WorkItemMapping
(
    IncidentId int not null primary key,
    ApplicationId int not null,
    WorkItemId varchar(60) not null,
    LastSynchronizationAtUtc datetime not null,
    Name varchar(255) not null,
    ApiUrl varchar(255) not null,
    UiUrl varchar(255) not null,
    IntegrationName varchar(40) not null
);

create table WorkItemIntegrationMapping
(
    ApplicationId int not null primary key,
    IntegrationName varchar(40) not null
);
