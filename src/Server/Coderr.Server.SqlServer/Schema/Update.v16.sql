alter table applications alter column NumberOfFtes decimal(4,1) null;

UPDATE DatabaseSchema SET Version = 16;
