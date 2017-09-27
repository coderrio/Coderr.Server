using System.Collections.Generic;
using codeRR.Server.App.Core.Invitations;
using codeRR.Server.SqlServer.Tools;
using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Core.Invitations
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