using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Coderr.Server.ReportAnalyzer.Boot.Adapters
{
    /// <summary>
    /// Used to move principal from the queue to the current container scope so that a proper DB connection can be opened.
    /// </summary>
    class ScopedPrincipal
    {
        public ClaimsPrincipal Principal { get; set; }
    }
}
