using System;

namespace Coderr.Tests.Runners.AppDomain.DTO
{
    [Serializable]
    public class RunTests
    {
        /// <summary>
        /// Assembly.GetName().Name
        /// </summary>
        public string AssemblyName { get; set; }
        public string AssemblyPath { get; set; }

        public TestCaseToRun[] TestCases { get; set; }

        /// <summary>
        /// From vsts
        /// </summary>
        public string Source { get; set; }
    }
}