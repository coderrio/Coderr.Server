using System;
using DotNetCqs.Queues;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace codeRR.Server.Infrastructure.Messaging
{
    /// <summary>
    ///     This serializer is used by the client/side and MAY NOT include type definitions.
    /// </summary>
    public class MessagingSerializer : IMessageSerializer<string>
    {
        private readonly Type _messageEnvelope;

        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.None,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ContractResolver = new IncludeNonPublicMembersContractResolver()
        };


        public MessagingSerializer()
        {
            _settings.Converters.Add(new StringEnumConverter());
        }

        public MessagingSerializer(Type messageEnvelope)
        {
            _messageEnvelope = messageEnvelope;
            _settings.Converters.Add(new StringEnumConverter());
        }

        object IMessageSerializer<string>.Deserialize(string contentType, string serializedDto)
        {
            try
            {
                var type = contentType == "Message"
                    ? _messageEnvelope
                    : Type.GetType(contentType);

                if (type == null)
                    throw new SerializationException($"Failed to lookup type \'{contentType}\'.", serializedDto);

                return JsonConvert.DeserializeObject(serializedDto, type, _settings);
            }
            catch (JsonException ex)
            {
                throw new SerializationException($"Failed to deserialize \'{contentType}\'.", serializedDto, ex);
            }
        }

        void IMessageSerializer<string>.Serialize(object dto, out string serializedDto, out string contentType)
        {
            try
            {
                contentType = dto.GetType().AssemblyQualifiedName;
                serializedDto = JsonConvert.SerializeObject(dto);
            }
            catch (JsonException ex)
            {
                throw new SerializationException($"Failed to serialize \'{dto.GetType().FullName}\'.", dto, ex);
            }
        }

        public object Deserialize(Type type, string message)
        {
            return JsonConvert.DeserializeObject(message, type, _settings);
        }

        public string Serialize(object message, out string contentType)
        {
            contentType = "application/json";
            return JsonConvert.SerializeObject(message, _settings);
        }
    }
}