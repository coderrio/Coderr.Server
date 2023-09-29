using System.Threading.Tasks;
using Coderr.Server.Api.Modules.Whitelists.Commands;
using DotNetCqs;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Modules.Whitelists
{
    internal class RemoveDomainHandler : IMessageHandler<RemoveEntry>
    {
        private readonly IAdoNetUnitOfWork _uow;

        public RemoveDomainHandler(IAdoNetUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task HandleAsync(IMessageContext context, RemoveEntry message)
        {
            var item = await _uow.FirstOrDefaultAsync<App.Modules.Whitelists.Whitelist>("Id = @id", new {message.Id});
            if (item == null)
                return;

            await _uow.DeleteAsync(item);
        }
    }
}