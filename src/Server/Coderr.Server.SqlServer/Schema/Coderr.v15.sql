create Table dbo.IgnoredReports
(
    Id int not null identity primary key,
    NumberOfReports int not null,
    Date datetime not null
);

alter table Applications add NumberOfFtes decimal;
alter table Applications add [EstimatedNumberOfErrors] int;
alter table Applications add [MuteStatisticsQuestion] bit not null default 0;
