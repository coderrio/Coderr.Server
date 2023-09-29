IF COL_LENGTH('dbo.IncidentPartitionValues', 'Value') IS NOT NULL
BEGIN
    ALTER TABLE IncidentPartitionValues DROP COLUMN Value;
END
