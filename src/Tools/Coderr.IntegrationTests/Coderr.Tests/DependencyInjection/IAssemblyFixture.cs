using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Coderr.Tests.DependencyInjection
{
    public interface IAssemblyFixture
    {
        Task Prepare();
        void RegisterServices(IServiceCollection builder);

        Task Cleanup();
    }
}
