alter table ErrorOrigins add IsLookedUp bit not null default(0);
go
update ErrorOrigins SET IsLookedUp = 1 WHERE CountryCode IS NOT NULL;
