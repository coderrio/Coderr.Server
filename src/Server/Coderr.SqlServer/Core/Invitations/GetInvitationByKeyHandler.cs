using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using Griffin.Data;
using codeRR.Api.Core.Invitations.Queries;

namespace codeRR.SqlServer.Core.Invitations
{
    [Component]
    internal class GetInvitationByKeyHandler : IQueryHandler<GetInvitationByKey, GetInvitationByKeyResult>
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public GetInvitationByKeyHandler(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GetInvitationByKeyResult> ExecuteAsync(GetInvitationByKey query)
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