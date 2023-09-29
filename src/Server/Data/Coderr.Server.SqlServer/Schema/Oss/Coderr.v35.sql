create table UserSettings
(
    AccountId int not null primary key,
    Name varchar(40)  not null,
    Value varchar(max) not null
);
