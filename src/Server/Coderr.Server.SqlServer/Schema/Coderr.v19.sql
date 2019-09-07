create table CorrelationIds
(
    Id int identity not null primary key,
    Value varchar(40) not null
);

create table IncidentCorrelations
(
    Id int identity not null primary key,
    CorrelationId int identity not null primary key
    IncidentId int not null
);

create table RelatedIncidents
(
    IncidentId int not null,
    
);