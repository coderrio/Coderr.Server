using System;
using Griffin.Logging;
using log4net;

namespace codeRR.Server.Web.Infrastructure.Logging
{
    internal class LogAdapter : ILogger
    {
        private readonly ILog _log;

        public LogAdapter(ILog log)
        {
            _log = log;
        }

        public void Trace(string message, params object[] formatters)
        {
            _log.DebugFormat(message, formatters);
        }

        public void Trace(string message, Exception exception)
        {
            _log.Debug(message, exception);
        }

        public void Debug(string message, params object[] formatters)
        {
            _log.DebugFormat(message, formatters);
        }

        public void Debug(string message, Exception exception)
        {
            _log.Debug(message, exception);
        }

        public void Info(string message, params object[] formatters)
        {
            _log.InfoFormat(message, formatters);
        }

        public void Info(string message, Exception exception)
        {
            _log.Info(message, exception);
        }

        public void Warning(string message, params object[] formatters)
        {
            _log.WarnFormat(message, formatters);
        }

        public void Warning(string message, Exception exception)
        {
            _log.Warn(message, exception);
        }

        public void Error(string message, params object[] formatters)
        {
            _log.ErrorFormat(message, formatters);
        }

        public void Error(string message, Exception exception)
        {
            _log.Error(message, exception);
        }

        public void Write(LogEntry entry)
        {
            switch (entry.LogLevel)
            {
                case LogLevel.Debug:
                    _log.Debug(entry.Message, entry.Exception);
                    break;
                case LogLevel.Error:
                    _log.Error(entry.Message, entry.Exception);
                    break;
                case LogLevel.Info:
                    _log.Info(entry.Message, entry.Exception);
                    break;
                case LogLevel.Warning:
                    _log.Warn(entry.Message, entry.Exception);
                    break;
                case LogLevel.Trace:
                    _log.Debug(entry.Message, entry.Exception);
                    break;
            }
        }
    }
}