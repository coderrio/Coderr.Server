CREATE TABLE IF NOT EXISTS ApplicationVersions
(
Id int GENERATED ALWAYS AS IDENTITY NOT NULL,
	ApplicationId int not null  ,
	ApplicationName varchar(40) not null,
	FirstReportDate timestamp not null,
	LastReportDate timestamp not null,
	Version varchar(10) not null
);

CREATE TABLE IF NOT EXISTS ApplicationVersionMonths
(
	Id int GENERATED ALWAYS AS IDENTITY NOT NULL,
	VersionId int not null  ,
	YearMonth timestamp not null,
	IncidentCount int not null,
	ReportCount int not null,
	LastUpdateAtUtc timestamp not null
);

CREATE TABLE IF NOT EXISTS IncidentVersions
(
	IncidentId int not null ,
	VersionId int not null 
);

--IF COL_LENGTH('dbo.Incidents', 'StackTrace') IS NULL
--BEGIN
--ALTER TABLE Incidents ADD StackTrace varchar(MAX);

--UPDATE Incidents
--	SET StackTrace = (
--	SELECT TOP (1) Substring(Exception, 
--					CHARINDEX('"StackTrace":"', Exception) + 14, 
--					DATALENGTH(exception)-CHARINDEX('"StackTrace":"', Exception) - 14 - 1)
--	FROM ErrorReports
--	WHERE IncidentId = Incidents.Id)

--END
