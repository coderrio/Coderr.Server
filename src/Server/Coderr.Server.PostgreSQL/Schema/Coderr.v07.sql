CREATE TABLE IF NOT EXISTS  MessageQueue 
( 
    Id int GENERATED ALWAYS AS IDENTITY NOT NULL,
    QueueName varchar(40) not null,
    CreatedAtUtc timestamp not null,
    MessageType varchar(512) not null,
    Body varchar not null
);

DROP TABLE IF EXISTS QueueEvents;
DROP TABLE IF EXISTS QueueFeedback;
DROP TABLE IF EXISTS QueueReports;
