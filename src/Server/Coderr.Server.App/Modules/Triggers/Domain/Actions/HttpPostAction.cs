using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DotNetCqs;
using log4net;
using Newtonsoft.Json;

namespace codeRR.Server.App.Modules.Triggers.Domain.Actions
{
    /// <summary>
    ///     Do a HTTP post in a trigger
    /// </summary>
    [TriggerActionName("Http")]
    public class HttpPostAction : ITriggerAction
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(HttpPostAction));

        /// <summary>
        ///     POSTs data using JSON. Json object is <c>{ Report = ErrorReport, Incident = incident }</c>
        /// </summary>
        /// <param name="context">trigger action context</param>
        public async Task ExecuteAsync(ActionExecutionContext context)
        {
            try
            {
                var request = WebRequest.CreateHttp(context.Config.Data);
                request.ContentType = "application/json";
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