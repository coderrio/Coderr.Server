
create table dbo.NotificationsBrowser
(
    Id int not null identity primary key,
    AccountId int not null constraint FK_NotificationBrowser_AccountId foreign key references Accounts(Id),
    Endpoint varchar(255) not null,
    PublicKey varchar(150) not null,
    AuthenticationSecret varchar(150) not null,
    CreatedAtUtc datetime not null,
    ExpiresAtUtc datetime null
);
