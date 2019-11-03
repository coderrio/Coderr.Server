namespace Coderr.Tests.Events
{
    public interface IEventReceiver
    {
        void Handle(object testEvent);
    }
}