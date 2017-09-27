using System;
using codeRR.Server.App.Core.Users;
using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Core.Users
{
    public class ApplicationTeamMemberMapper : CrudEntityMapper<ApplicationTeamMember>
    {
        public ApplicationTeamMemberMapper() : base("ApplicationMembers")
        {
            Property(x => x.Id)
                .PrimaryKey(true);

            Property(x => x.AccountId)
                .ToColumnValue(x => x == 0 ? (object)DBNull.Value : x)
                .ToPropertyValue(x => x is DBNull ? 0 : (int)x);

            Property(x => x.UserName)
                .NotForCrud();

            Property(x => x.Roles)
                .ToColumnValue(x => string.Join(",", x))
                .ToPropertyValue(x => ((string)x).Split(','));
        }
    }
}