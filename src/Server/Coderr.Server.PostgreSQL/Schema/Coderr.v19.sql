CREATE TABLE IF NOT EXISTS IncidentReports
(
       	Id int GENERATED ALWAYS AS IDENTITY NOT NULL,
    IncidentId int not null  ,
    ReceivedAtUtc timestamp not null,
    ErrorId varchar(40) not null
);
--create index IDX_IncidentReports_IncidentId ON IncidentReports (IncidentId, ReceivedAtUtc);

insert into IncidentReports (IncidentId, ReceivedAtUtc, ErrorId)
SELECT IncidentId, CreatedAtUtc, ErrorId
FROM ErrorReports;
