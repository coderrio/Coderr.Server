CREATE TABLE IF NOT EXISTS IgnoredReports
(
  	Id int GENERATED ALWAYS AS IDENTITY NOT NULL,
    NumberOfReports int not null,
    Date timestamp not null,
     NumberOfFtes decimal,
     EstimatedNumberOfErrors int, 
     MuteStatisticsQuestion BOOLEAN not null
);

 
