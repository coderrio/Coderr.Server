CREATE TABLE IF NOT EXISTS ErrorReportSpikes
(
   	Id int GENERATED ALWAYS AS IDENTITY NOT NULL,
    ApplicationId int not null  ,
    SpikeDate timestamp not null,
    Count int not null,
    NotifiedAccounts varchar  not null
);
