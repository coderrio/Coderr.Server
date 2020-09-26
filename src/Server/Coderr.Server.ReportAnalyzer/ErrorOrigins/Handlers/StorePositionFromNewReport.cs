using System;
using System.Globalization;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.Domain.Modules.ErrorOrigins;
using Coderr.Server.ReportAnalyzer.Abstractions.ErrorReports;
using Coderr.Server.ReportAnalyzer.Abstractions.Incidents;
using DotNetCqs;
using log4net;

namespace Coderr.Server.ReportAnalyzer.ErrorOrigins.Handlers
{
    /// <summary>
    ///     Responsible of looking up geographic position of the IP address that delivered the report.
    /// </summary>
    public class StorePositionFromNewReport : IMessageHandler<ReportAddedToIncident>
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(StorePositionFromNewReport));
        private readonly IErrorOriginRepository _repository;
        private readonly IConfiguration<OriginsConfiguration> _originConfiguration;

        /// <summary>
        ///     Creates a new instance of <see cref="StorePositionFromNewReport" />.
        /// </summary>
        /// <param name="repository">repos</param>
        /// <exception cref="ArgumentNullException">repository</exception>
        public StorePositionFromNewReport(IErrorOriginRepository repository,
            IConfiguration<OriginsConfiguration> originConfiguration)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _originConfiguration = originConfiguration;
        }

        /// <summary>
        ///     Process an event asynchronously.
        /// </summary>
        /// <param name="e">event to process</param>
        /// <returns>
        ///     Task to wait on.
        /// </returns>
        public async Task HandleAsync(IMessageContext context, ReportAddedToIncident e)
        {
            // Random swedish IP for testing purposes
            if (e.Report.RemoteAddress == "::1" || e.Report.RemoteAddress == "127.0.0.1")
                e.Report.RemoteAddress = "94.254.57.227";

            var numberStyles = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;
            var collection = e.Report.GetCoderrCollection();
            if (collection != null)
            {
                var latitude = 0d;
                var longitude = 0d;
                var gotLat = collection.Properties.TryGetValue("Longitude", out var longitudeStr)
                             && double.TryParse(longitudeStr, numberStyles,
                                 CultureInfo.InvariantCulture, out longitude);

                var gotLong = collection.Properties.TryGetValue("Latitude", out var latitudeStr)
                              && double.TryParse(latitudeStr, numberStyles,
                                  CultureInfo.InvariantCulture, out latitude);
                if (gotLat && latitude > 0 && gotLong && longitude > 0)
                {
                    var errorOrigin2 = new ErrorOrigin(e.Report.RemoteAddress, longitude, latitude);
                    await _repository.CreateAsync(errorOrigin2, e.Incident.ApplicationId, e.Incident.Id, e.Report.Id);
                    return;
                }
            }


            var latitude1 = e.Report.FindCollectionProperty("ReportLatitude");
            var longitude1 = e.Report.FindCollectionProperty("ReportLongitude");
            if (longitude1 != null
                && double.TryParse(latitude1, numberStyles, CultureInfo.InvariantCulture,
                    out var latitude2)
                && latitude1 != null
                && double.TryParse(longitude1, numberStyles, CultureInfo.InvariantCulture,
                    out var longitude2))
            {
                var errorOrigin2 = new ErrorOrigin(e.Report.RemoteAddress, longitude2, latitude2);
                await _repository.CreateAsync(errorOrigin2, e.Incident.ApplicationId, e.Incident.Id, e.Report.Id);
                return;
            }


            if (string.IsNullOrEmpty(e.Report.RemoteAddress) ||
                string.IsNullOrEmpty(_originConfiguration.Value?.ApiKey)) return;


            var origin = new ErrorOrigin(e.Report.RemoteAddress);
            await _repository.CreateAsync(origin, e.Incident.ApplicationId, e.Incident.Id, e.Report.Id);
        }
    }
}