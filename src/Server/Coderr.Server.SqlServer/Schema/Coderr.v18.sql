IF EXISTS (SELECT *  FROM sys.indexes  WHERE name='IDX_ErrorReportCollectionProperties_ReportId' 
AND object_id = OBJECT_ID('ErrorReportCollectionProperties'))
begin
	CREATE INDEX IDX_ErrorReportCollectionProperties_ReportId
	ON ErrorReportCollectionProperties (ReportId);

	CREATE INDEX IDX_ErrorReportCollectionInbound_ReportId
	ON ErrorReportCollectionInbound (ReportId);

end

