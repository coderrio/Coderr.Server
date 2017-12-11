using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.Incidents;
using codeRR.Server.Api.Core.Reports;
using codeRR.Server.App.Configuration;
using codeRR.Server.App.Core.Users;
using codeRR.Server.Infrastructure.Configuration;
using Coderr.Server.PluginApi.Config;

namespace codeRR.Server.App.Core.Notifications.Tasks
{
    /// <summary>
    ///     Send SMS regarding an incident
    /// </summary>
    public class SendIncidentSms
    {
        private readonly IUserRepository _userRepository;
        private ConfigurationStore _configStore;

        /// <summary>
        ///     Creates a new instance of <see cref="SendIncidentSms" />.
        /// </summary>
        /// <param name="userRepository">to fetch phone number</param>
        /// <exception cref="ArgumentNullException">userRepository</exception>
        public SendIncidentSms(IUserRepository userRepository, ConfigurationStore configStore)
        {
            if (userRepository == null) throw new ArgumentNullException("userRepository");
            _userRepository = userRepository;
            _configStore = configStore;
        }

        /// <summary>
        ///     Send
        /// </summary>
        /// <param name="accountId">Account to send to</param>
        /// <param name="incident">Incident that the report belongs to</param>
        /// <param name="report">report being processed</param>
        /// <returns>task</returns>
        /// <exception cref="ArgumentNullException">incident;report</exception>
        /// <exception cref="ArgumentOutOfRangeException">accountId</exception>
        public async Task SendAsync(int accountId, IncidentSummaryDTO incident, ReportDTO report)
        {
            if (incident == null) throw new ArgumentNullException("incident");
            if (report == null) throw new ArgumentNullException("report");
            if (accountId <= 0) throw new ArgumentOutOfRangeException("accountId");

            var settings = await _userRepository.GetUserAsync(accountId);
            if (string.IsNullOrEmpty(settings.MobileNumber))
                return; //TODO: LOG

            var config = _configStore.Load<BaseConfiguration>();
            var url = config.BaseUrl;
            var shortName = incident.Name.Length > 20
                ? incident.Name.Substring(0, 20) + "..."
                : incident.Name;

            var exMsg = report.Exception.Message.Length > 100
                ? report.Exception.Message.Substring(0, 100)
                : report.Exception.Message;

            string msg;
            if (incident.IsReOpened)
            {
                msg = string.Format(@"ReOpened: {0}
{1}/#/incident/{2}

{3}", shortName, url, incident.Id, exMsg);
            }
            else if (incident.ReportCount == 1)
            {
                msg = string.Format(@"New: {0}
{1}/#/incident/{2}

Exception: {3}", shortName, url, incident.Id, exMsg);
            }
            else
            {
                msg = string.Format(@"Updated: {0}
ReportCount: {4}
{1}/#/incident/{2}

{3}", shortName, url, incident.Id, exMsg, incident.ReportCount);
            }

            var iso = Encoding.GetEncoding("ISO-8859-1");
            var utfBytes = Encoding.UTF8.GetBytes(msg);
            var isoBytes = Encoding.Convert(Encoding.UTF8, iso, utfBytes);
            msg = iso.GetString(isoBytes);

            var request =
                WebRequest.CreateHttp("https://web.smscom.se/sendsms.aspx?acc=ip1-755&pass=z35llww4&msg=" +
                                      Uri.EscapeDataString(msg) + "&to=" + settings.MobileNumber +
                                      "&from=codeRR&prio=2");
            request.ContentType = "application/json";
            request.Method = "GET";
            await request.GetResponseAsync();
        }
    }
}