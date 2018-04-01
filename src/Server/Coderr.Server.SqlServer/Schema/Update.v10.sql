create table IncidentEnvironments
(
    Id int not null identity primary key,
    IncidentId int not null constraint FK_IncidentEnvironment_Incidents REFERENCES Incidents(Id) ON DELETE CASCADE,
    EnvironmentName varchar(50) not null
);

create table IncidentHistory
(
    Id int not null identity primary key,
    IncidentId int not null constraint FK_IncidentEnvironment_Incidents REFERENCES Incidents(Id) ON DELETE CASCADE,
    CreatedAtUtc datetime not null,
    AccountId int NULL, -- for system entries
    State int not null,
    ApplicationVersion varchar(40) NULL -- for action where version is not related to the action
);
alter table Incidents add IgnoredUntilVersion varchar(20) null;
CREATE NONCLUSTERED INDEX IX_IncidentHistory_Incidents ON dbo.IncidentHistory (IncidentId);


UPDATE DatabaseSchema SET Version = 10;