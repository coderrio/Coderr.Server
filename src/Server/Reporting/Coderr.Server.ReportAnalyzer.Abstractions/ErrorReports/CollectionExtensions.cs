using System.Collections.Generic;
using System.Linq;

namespace Coderr.Server.ReportAnalyzer.Abstractions.ErrorReports
{
    public static class ReportExtensions
    {

        public static ContextCollectionDTO GetCoderrCollection(
            this IEnumerable<ContextCollectionDTO> instance)
        {
            return instance.FirstOrDefault(x => x.Name == "CoderrData");
        }

        public static ContextCollectionDTO GetCoderrCollection(
            this ReportDTO instance)
        {
            return instance.ContextCollections.FirstOrDefault(x => x.Name == "CoderrData");
        }



    }
}
