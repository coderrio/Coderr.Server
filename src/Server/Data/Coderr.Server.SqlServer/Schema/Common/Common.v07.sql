alter table CommonIncidentProgressTracking alter column CreatedAtUtc datetime not null;
alter table CommonIncidentProgressTracking alter column AssignedAtUtc datetime null;
alter table CommonIncidentProgressTracking alter column ClosedAtUtc datetime null;
alter table CommonIncidentProgressTracking alter column ReOpenedAtUtc datetime null;

create table dbo.ApplicationPartitionInsights
(
    Id int identity primary key not null,
	PartitionId int not null constraint FK_ApplicationPartitionInsights_PartitionDefinitions foreign key references PartitionDefinitions(Id) on delete cascade,
	Value int not null,
	YearMonth Date not null
);
