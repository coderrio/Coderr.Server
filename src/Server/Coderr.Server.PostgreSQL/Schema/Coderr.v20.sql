CREATE TABLE IF NOT EXISTS CorrelationIds
(
 	Id int GENERATED ALWAYS AS IDENTITY NOT NULL,
    Value varchar(40) not null
);

CREATE TABLE IF NOT EXISTS IncidentCorrelations
(
  	Id int GENERATED ALWAYS AS IDENTITY NOT NULL,
    CorrelationId int not null  ,
    IncidentId int not null 
);

--create unique index IDX_IncidentCorrelations_Pair ON IncidentCorrelations(CorrelationId, IncidentId);
