using System;
using codeRR.Server.App.Core.Notifications;
using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Core.Notifications
{
    internal class UserNotificationSettingsMap : CrudEntityMapper<UserNotificationSettings>
    {
        public UserNotificationSettingsMap()
            : base("UserNotificationSettings")
        {
            Property(x => x.AccountId).ColumnName("AccountId").PrimaryKey();
            Property(x => x.ApplicationId).PrimaryKey();
            Property(x => x.NewIncident)
                .ToColumnValue(StringToEnum)
                .ToPropertyValue(EnumToString<NotificationState>);
            Property(x => x.ReopenedIncident)
                .ToColumnValue(StringToEnum)
                .ToPropertyValue(EnumToString<NotificationState>);
            Property(x => x.NewReport)
                .ToColumnValue(StringToEnum)
                .ToPropertyValue(EnumToString<NotificationState>);
            Property(x => x.UserFeedback)
                .ToColumnValue(StringToEnum)
                .ToPropertyValue(EnumToString<NotificationState>);
            Property(x => x.ApplicationSpike)
                .ToColumnValue(StringToEnum)
                .ToPropertyValue(EnumToString<NotificationState>);
            Property(x => x.WeeklySummary)
                .ToColumnValue(StringToEnum)
                .ToPropertyValue(EnumToString<NotificationState>);
        }

        public static object StringToEnum<TEnum>(TEnum notificationState)
        {
            return notificationState.ToString();
        }

        private TEnum EnumToString<TEnum>(object arg) where TEnum : struct
        {
            if (arg is DBNull)
                return default(TEnum);

            TEnum en;
            if (!Enum.TryParse(arg.ToString(), true, out en))
            {
                throw new MappingException(typeof(UserNotificationSettings),
                    "Failed to convert '" + arg + "' to '" + typeof(TEnum).FullName + "'.");
            }

            return en;
        }
    }
}