create table WhitelistedDomains
(
	Id int not null identity primary key,
	DomainName varchar(255) not null
);

create table WhitelistedDomainApps
(
	Id int not null identity primary key,
	DomainId int not null constraint FK_WhitelistedDomainApps_WhitelistedDomains foreign key references WhitelistedDomains(Id) ON DELETE CASCADE,
	ApplicationId int not null constraint FK_WhitelistedDomainApps_Applications foreign key references Applications(Id) ON DELETE CASCADE,
);

create table WhitelistedDomainIps
(
	Id int not null identity primary key,
	DomainId int not null constraint FK_WhitelistedDomainIps_WhitelistedDomains foreign key references WhitelistedDomains(Id) ON DELETE CASCADE,
	IpAddress varchar(36) not null,
	IpType int not null,
    StoredAtUtc datetime not null
);
