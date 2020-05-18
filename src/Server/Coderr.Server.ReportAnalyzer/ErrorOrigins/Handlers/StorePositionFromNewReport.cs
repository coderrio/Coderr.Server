using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Coderr.Server.Domain.Modules.ErrorOrigins;
using Coderr.Server.ReportAnalyzer.Abstractions.Incidents;
using DotNetCqs;
using log4net;
using Newtonsoft.Json.Linq;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.ReportAnalyzer;
using Coderr.Server.ReportAnalyzer.Abstractions.ErrorReports;

namespace Coderr.Server.ReportAnalyzer.ErrorOrigins.Handlers
{
    /// <summary>
    ///     Responsible of looking up geographic position of the IP address that delivered the report.
    /// </summary>
    public class StorePositionFromNewReport : IMessageHandler<ReportAddedToIncident>
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(StorePositionFromNewReport));
        private readonly IErrorOriginRepository _repository;
        private IConfiguration<OriginsConfiguration> _originConfiguration;

        /// <summary>
        ///     Creates a new instance of <see cref="StorePositionFromNewReport" />.
        /// </summary>
        /// <param name="repository">repos</param>
        /// <exception cref="ArgumentNullException">repository</exception>
        public StorePositionFromNewReport(IErrorOriginRepository repository, IConfiguration<OriginsConfiguration> originConfiguration)
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
            var collection = e.Report.GetCoderrCollection();
            if (collection != null)
            {
                var latitude = 0d;
                var longitude = 0d;
                var gotLat = collection.Properties.TryGetValue("Longitude", out var longitudeStr)
                             && double.TryParse(longitudeStr, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out longitude);

                var gotLong = collection.Properties.TryGetValue("Latitude", out var latitudeStr)
                              && double.TryParse(latitudeStr, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out latitude);
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
                && double.TryParse(latitude1, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var latitude2)
                && latitude1 != null
                && double.TryParse(longitude1, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var longitude2))
            {
                var errorOrigin2 = new ErrorOrigin(e.Report.RemoteAddress, longitude2, latitude2);
                await _repository.CreateAsync(errorOrigin2, e.Incident.ApplicationId, e.Incident.Id, e.Report.Id);
                return;
            }


            if (string.IsNullOrEmpty(e.Report.RemoteAddress) || string.IsNullOrEmpty(_originConfiguration.Value?.ApiKey))
            {
                return;
            }


            // Random swedish IP for testing purposes
            if (e.Report.RemoteAddress == "::1" || e.Report.RemoteAddress == "127.0.0.1")
                e.Report.RemoteAddress = "94.254.57.227";

            var errorOrigin = await LookupIpAddress(e);
            await _repository.CreateAsync(errorOrigin, e.Incident.ApplicationId, e.Incident.Id, e.Report.Id);
        }

        private async Task<ErrorOrigin> LookupIpAddress(ReportAddedToIncident e)
        {
            var url = $"http://api.ipstack.com/{e.Report.RemoteAddress}?access_key={_originConfiguration.Value.ApiKey}";
            var request = WebRequest.CreateHttp(url);
            var json = "";
            ErrorOrigin errorOrigin;
            try
            {
                var response = await request.GetResponseAsync();
                var stream = response.GetResponseStream();
                var reader = new StreamReader(stream);
                json = await reader.ReadToEndAsync();
                var jsonObj = JObject.Parse(json);

                /*    /*{"ip":"94.254.21.175","country_code":"SE","country_name":"Sweden","region_code":"10","region_name":"Dalarnas Lan","city":"Falun","zipcode":"",
                 * "latitude":60.6,"longitude":15.6333,
     * "metro_code":"","areacode":""}*/

                var lat = double.Parse(jsonObj["latitude"].Value<string>(), CultureInfo.InvariantCulture);
                var lon = double.Parse(jsonObj["longitude"].Value<string>(), CultureInfo.InvariantCulture);
                errorOrigin = new ErrorOrigin(e.Report.RemoteAddress, lon, lat)
                {
                    City = jsonObj["city"].ToString(),
                    CountryCode = jsonObj["country_code"].ToString(),
                    CountryName = jsonObj["country_name"].ToString(),
                    RegionCode = jsonObj["region_code"].ToString(),
                    RegionName = jsonObj["region_name"].ToString(),
                    ZipCode = jsonObj["zip"].ToString()
                };
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException($"Failed to call lookupService or parse the JSON: {json}.", exception);
            }

            return errorOrigin;
        }
    }
}