using System.Collections.Generic;
using Griffin.Data.Mapper;
using OneTrueError.App.Core.Invitations;
using OneTrueError.SqlServer.Tools;

namespace OneTrueError.SqlServer.Core.Invitations
{
    public class InvitationMapping : CrudEntityMapper<Invitation>
    {
        public InvitationMapping()
            : base("Invitations")
        {
            Property(x => x.Id).PrimaryKey(true);
            Property(x => x.EmailToInvitedUser).ColumnName("Email");
            Property(x => x.Invitations)
                .ToColumnValue(EntitySerializer.Serialize)
                .ToPropertyValue(x => EntitySerializer.Deserialize<IList<ApplicationInvitation>>((string) x));
        }
    }
}