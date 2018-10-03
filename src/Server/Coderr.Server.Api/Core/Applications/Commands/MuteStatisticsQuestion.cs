using System;
using System.Collections.Generic;
using System.Text;

namespace Coderr.Server.Api.Core.Applications.Commands
{
    /// <summary>
    /// Do not pester the user with the question about adding info for statistics.
    /// </summary>
    [Command]
    public class MuteStatisticsQuestion 
    {
        public int ApplicationId { get; set; }
    }
}
