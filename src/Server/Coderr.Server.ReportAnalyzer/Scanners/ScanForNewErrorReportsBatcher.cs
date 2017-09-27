using System;
using Griffin.ApplicationServices;
using Griffin.Container;
using log4net;

namespace codeRR.Server.ReportAnalyzer.Scanners
{
    /// <summary>
    ///     Executes <see cref="ScanForNewErrorReports" /> from a background thread.
    /// </summary>
    [Component(Lifetime = Lifetime.Singleton)]
    public class ScanForNewErrorReportsBatcher : ApplicationServiceTimer
    {
        private readonly IScopedTaskInvoker _invoker;
        private ILog _logger = LogManager.GetLogger(typeof(ScanForNewErrorReportsBatcher));

        /// <summary>
        ///     Creates a new instance of <see cref="ScanForNewErrorReportsBatcher" />.
        /// </summary>
        /// <param name="invoker">Creates a IoC lifetime scope everytime <see cref="ScanForNewErrorReports" /> is executed.</param>
        /// <exception cref="ArgumentNullException">invoker</exception>
        public ScanForNewErrorReportsBatcher(IScopedTaskInvoker invoker)
        {
            if (invoker == null) throw new ArgumentNullException(nameof(invoker));
            _invoker = invoker;
        }

        /// <summary>
        ///     Used to do work periodically.
        /// </summary>
        /// <remarks>
        ///     Invoked every time the timer does an iteration. The interval is configured by
        ///     <see cref="P:Griffin.ApplicationServices.ApplicationServiceTimer.FirstInterval" /> and
        ///     <see cref="P:Griffin.ApplicationServices.ApplicationServiceTimer.Interval" />. The intervals
        ///     are paused during the execution of <c>Execute()</c> so that your method is not invoked twice if it doesn't complete
        ///     within the specified interval.
        /// </remarks>
        /// <example>
        ///     <code>
        /// protected override void Execute()
        ///             {
        ///                //Do some work.
        ///             }
        /// </code>
        /// </example>
        protected override void Execute()
        {
            while (true)
            {
                // There is no spoon.
                var fork = false;
                _invoker.Execute<ScanForNewErrorReports>(x => fork = x.Execute());
                if (false == fork)
                    break;
            }
        }
    }
}