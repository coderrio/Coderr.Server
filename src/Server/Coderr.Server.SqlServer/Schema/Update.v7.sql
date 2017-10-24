CREATE TABLE MessageQueue 
( 
    Id int not null identity primary key,
    QueueName varchar(40) not null,
    CreatedAtUtc datetime not null,
    MessageType varchar(512) not null,
    Body nvarchar(MAX) not null
);

DROP TABLE QueueEvents;
DROP TABLE QueueFeedback;
DROP TABLE QueueReports;

UPDATE DatabaseSchema SET Version = 7;
