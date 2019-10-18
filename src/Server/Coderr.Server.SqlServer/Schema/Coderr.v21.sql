create table WhitelistedDomains
(
	Id int not null identity primary key,
	ApplicationId int not null constraint FK_WhitelistedDomains_Applications foreign key references Applications(Id) ON DELETE CASCADE,
	DomainName varchar(255) not null
);

create table WhitelistedDomainIps
(
	Id int not null identity primary key,
	DomainId int not null constraint FK_WhitelistedDomainIps_WhitelistedDomains foreign key references WhitelistedDomains(Id) ON DELETE CASCADE,
	IpAddress varchar(36) not null,
	IpType int not null
);
