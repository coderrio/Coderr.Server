using System;
using log4net;
using Microsoft.Extensions.Logging;

namespace Coderr.Server.WebSite.Infrastructure.Adapters.Logging
{
    public class MicrosoftLogFactoryAdapter : ILoggerFactory
    {
        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            var logger = LogManager.GetLogger(typeof(MicrosoftLogAdapter));
            return new MicrosoftLogAdapter(logger);
        }

        public void AddProvider(ILoggerProvider provider)
        {
            
        }
    }
}