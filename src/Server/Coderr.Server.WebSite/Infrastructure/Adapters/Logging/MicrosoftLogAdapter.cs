using System;
using System.Diagnostics;
using log4net;
using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Coderr.Server.WebSite.Infrastructure.Adapters.Logging
{
    public class MicrosoftLogAdapter : ILogger
    {
        private ILog _logger;

        public MicrosoftLogAdapter(ILog logger)
        {
            _logger = logger;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                    _logger.Debug(formatter(state, exception));
                    break;
                case LogLevel.Information:
                    _logger.Info(formatter(state, exception));
                    break;
                case LogLevel.Warning:
                    _logger.Warn(formatter(state, exception));
                    break;
                case LogLevel.Error:
                case LogLevel.Critical:
                    _logger.Error(formatter(state, exception));
                    break;
                default:
                    Debugger.Break();
                    break;
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new FakeLogScope(state);
        }
    }
}
