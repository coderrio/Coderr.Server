using System.Collections.Generic;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Abstractions.Directory;
using log4net;

namespace Coderr.Server.App.ActiveDirectory
{
    [ContainerService]
    internal class AzureDirectoryService : IDirectoryService
    {
        private ILog _logger = LogManager.GetLogger(typeof(AzureDirectoryService));

        public bool IsInDomain { get; } = false;

        public IEnumerable<IDirectoryEntry> Find(string text, bool groups, bool users, int pageNumber = 1,
            int pageSize = 20)
        {
            return new IDirectoryEntry[0];
        }

        public string FindEmail(string samAccountName)
        {
            return null;
        }

        public IDirectoryEntry GetBySid(string sid)
        {
            return null;
        }
    }
}