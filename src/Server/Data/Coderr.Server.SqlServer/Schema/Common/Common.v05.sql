ALTER TABLE IncidentPartitionValues ADD ReceivedAtUtc datetime;
GO
UPDATE IncidentPartitionValues
SET ReceivedAtUtc = i.CreatedAtUtc
FROM IncidentPartitionValues ipv
JOIN Incidents i ON (i.Id = ipv.IncidentId)

