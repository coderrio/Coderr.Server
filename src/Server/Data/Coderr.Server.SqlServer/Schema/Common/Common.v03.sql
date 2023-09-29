if not exists (select * from sysobjects where name='ApplicationGroups' and xtype='U')
begin

    CREATE TABLE dbo.ApplicationGroups
    (
        Id int not null identity primary key,
        Name varchar(50) not null
    );

    CREATE Table dbo.ApplicationGroupMap
    (
        Id int not null identity primary key,
        ApplicationId int not null constraint FK_ApplicationGroupMap_Application foreign key references Applications(id) on delete cascade,
        ApplicationGroupId int not null constraint FK_ApplicationGroupMap_ApplicationGroup foreign key references ApplicationGroups(id),
    );

    insert into ApplicationGroups (Name) VALUES('General');
    insert into ApplicationGroupMap (ApplicationId, ApplicationGroupId)
    SELECT Id, 1
    FROM Applications;
end
