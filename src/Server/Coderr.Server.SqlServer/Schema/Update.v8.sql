ALTER TABLE Incidents ADD State int not null default(0);
ALTER TABLE Incidents ADD AssignedToId int;
ALTER TABLE Incidents ADD AssignedAtUtc datetime;
ALTER TABLE Incidents ADD LastReportAtUtc datetime;

DECLARE @ConstraintName nvarchar(200)
SELECT @ConstraintName = d.Name 
	FROM SYS.DEFAULT_CONSTRAINTS d, sys.columns c
	WHERE c.name = 'IsSolved'
	AND c.object_id = OBJECT_ID(N'Incidents')
	AND d.PARENT_COLUMN_ID = c.column_id
EXEC('ALTER TABLE Incidents DROP CONSTRAINT ' + @ConstraintName)
alter table Incidents drop column IsSolved;

SELECT @ConstraintName = d.Name 
	FROM SYS.DEFAULT_CONSTRAINTS d, sys.columns c
	WHERE c.name = 'IgnoreReports'
	AND c.object_id = OBJECT_ID(N'Incidents')
	AND d.PARENT_COLUMN_ID = c.column_id
EXEC('ALTER TABLE Incidents DROP CONSTRAINT ' + @ConstraintName)
alter table Incidents drop column IgnoreReports;

-- ApiKey module deletes relations manually.
SELECT 
    @ConstraintName = KCU.CONSTRAINT_NAME
FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS AS RC 
INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KCU
    ON KCU.CONSTRAINT_CATALOG = RC.CONSTRAINT_CATALOG  
    AND KCU.CONSTRAINT_SCHEMA = RC.CONSTRAINT_SCHEMA 
    AND KCU.CONSTRAINT_NAME = RC.CONSTRAINT_NAME
WHERE
    KCU.TABLE_NAME = 'ApiKeyApplications' AND
    KCU.COLUMN_NAME = 'ApplicationId'
IF @ConstraintName IS NOT NULL EXEC('alter table ApiKeyApplications drop  CONSTRAINT ' + @ConstraintName)

UPDATE DatabaseSchema SET Version = 8;
