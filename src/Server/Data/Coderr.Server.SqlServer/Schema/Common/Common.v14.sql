create table WorkItemUserMappings
(
    AccountId int not null primary key,
    ExternalId varchar(255) not null,
    AdditionalData varchar(2000)
);
