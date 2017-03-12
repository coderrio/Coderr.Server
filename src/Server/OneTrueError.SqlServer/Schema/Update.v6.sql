create table ApplicationVersions
(
	Id int not null identity primary key,
	ApplicationId int not null foreign key references Applications (Id),
	ApplicationName varchar(40) not null,
	FirstReportDate datetime not null,
	LastReportDate datetime not null,
	Version varchar(10) not null
);

create table ApplicationVersionMonths
(
	Id int not null identity primary key,
	VersionId int not null foreign key references ApplicationVersions (Id),
	YearMonth date not null,
	IncidentCount int not null,
	ReportCount int not null,
	LastUpdateAtUtc datetime not null
);

create table ApplicationVersionConfig
(
	Id int not null identity primary key,
	ApplicationId int not null,
	AssemblyName varchar(255) not null
);


UPDATE DatabaseSchema SET Version = 6;
