CREATE TABLE IF NOT EXISTS ApiKeys (
      Id int GENERATED ALWAYS AS IDENTITY NOT NULL,
		ApplicationName varchar(40) NOT NULL, 
        CreatedAtUtc    timestamp      NOT NULL,
        CreatedById   int NOT NULL,
        GeneratedKey   varchar(36) NOT NULL,
        SharedSecret   varchar(36) NOT NULL
	);
CREATE TABLE IF NOT EXISTS ApiKeyApplications (
        ApiKeyId     INT not    NULL,
		ApplicationId INT NOT NULL,
		Primary key (ApiKeyId, ApplicationId) 
	);
