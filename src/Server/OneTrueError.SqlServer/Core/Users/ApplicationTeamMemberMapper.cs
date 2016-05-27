using System;
using Griffin.Data.Mapper;
using OneTrueError.App.Core.Users;

namespace OneTrueError.SqlServer.Core.Users
{
    public class ApplicationTeamMemberMapper : CrudEntityMapper<ApplicationTeamMember>
    {
        public ApplicationTeamMemberMapper() : base("ApplicationMembers")
        {
            /* [AccountId]     INT           NULL foreign key references Accounts (Id),
    [ApplicationId] INT           NOT NULL foreign key references Applications (Id),
	[EmailAddress]  nvarchar(255) not null,
    [AddedAtUtc]    DATETIME      NOT NULL,
    [AddedByName]   VARCHAR (50)  NOT NULL,
    [Roles]         VARCHAR (255) NOT NULL,*/
            Property(x => x.AccountId)
                .ToColumnValue(x => x == 0 ? (object) DBNull.Value : x)
                .ToPropertyValue(x => x is DBNull ? 0 : (int) x);

            Property(x => x.UserName).NotForCrud();
            Property(x => x.Roles)
                .ToColumnValue(x => string.Join(",", x))
                .ToPropertyValue(x => ((string) x).Split(','));
        }
    }
}