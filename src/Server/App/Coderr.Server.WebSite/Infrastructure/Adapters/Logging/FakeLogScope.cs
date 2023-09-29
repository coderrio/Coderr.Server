using System;

namespace Coderr.Server.WebSite.Infrastructure.Adapters.Logging
{
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