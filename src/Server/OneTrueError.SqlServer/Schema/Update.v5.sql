--version 1.0 of OneTrueError
--a split was required due to updating created column
UPDATE Accounts SET IsSysAdmin = 1 WHERE Id = (SELECT TOP 1 Id FROM ACCOUNTS ORDER BY Id);

UPDATE DatabaseSchema SET Version = 5;
