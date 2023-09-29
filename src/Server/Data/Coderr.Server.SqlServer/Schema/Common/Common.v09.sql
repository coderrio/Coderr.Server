ALTER TABLE IncidentPartitionValues DROP CONSTRAINT FK_IncidentPartitionValues_Incidents;
ALTER TABLE InboundPartitionValues DROP CONSTRAINT FK_PartitionInboundValues_Incidents;
alter table ApplicationPartitionInsights alter column Value varchar(max) not null;
alter table ErrorReportOrigins drop constraint FK_ErrorReportOrigins_Reports;
alter table InboundPartitionValues add ReceivedAtUtc datetime not null default GetUtcDate();
