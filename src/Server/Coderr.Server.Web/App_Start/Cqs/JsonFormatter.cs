using System.IO;
using System.Messaging;
using System.Text;
using Newtonsoft.Json;

namespace codeRR.Server.Web.Cqs
{
    public class JsonFormatter : IMessageFormatter
    {
        public object Clone()
        {
            return this;
        }

        public bool CanRead(Message message)
        {
            return true;
        }

        public object Read(Message message)
        {
            var extension = Metadata.Parse(message.Extension);
            var type = extension.CreateType();
            var json = new StreamReader(message.BodyStream).ReadToEnd();
            return JsonConvert.DeserializeObject(json, type);
        }

        public void Write(Message message, object obj)
        {
            var extension = new Metadata(obj.GetType()).Serialize();
            message.Extension = extension;
            var json = JsonConvert.SerializeObject(obj);
            var buf = Encoding.UTF8.GetBytes(json);
            message.BodyStream = new MemoryStream(buf);
        }
    }
}