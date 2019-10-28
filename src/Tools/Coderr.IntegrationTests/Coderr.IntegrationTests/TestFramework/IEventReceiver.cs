namespace Coderr.IntegrationTests.Core.TestFramework
{
    public interface IEventReceiver
    {
        void Handle(object testEvent);
    }
}