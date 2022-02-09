using System.Threading.Tasks;
using Coderr.Server.Api.Core.Settings.Commands;
using Coderr.Server.Api.Core.Settings.Queries;
using DotNetCqs;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Core.Settings
{
    internal class SaveAccountSettingHandler : IMessageHandler<SaveAccountSetting>
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public SaveAccountSettingHandler(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task HandleAsync(IMessageContext context, SaveAccountSetting message)
        {
            var setting =
                await _unitOfWork.FirstOrDefaultAsync<AccountSetting>("AccountId  = @AccountId AND Name = @Name", message);
            if (setting != null)
            {
                setting.Value = message.Value;
                await _unitOfWork.UpdateAsync(setting);
            }
            else
            {
                await _unitOfWork.InsertAsync(new AccountSetting
                {
                    AccountId = message.AccountId, Name = message.Name, Value = message.Value
                });
            }
        }
    }
}