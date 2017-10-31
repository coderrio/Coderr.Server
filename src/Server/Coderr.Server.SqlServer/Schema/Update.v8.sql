ALTER TABLE Incidents ADD State int not null default(0);
ALTER TABLE Incidents ADD AssignedToId int;
ALTER TABLE Incidents ADD AssignedAtUtc datetime;
ALTER TABLE Incidents ADD LastReportAtUtc datetime;

DECLARE @ConstraintName nvarchar(200)
SELECT @ConstraintName = Name FROM SYS.DEFAULT_CONSTRAINTS
WHERE PARENT_OBJECT_ID = OBJECT_ID('incidents')
AND PARENT_COLUMN_ID = (SELECT column_id FROM sys.columns
                        WHERE NAME = N'IsSolved'
                        AND object_id = OBJECT_ID(N'Incidents'))
IF @ConstraintName IS NOT NULL
EXEC('ALTER TABLE incidents DROP CONSTRAINT ' + @ConstraintName)
alter table incidents drop column IsSolved;

SELECT @ConstraintName = Name FROM SYS.DEFAULT_CONSTRAINTS
WHERE PARENT_OBJECT_ID = OBJECT_ID('incidents')
AND PARENT_COLUMN_ID = (SELECT column_id FROM sys.columns
                        WHERE NAME = N'IgnoreReports'
                        AND object_id = OBJECT_ID(N'Incidents'))
IF @ConstraintName IS NOT NULL
EXEC('ALTER TABLE incidents DROP CONSTRAINT ' + @ConstraintName)
alter table incidents drop column IgnoreReports;

UPDATE DatabaseSchema SET Version = 8;
