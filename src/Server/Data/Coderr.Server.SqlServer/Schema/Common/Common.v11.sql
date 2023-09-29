alter table PartitionDefinitions add CriticalThreshold int;
alter table PartitionDefinitions add ImportantThreshold int;

create table PartitionTaggedIncidents
(
  IncidentId int not null,
  PartitionId int not null,
  IsCritical bit not null,
  IsImportant bit not null
);

alter table UserNotificationSettings add CriticalIncident varchar(40) not null default 'Disabled';
alter table UserNotificationSettings add ImportantIncident varchar(40) not null default 'Disabled';
