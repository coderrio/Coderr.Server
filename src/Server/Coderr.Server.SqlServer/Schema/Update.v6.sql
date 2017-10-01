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

create table IncidentVersions
(
	IncidentId int not null constraint FK_IncidentVersions_Incidents references Incidents(Id),
	VersionId int not null constraint FK_IncidentVersions_ApplicationVersions references ApplicationVersions(Id)
);

IF COL_LENGTH('dbo.Incidents', 'StackTrace') IS NULL
BEGIN
ALTER TABLE Incidents ADD StackTrace varchar(MAX);

UPDATE Incidents
	SET StackTrace = (
	SELECT TOP (1) Substring(Exception, 
					CHARINDEX('"StackTrace":"', Exception) + 14, 
					DATALENGTH(exception)-CHARINDEX('"StackTrace":"', Exception) - 14 - 1)
	FROM ErrorReports
	WHERE IncidentId = Incidents.Id)

END


UPDATE DatabaseSchema SET Version = 6;
