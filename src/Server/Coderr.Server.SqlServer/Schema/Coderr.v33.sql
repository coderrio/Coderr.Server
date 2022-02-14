create table InsightDefinitions
(
    Id int not null identity primary key,
    Name varchar(40) not null,
    DisplayName varchar(40) not null,
    Description varchar(2500) not null
);

create table dbo.InsightsConfigurations
(
    Id int not null identity primary key,
    DefinitionId int not null constraint FK_InsightsConfigurations_InsightDefinitions foreign key references InsightDefinitions(Id),
    ApplicationId int not null constraint FK_InsightsConfigurations_Applications foreign key references Applications(Id) on delete CASCADE,
    AccountId int not null,
    FromDays int not null,
    ToDays int not null
);
