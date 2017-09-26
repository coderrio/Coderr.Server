using System;
using Griffin.Logging;
using LogManager = log4net.LogManager;

namespace OneTrueError.Web.Infrastructure.Logging
{
    internal class LogManagerAdapter : ILogProvider
    {
        public ILogger GetLogger(Type type)
        {
            return new LogAdapter(LogManager.GetLogger(type));
        }
    }
}