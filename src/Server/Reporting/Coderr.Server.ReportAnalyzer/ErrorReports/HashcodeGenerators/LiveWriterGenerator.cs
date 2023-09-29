using System;
using System.Collections.Generic;
using System.Text;
using Coderr.Server.Domain.Core.ErrorReports;

namespace Coderr.Server.ReportAnalyzer.ErrorReports.HashcodeGenerators
{
    
    class LiveWriterGenerator : IHashCodeSubGenerator
    {
        public bool CanGenerateFrom(ErrorReportEntity entity)
        {
            return false;
        }

        public ErrorHashCode GenerateHashCode(ErrorReportEntity entity)
        {
            return null;
        }
    }
}
