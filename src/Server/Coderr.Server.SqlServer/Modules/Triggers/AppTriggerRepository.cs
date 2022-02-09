using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.App.Modules.Triggers;

namespace Coderr.Server.SqlServer.Modules.Triggers
{
    [ContainerService]
    class AppTriggerRepository : ITriggerRepository
    {
        public Task CreateAsync(Trigger trigger)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Trigger> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Trigger> GetForApplication(int applicationId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Trigger entity)
        {
            throw new NotImplementedException();
        }
    }
}
