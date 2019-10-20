using System;
using System.Collections.Generic;
using System.Text;

namespace Coderr.IntegrationTests.Core.TestFramework
{
    public interface IEventReceiver
    {
        void Handle(object testEvent);
    }
}
