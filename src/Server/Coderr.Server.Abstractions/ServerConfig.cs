using System;

namespace Coderr.Server.Abstractions
{
    public class ServerConfig
    {
        public const string Premise = "Premise";
        public const string Live = "Live";

        public static ServerConfig Instance = new ServerConfig();
        private ServerType _serverType;

        public bool IsLive { get; private set; }

        public bool UseSmtpHandler { get; set; }

        public ServerType ServerType
        {
            get => _serverType;
            set
            {
                _serverType = value;
                IsLive = value == ServerType.Live;
                UseSmtpHandler = !IsLive;
            }
        }

        public QueueConfig Queues { get; set; } = new QueueConfig();
        public bool IsCommercial { get; set; }

        public bool IsModuleIgnored(Type type)
        {
            if (Instance.IsLive)
            {
                if (type.Assembly.GetName().Name.Contains($".{ServerConfig.Premise}"))
                    return true;
            }
            else
            {
                if (type.Assembly.GetName().Name.Contains($".{ServerConfig.Live}"))
                    return true;
            }

            return false;
        }
    }
}
