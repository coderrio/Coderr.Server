create table ApplicationEnvironments
(
    Id int identity not null primary key,
    ApplicationId int not null constraint FK_ApplicationEnvironment_Application foreign key  references Applications(Id) ON DELETE CASCADE,
    Name varchar(100) not null,
);

drop table IncidentEnvironments;
CREATE TABLE IncidentEnvironments
(
    IncidentId int not null constraint FK_IncidentEnvironment_Incident foreign key references Incidents(Id) ON DELETE CASCADE,
    ApplicationEnvironmentId int not null constraint FK_IncidentEnvironment_ApplicationEnvironment foreign key  references ApplicationEnvironments(Id) ,
);
