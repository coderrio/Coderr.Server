CREATE TABLE IF NOT EXISTS IncidentEnvironments
(
  	Id int GENERATED ALWAYS AS IDENTITY NOT NULL,
    IncidentId int not null  ,
    EnvironmentName varchar(50) not null
);

CREATE TABLE IF NOT EXISTS IncidentHistory
(
   	Id int GENERATED ALWAYS AS IDENTITY NOT NULL,
    IncidentId int not null  ,
    CreatedAtUtc timestamp not null,
    AccountId int NULL, -- for system entries
    State int not null,
    ApplicationVersion varchar(40) NULL -- for action where version is not related to the action
);
alter table Incidents add IgnoredUntilVersion varchar(20) null;
--CREATE NONCLUSTERED INDEX IX_IncidentHistory_Incidents ON dbo.IncidentHistory (IncidentId);
