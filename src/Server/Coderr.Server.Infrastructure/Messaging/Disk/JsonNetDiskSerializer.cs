using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Coderr.Server.Infrastructure.Messaging.Disk.Queue;
using Newtonsoft.Json;

namespace Coderr.Server.Infrastructure.Messaging.Disk
{
    public class JsonNetDiskSerializer : IContentSerializer
    {
        private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ContractResolver = new IncludeNonPublicMembersContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
        };

        public Task SerializeAsync(Stream destination, object entity)
        {
            var serializer =
                new JsonSerializer
                {
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                    ContractResolver = new IncludeNonPublicMembersContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore,
                    TypeNameHandling = TypeNameHandling.Auto,
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
                };
            using (var writer = new StreamWriter(destination, Encoding.UTF8, 65535, true))
            {
                serializer.Serialize(writer, entity);
            }

            return Task.CompletedTask;
        }

        public Task<object> DeserializeAsync(Stream source, int recordSize, Type entityType)
        {
            var serializer =
                new JsonSerializer
                {
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                    ContractResolver = new IncludeNonPublicMembersContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                };

            var reader2 = new StreamReader(source, Encoding.UTF8, true, 65535, true);
            var json = reader2.ReadToEnd();
            var entry = JsonConvert.DeserializeObject(json, entityType, _settings);
            return Task.FromResult(entry);
        }
    }
}