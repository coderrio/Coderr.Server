alter table Applications add RetentionDays int not null default 60;
alter table Environments add StoreReports bit not null default 1;
create table ApplicationEnvironments
(
    Id int not null identity primary key,
    ApplicationId int not null constraint FK_ApplicationEnvironments_Applications foreign key references Applications(Id),
    EnvironmentId int not null,
    DeleteIncidents bit not null default 0
);
