using System.Threading.Tasks;
using codeRR.Server.Api.Core.Invitations.Queries;
using DotNetCqs;
using Griffin.Container;
using Griffin.Data;

namespace codeRR.Server.SqlServer.Core.Invitations
{
    [Component]
    internal class GetInvitationByKeyHandler : IQueryHandler<GetInvitationByKey, GetInvitationByKeyResult>
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public GetInvitationByKeyHandler(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GetInvitationByKeyResult> HandleAsync(IMessageContext context, GetInvitationByKey query)
        {
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = "SELECT email FROM Invitations WHERE InvitationKey = @id";
                cmd.AddParameter("id", query.InvitationKey);
                return new GetInvitationByKeyResult {EmailAddress = (string) await cmd.ExecuteScalarAsync()};
            }
        }
    }
}