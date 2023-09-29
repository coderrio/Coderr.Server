using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coderr.Server.Common.AzureDevOps.App.Connections
{
    public interface ISettingsRepository
    {
        Task<Settings> Get(int applicationId);
        Task<List<Settings>> GetAll();
        Task Save(Settings settings);
    }
}
