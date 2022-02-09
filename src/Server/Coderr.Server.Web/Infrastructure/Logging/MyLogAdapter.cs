using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using log4net.Core;
using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Coderr.Server.Web.Infrastructure.Logging
{
    public class MyLogFactoryAdapter : ILoggerFactory
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public ILogger CreateLogger(string categoryName)
        {
            var logger = LogManager.GetLogger(typeof(MyLogAdapter));
            return new MyLogAdapter(logger);
        }

        public void AddProvider(ILoggerProvider provider)
        {
            
        }
    }

    public class MyLogAdapter : ILogger
    {
        private ILog _logger;

        public MyLogAdapter(ILog logger)
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

    public class FakeLogScope : IDisposable
    {
        private readonly object _state;

        public FakeLogScope(object state)
        {
            _state = state;
        }

        public void Dispose()
        {
        }
    }
}
