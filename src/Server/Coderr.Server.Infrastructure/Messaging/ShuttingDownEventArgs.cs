using System;

namespace Coderr.Server.Infrastructure.Messaging
{
    public class ShuttingDownEventArgs : EventArgs
    {
        public bool CanShutdown { get; set; }
    }
}