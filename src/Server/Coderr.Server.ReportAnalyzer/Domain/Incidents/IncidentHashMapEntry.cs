//using System;
//using System.Collections.Generic;
//using System.Linq;
//using codeRR.ReportAnalyzer.Domain.Reports;

//namespace codeRR.ReportAnalyzer.Domain.Incidents
//{
//    /// <summary>
//    ///     Different error reports can get the same hash code.
//    ///     this entity is used to be able to find the correct incident by mapping exception full names to incidentIds.
//    ///     (we might have to store the stack trace instead).
//    /// </summary>
//    public class IncidentHashMapEntry
//    {
//        /// <summary>
//        /// Creates a new instance of <see cref="IncidentHashMapEntry"/>.
//        /// </summary>
//        public IncidentHashMapEntry()
//        {
//            IncidentFullPaths = new Dictionary<string, string>();
//        }


//        /// <summary>
//        ///     Map where the key is <c>incident.FirstStacktraceLine</c>
//        /// </summary>
//        public Dictionary<string, string> IncidentFullPaths { get; private set; }

//        /// <summary>
//        /// Generate a hashcode from an incident
//        /// </summary>
//        /// <param name="entity"></param>
//        /// <returns></returns>
//        /// <exception cref="InvalidOperationException"></exception>
//        public string GetIncidentId(ErrorReportEntity entity)
//        {
//            if (IncidentFullPaths.Count == 1)
//                return IncidentFullPaths.Values.First();

//            if (entity.Exception == null)
//                throw new InvalidOperationException(string.Format("Failed to exception in entity: {0}", entity.Id));

//            if (entity.Exception.StackTrace == null)
//                throw new InvalidOperationException(string.Format("Failed to stack trace in entity: {0}", entity.Id));

//            int pos = entity.Exception.StackTrace.IndexOf("\r\n");
//            if (pos == -1)
//                throw new InvalidOperationException(
//                    string.Format("Failed to find first line in stack trace: {0} for entity {1}",
//                        entity.Exception.StackTrace, entity.Id));

//            string firstLine = entity.Exception.StackTrace.Substring(0, pos);

//            return IncidentFullPaths[firstLine];
//        }

//        public void Add(string incidentId, string firstLineInStacktrace)
//        {
//            IncidentFullPaths[firstLineInStacktrace] = incidentId;
//        }
//    }
//}

