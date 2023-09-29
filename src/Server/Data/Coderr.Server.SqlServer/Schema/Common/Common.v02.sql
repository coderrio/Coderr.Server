IF OBJECT_ID(N'dbo.[CommonIncidentProgressTracking]', N'U') IS NULL
BEGIN

    CREATE TABLE dbo.CommonIncidentProgressTracking
    (
	    IncidentId int NOT NULL primary key,
	    ApplicationId int NOT NULL,
	    CreatedAtUtc date NOT NULL,
	    AssignedAtUtc date NULL,
	    AssignedToId int NULL,
	    ClosedById int NULL,
	    ClosedAtUtc date NULL,
	    ReOpenCount int NOT NULL default 0,
	    ReOpenedAtUtc date NULL,
	    VersionCount int default 1,
	    Versions varchar(255) null
    );

END;
