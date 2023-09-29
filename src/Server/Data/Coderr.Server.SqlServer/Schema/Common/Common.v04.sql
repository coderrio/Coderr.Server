ALTER TABLE IncidentPartitionValues ADD ValueId int constraint FK_IncidentPartitionValues_ApplicationPartitionValues references ApplicationPartitionValues(Id);
GO
UPDATE IncidentPartitionValues
SET ValueId = apv.Id
FROM IncidentPartitionValues ipv
JOIN ApplicationPartitionValues apv ON (apv.PartitionId = ipv.PartitionId)
WHERE ipv.Value = apv.Value
