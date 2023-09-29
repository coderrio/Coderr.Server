IF OBJECT_ID(N'dbo.[Settings]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[Settings](
		[Section] [varchar](50) NOT NULL,
		[Name] [varchar](50) NOT NULL,
		[Value] [varchar](512),
	 ) ON [PRIMARY]
 END

CREATE TABLE PartitionDefinitions
(
	Id int not null identity primary key,
	ApplicationId int not null CONSTRAINT FK_PartitionDefinitions_Applications REFERENCES Applications (Id) ON DELETE CASCADE,
	Name varchar(40) not null,
	NumberOfItems int null,
	PartitionKey varchar(50) not null,
	Weight int not null default (1)
);

CREATE TABLE InboundPartitionValues
(
	Id int not null identity primary key,
	IncidentId int not null  CONSTRAINT FK_PartitionInboundValues_Incidents REFERENCES Incidents (Id) ON DELETE CASCADE,
	PartitionKey varchar(40) not null,
	Value varchar(40) not null,
);

CREATE TABLE IncidentPartitionValues
(
	Id int not null identity primary key,
	PartitionId int not null, --får felet multiple cascade path :( CONSTRAINT FK_IncidentPartitionValues_PartitionDefinitions REFERENCES PartitionDefinitions (Id) ON DELETE SET NULL,
	IncidentId int not null CONSTRAINT FK_IncidentPartitionValues_Incidents REFERENCES Incidents (Id) ON DELETE CASCADE,
	Value varchar(40) not null
);

CREATE TABLE ApplicationPartitionValues
(
	Id int not null identity primary key,
	PartitionId int not null CONSTRAINT FK_ApplicationPartitionValues_PartitionDefinitions REFERENCES PartitionDefinitions (Id) ON DELETE CASCADE,
	Value varchar(MAX) not null
);

