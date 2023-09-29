using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;

namespace Coderr.Server.App.Insights.Keyfigures.Data
{
    [ContainerService]
    public class KeyPerformanceIndicatorRepository
    {
        public Task<IReadOnlyList<KeyPerformanceIndicatorValue>> GetAllAsync(int[] applicationIds, string kpiName)
        {
            throw new NotImplementedException();
        }

        public KeyPerformanceIndicatorValue GetMonth(int applicationId, string kpiName, DateTime date)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get this and last month
        /// </summary>
        /// <param name="indicatorName">KPI name</param>
        /// <param name="date">Year and month (day should be 1)</param>
        /// <returns>Months if found; otherwise an empty list</returns>
        public IReadOnlyList<KeyPerformanceIndicatorValue> GetTwoMonths(int applicationId, string indicatorName, DateTime date)
        {
            throw new NotImplementedException();
        }

        public void CreateAsync(KeyPerformanceIndicatorValue entity)
        {
            
        }
    }
}
