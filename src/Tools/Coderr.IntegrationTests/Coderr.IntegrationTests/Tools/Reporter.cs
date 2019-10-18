using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using Coderr.Client.Config;
using Coderr.Client.Contracts;
using Coderr.Client.Processor;
using Coderr.Client.Reporters;
using Coderr.Client.Uploaders;

namespace Coderr.IntegrationTests.Core.Tools
{
    public class Reporter
    {
        private readonly List<Func<string, Exception>> _exceptions = new List<Func<string, Exception>>
        {
            msg => new InvalidOperationException(msg),
            msg => new FormatException(msg),
            msg => new InvalidDataException(msg),
            msg => new InvalidOperationException(msg),
            msg => new ArgumentException(msg),
            msg => new InvalidCastException(msg),
            msg => new SecurityException(msg)
        };

        private readonly Random _random = new Random();

        private readonly ExceptionProcessor _processor;
        private CoderrConfiguration _config;

        public Reporter(Uri address, string appKey, string sharedSecret)
        {
            _config = new CoderrConfiguration();
            _config.Uploaders.Register(new UploadToCoderr(address, appKey, sharedSecret));
            _processor = new ExceptionProcessor(_config);
        }

        public Exception CreateException(string message)
        {
            try
            {
                var msg = Guid.NewGuid().ToString("N").Substring(0, 4) + message;
                throw _exceptions[_random.Next(0, _exceptions.Count)](msg);
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        public ErrorReportDTO ReportUnique(string message)
        {
            var ex = CreateException(message);
            var report = _processor.Build(ex);

            var exType = report.Exception.GetType();
            var value = exType.GetProperty("StackTrace").GetValue(report.Exception);
            value = "  at " + Guid.NewGuid().ToString("N") + ":line 40\r\n" + value;
            exType.GetProperty("StackTrace").SetValue(report.Exception, value);

            _config.Uploaders.Upload(report);
            return report;
        }


        public ErrorReportDTO ReportUnique(string message, object contextData)
        {
            var ex = CreateException(message);
            var report =_processor.Build(ex, contextData);

            var exType = report.Exception.GetType();
            var value = exType.GetProperty("StackTrace").GetValue(ex);
            value = "  at " + Guid.NewGuid().ToString("N") + ":line 40\r\n" + value;
            exType.GetProperty("StackTrace").SetValue(ex, value);

            _config.Uploaders.Upload(report);
            return report;
        }

        public void ReportCopy(ErrorReportDTO blueprint)
        {
            var id = ReportIdGenerator.Generate(new Exception());
            var newReport = new ErrorReportDTO(id, blueprint.Exception, blueprint.ContextCollections);
            _config.Uploaders.Upload(newReport);
        }

        public void Report(Exception ex)
        {
            _processor.Process(ex);
        }

        public void Report(IErrorReporterContext context)
        {
            _processor.Process(context);
        }
    }
}