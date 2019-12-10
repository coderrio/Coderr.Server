using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.V3.Pages.Account.Manage.Internal;
using Microsoft.Extensions.Configuration;
using WebPush;

namespace Coderr.Server.Web.Services
{
    [Coderr.Server.Abstractions.Boot.ContainerService]

    public class PushService
    {
        private string _vapidSubject;
        private string _vapidPublicKey;
        private string _vapidPrivateKey;

        public PushService(IConfiguration configuration)
        {
            _vapidSubject = configuration.GetValue<string>("Vapid:Subject");
            _vapidPublicKey = configuration.GetValue<string>("Vapid:PublicKey");
            _vapidPrivateKey = configuration.GetValue<string>("Vapid:PrivateKey");
        }

        public VapidKey Generate()
        {
            var keys = VapidHelper.GenerateVapidKeys();
            return new VapidKey
            {
                PrivateKey = keys.PrivateKey,
                PublicKey = keys.PublicKey
            };
        }

        public class VapidKey
        {
            public string PublicKey { get; set; }
            public string PrivateKey { get; set; }
        }
    }
}
