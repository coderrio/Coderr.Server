using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coderr.Server.Domain.Core.Incidents;

namespace Coderr.Server.Domain.Core.ErrorReports
{
    public static class ReportsExtensions
    {
        public static ErrorReportContextCollection GetCoderrCollection(
            this IEnumerable<ErrorReportContextCollection> instance)
        {
            return instance.FirstOrDefault(x => x.Name == "CoderrData");
        }

        public static ErrorReportContextCollection GetCoderrCollection(
            this ErrorReportEntity instance)
        {
            return instance.ContextCollections.FirstOrDefault(x => x.Name == "CoderrData");
        }

    }
}
