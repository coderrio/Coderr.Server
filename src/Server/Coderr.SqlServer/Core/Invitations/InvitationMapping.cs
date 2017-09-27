using System.Collections.Generic;
using Griffin.Data.Mapper;
using codeRR.App.Core.Invitations;
using codeRR.SqlServer.Tools;

namespace codeRR.SqlServer.Core.Invitations
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