CREATE TABLE IncidentReports
(
    Id int not null identity primary key,
    IncidentId int not null constraint FK_IncidentReports_Incidents foreign key references Incidents(Id) on delete cascade,
    ReceivedAtUtc datetime not null,
    ErrorId varchar(40) not null
);
create index IDX_IncidentReports_IncidentId ON IncidentReports (IncidentId, ReceivedAtUtc);

insert into IncidentReports (IncidentId, ReceivedAtUtc, ErrorId)
SELECT IncidentId, CreatedAtUtc, ErrorId
FROM ErrorReports;
