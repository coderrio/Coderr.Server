using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using codeRR.Server.App.Configuration;
using codeRR.Server.Infrastructure.Configuration;
using Coderr.Server.PluginApi.Config;
using log4net;
using Newtonsoft.Json;

namespace codeRR.Server.App.Modules.Triggers.Domain.Actions
{
    /// <summary>
    ///     Send SMS (through Gauffin Interactive)
    /// </summary>
    [TriggerActionName("Sms")]
    public class SendSmsAction : ITriggerAction
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(SendSmsAction));
        private ConfigurationStore _configStore;

        public SendSmsAction(ConfigurationStore configStore)
        {
            _configStore = configStore;
        }

        /// <summary>
        ///     Execute action
        /// </summary>
        /// <param name="context"></param>
        public async Task ExecuteAsync(ActionExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            try
            {
                var config = _configStore.Load<BaseConfiguration>();
                var baseUrl = config.BaseUrl;
                //TODO: Add title
                var msg = "";
                if (context.Incident.ReportCount == 1)
                {
                    msg = string.Format("{2}\r\nurl: {0}/incident/{1}", baseUrl, context.Incident.Id,
                        context.Incident.Name);
                }
                else
                {
                    msg = string.Format("{0}\r\nreport count: {4}\r\nurl: {1}/incident/{2}/report/{3}\r\n",
                        context.Incident.Name, baseUrl, context.Incident.Id, context.ErrorReport.ReportId,
                        context.Incident.ReportCount);
                }

                //TODO: Move to our service.
                var iso = Encoding.GetEncoding("ISO-8859-1");
                var utfBytes = Encoding.UTF8.GetBytes(msg);
                var isoBytes = Encoding.Convert(Encoding.UTF8, iso, utfBytes);
                msg = iso.GetString(isoBytes);

                var request =
                    WebRequest.CreateHttp("https://web.smscom.se/sendsms.aspx?acc=ip1-755&pass=z35llww4&msg=" +
                                          Uri.EscapeDataString(msg) + "&to=" + context.Config.Data +
                                          "&from=codeRR&prio=2");
                request.ContentType = "application/json";
                request.Method = "POST";
                var stream = await request.GetRequestStreamAsync();
                var json = JsonConvert.SerializeObject(new {Report = context.ErrorReport, context.Incident});
                var buffer = Encoding.UTF8.GetBytes(json);
                stream.Write(buffer, 0, buffer.Length);
                await request.GetResponseAsync();
            }
            catch (Exception exception)
            {
                _log.Error("Failed to contact " + context.Config.Data, exception);
            }
        }
    }
}