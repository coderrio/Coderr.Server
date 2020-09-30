create table dbo.CorrelationIds
(
    Id int identity not null primary key,
    Value varchar(40) not null
);

create table dbo.IncidentCorrelations
(
    Id int identity not null primary key,
    CorrelationId int not null constraint FK_IncidentCorrelations_CorrelationIds foreign key references CorrelationIds(Id),
    IncidentId int not null constraint FK_IncidentCorrelations_Incidents foreign key references Incidents(Id) ON DELETE CASCADE
);

create unique index IDX_IncidentCorrelations_Pair ON IncidentCorrelations(CorrelationId, IncidentId);
