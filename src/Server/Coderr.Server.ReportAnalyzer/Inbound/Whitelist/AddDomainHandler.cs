using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Coderr.Server.ReportAnalyzer.Abstractions.Inbound.Whitelists.Commands;
using DotNetCqs;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.ReportAnalyzer.Inbound.Whitelist
{
    class AddDomainHandler : IMessageHandler<AddDomain>
    {
        IAdoNetUnitOfWork _uow;

        public AddDomainHandler(IAdoNetUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task HandleAsync(IMessageContext context, AddDomain message)
        {
            await _uow.InsertAsync(new WhitelistedDomain(message.ApplicationId, message.DomainName));
        }
    }
}
