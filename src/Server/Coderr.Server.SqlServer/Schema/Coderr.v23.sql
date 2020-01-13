ALTER TABLE Incidents ADD LastStoredReportUtc datetime;
go
UPDATE Incidents SET LastStoredReportUtc = LastReportAtUtc;
