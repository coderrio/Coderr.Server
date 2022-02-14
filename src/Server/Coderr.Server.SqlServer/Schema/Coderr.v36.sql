-- need to check if it's been part of commercial editions
if not exists (select * from sysobjects where name='ErrorReportLogs' and xtype='U')
begin

CREATE Table dbo.ErrorReportLogs
(
    Id int not null identity primary key,
    ReportId int not null constraint FK_ErrorReportLogs_ErrorReports foreign key references ErrorReports(id) on delete cascade,
    IncidentId int not null constraint FK_ErrorReportLogs_Incidents foreign key references Incidents(id),
    Json varchar(max) not null
);



end
