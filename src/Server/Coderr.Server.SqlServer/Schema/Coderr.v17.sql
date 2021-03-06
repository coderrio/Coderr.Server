﻿create table dbo.Environments
(
    Id int identity not null primary key,
    Name varchar(100) not null,
);

drop table dbo.IncidentEnvironments;
CREATE TABLE dbo.IncidentEnvironments
(
    IncidentId int not null constraint FK_IncidentEnvironment_Incident foreign key references Incidents(Id) ON DELETE CASCADE,
    EnvironmentId int not null constraint FK_IncidentEnvironment_Environments foreign key  references Environments(Id),
);
