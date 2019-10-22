CREATE TABLE IF NOT EXISTS Environments
(
   	Id int GENERATED ALWAYS AS IDENTITY NOT NULL,
    Name varchar(100) not null
);

CREATE TABLE IF NOT EXISTS IncidentEnvironments
(
    IncidentId int not null  ,
    EnvironmentId int not null 
);
