using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codeRR.Infrastructure
{
    public interface IUrlHelper
    {
        string ToAbsolutePath(string virtualPath);
    }
}
