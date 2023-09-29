using Coderr.Server.Domain.Modules.UserNotifications;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Core.Notifications
{
    internal class BrowserSubscriptionMapper : CrudEntityMapper<BrowserSubscription>
    {
        public BrowserSubscriptionMapper() : base("NotificationsBrowser")
        {
            Property(x => x.Id)
                .PrimaryKey(true);
        }
    }
}