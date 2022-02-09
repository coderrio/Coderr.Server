alter table IncidentFeedback alter column RemoteAddress varchar(45) not null;
alter table ErrorOrigins alter column IpAddress varchar(45) null;
alter table WhitelistedDomainIps alter column IpAddress varchar(45) not null;
