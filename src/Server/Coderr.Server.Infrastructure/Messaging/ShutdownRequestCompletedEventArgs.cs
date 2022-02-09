namespace Coderr.Server.Infrastructure.Messaging
{
    public class ShutdownRequestCompletedEventArgs
    {
        public ShutdownRequestCompletedEventArgs(bool isShutDown)
        {
            IsShutDown = isShutDown;
        }

        public bool IsShutDown { get; }
    }
}