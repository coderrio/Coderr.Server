using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using log4net;
using Newtonsoft.Json.Linq;
using OneTrueError.Api.Core.Incidents.Events;

namespace OneTrueError.App.Modules.Geolocation.EventHandlers
{
    /// <summary>
    ///     Responsible of looking up geographic position of the IP address that delivered the report.
    /// </summary>
    [Component(RegisterAsSelf = true)]
    public class StorePositionFromNewReport : IApplicationEventSubscriber<ReportAddedToIncident>
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(StorePositionFromNewReport));
        private readonly IErrorOriginRepository _repository;

        /// <summary>
        ///     Creates a new instance of <see cref="StorePositionFromNewReport" />.
        /// </summary>
        /// <param name="repository">repos</param>
        /// <exception cref="ArgumentNullException">repository</exception>
        public StorePositionFromNewReport(IErrorOriginRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            _repository = repository;
        }

        /// <summary>
        ///     Process an event asynchronously.
        /// </summary>
        /// <param name="e">event to process</param>
        /// <returns>
        ///     Task to wait on.
        /// </returns>
        public async Task HandleAsync(ReportAddedToIncident e)
        {
            if (string.IsNullOrEmpty(e.Report.RemoteAddress))
                return;

            if (e.Report.RemoteAddress == "::1")
                return;
            if (e.Report.RemoteAddress == "127.0.0.1")
                return;

            var request = WebRequest.CreateHttp("http://freegeoip.net/json/" + e.Report.RemoteAddress);
            try
            {
                var response = await request.GetResponseAsync();
                var stream = response.GetResponseStream();
                var reader = new StreamReader(stream);
                var json = await reader.ReadToEndAsync();
                var jsonObj = JObject.Parse(json);

                /*    /*{"ip":"94.254.21.175","country_code":"SE","country_name":"Sweden","region_code":"10","region_name":"Dalarnas Lan","city":"Falun","zipcode":"",
                 * "latitude":60.6,"longitude":15.6333,
     * "metro_code":"","areacode":""}*/

                var lat = double.Parse(jsonObj["latitude"].Value<string>());
                var lon = double.Parse(jsonObj["longitude"].Value<string>());
                var cmd = new ErrorOrigin(e.Report.RemoteAddress, lon, lat);
                cmd.City = jsonObj["city"].ToString();
                cmd.CountryName = jsonObj["city"].ToString();
                cmd.CountryCode = jsonObj["country_code"].ToString();
                cmd.CountryName = jsonObj["country_name"].ToString();
                cmd.RegionCode = jsonObj["region_code"].ToString();
                cmd.RegionName = jsonObj["region_name"].ToString();
                cmd.ZipCode = jsonObj["zip_code"].ToString();

                await _repository.CreateAsync(cmd, e.Incident.ApplicationId, e.Incident.Id, e.Report.Id);
            }
            catch (Exception exception)
            {
                _logger.Error("Failed to store location.", exception);
            }
        }
    }
}