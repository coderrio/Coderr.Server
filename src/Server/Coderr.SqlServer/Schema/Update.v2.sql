IF OBJECT_ID(N'dbo.[DatabaseSchema]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[DatabaseSchema] (
        [Version] int not null default 1
	);
	INSERT INTO DatabaseSchema VALUES(1);
END

IF OBJECT_ID(N'dbo.[ApiKeys]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[ApiKeys] (
        [Id]     INT identity      not    NULL primary key,
		[ApplicationName] varchar(40) NOT NULL, 
        [CreatedAtUtc]    DATETIME      NOT NULL,
        [CreatedById]   int NOT NULL,
        [GeneratedKey]   varchar(36) NOT NULL,
        [SharedSecret]   varchar(36) NOT NULL
	);
	CREATE TABLE [dbo].[ApiKeyApplications] (
        [ApiKeyId]     INT not    NULL,
		[ApplicationId] INT NOT NULL,
		Primary key (ApiKeyId, ApplicationId),
		FOREIGN KEY (ApiKeyId) REFERENCES ApiKeys(Id) ON DELETE CASCADE,
		FOREIGN KEY (ApplicationId) REFERENCES Applications(Id) ON DELETE NO ACTION
	);
	update DatabaseSchema SET Version = 2;
END
