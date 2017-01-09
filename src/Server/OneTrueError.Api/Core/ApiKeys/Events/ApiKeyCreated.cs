using System;
using DotNetCqs;

namespace OneTrueError.Api.Core.ApiKeys.Events
{
    public class ApiKeyCreated : ApplicationEvent
    {
        private readonly int[] _applicationIds;

        public ApiKeyCreated(string applicationNameForTheAppUsingTheKey, string apiKey, string sharedSecret,
            int[] applicationIds,
            int createdById)
        {
            if (sharedSecret == null) throw new ArgumentNullException("sharedSecret");
            _applicationIds = applicationIds;
            ApplicationNameForTheAppUsingTheKey = applicationNameForTheAppUsingTheKey;
            ApiKey = apiKey;
            SharedSecret = sharedSecret;
            CreatedById = createdById;
        }

        protected ApiKeyCreated()
        {
        }

        public string ApiKey { get; set; }
        public string ApplicationNameForTheAppUsingTheKey { get; set; }
        public int CreatedById { get; set; }
        public string SharedSecret { get; set; }
    }
}