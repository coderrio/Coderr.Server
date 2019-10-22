using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.App.Core.Invitations;
using Coderr.Server.ReportAnalyzer.Abstractions;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.PostgreSQL.Core.Invitations
{
    [ContainerService]
    internal class InvitationRepository : IInvitationRepository
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public InvitationRepository(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task DeleteAsync(string invitationKey)
        {
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = "DELETE FROM Invitations WHERE InvitationKey = @key;";
                cmd.AddParameter("key", invitationKey);
                await cmd.ExecuteNonQueryAsync();
            }
        }


        public async Task<Invitation> FindByEmailAsync(string email)
        {
            return await _unitOfWork.FirstOrDefaultAsync<Invitation>(new {EmailToInvitedUser = email});
        }

        public async Task CreateAsync(Invitation invitation)
        {
            await _unitOfWork.InsertAsync(invitation);
        }

        public async Task UpdateAsync(Invitation invitation)
        {
            await _unitOfWork.UpdateAsync(invitation);
        }

        public async Task<Invitation> GetByInvitationKeyAsync(string invitationKey)
        {
            return await _unitOfWork.FirstOrDefaultAsync<Invitation>(new {InvitationKey = invitationKey});
        }
    }
}