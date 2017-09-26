--version 1.0 (part A) of OneTrueError

ALTER TABLE Accounts ADD IsSysAdmin bit not null default 0;
alter table ApplicationMembers add Id int identity not null primary key;
ALTER TABLE ApplicationMembers ALTER COLUMN [EmailAddress]  nvarchar(255) null;


UPDATE DatabaseSchema SET Version = 4;
