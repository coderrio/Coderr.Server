using System.Threading.Tasks;
using Coderr.Server.Api.Core.Settings.Commands;
using DotNetCqs;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Core.Settings
{
    class SaveAccountSettingsHandler : IMessageHandler<SaveAccountSettings>
    {
        private IAdoNetUnitOfWork _unitOfWork;

        public SaveAccountSettingsHandler(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task HandleAsync(IMessageContext context, SaveAccountSettings message)
        {
            foreach (var setting in message.Settings)
            {
                await SaveSetting(message.AccountId, setting.Key, setting.Value);
            }
        }

        private async Task SaveSetting(int accountId, string name, string value)
        {
            var entity =
                await _unitOfWork.FirstOrDefaultAsync<AccountSetting>("AccountId  = @AccountId AND Name = @Name",
                    new { accountId, Name = name });
            if (entity != null)
            {
                entity.Value = value;
                await _unitOfWork.UpdateAsync(entity);
            }
            else
            {
                await _unitOfWork.InsertAsync(new AccountSetting
                {
                    AccountId = accountId,
                    Name = name,
                    Value = value
                });
            }
        }
    }
}
