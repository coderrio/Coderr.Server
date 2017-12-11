using System;
using System.Collections.Generic;
using System.Data;
using Coderr.Server.PluginApi.Config;
using Griffin.Data;

namespace codeRR.Server.Infrastructure.Configuration.Database
{
    /// <summary>
    ///     Uses a DB to store configuration.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Items are cached for 30 seconds to avoid loading the DB.
    ///     </para>
    /// </remarks>
    public class DatabaseStore : ConfigurationStore
    {
        private static readonly Dictionary<Type, Wrapper> _cachedItems = new Dictionary<Type, Wrapper>();
        private readonly Func<IDbConnection> _connectionFactory;

        public DatabaseStore(Func<IDbConnection> connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        /// <summary>
        ///     Load a settings section
        /// </summary>
        /// <typeparam name="T">Type of section</typeparam>
        /// <returns>Category if found; otherwise <c>null</c>.</returns>
        public override T Load<T>()
        {
            if (TryGetCachedItem(out T tValue))
                return tValue;

            var section = new T();
            using (var connection = OpenConnectionFor<T>())
            {
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT Name, Value FROM Settings WHERE section = @section";
                    cmd.AddParameter("section", section.SectionName);
                    using (var reader = cmd.ExecuteReader())
                    {
                        var items = new Dictionary<string, string>();
                        while (reader.Read())
                        {
                            var name = reader.GetString(0);
                            var dbValue = reader.GetValue(1);
                            var value = dbValue is DBNull ? null : (string) dbValue;
                            items[name] = value;
                        }

                        // all config classes should have defaults.
                        if (items.Count == 0)
                            return new T(); 

                        section.Load(items);
                    }
                }
            }

            SetCache(section);
            return section;
        }

        public override void Store(IConfigurationSection section)
        {
            SetCache(section);
            using (var connection = OpenConnectionFor(section.GetType()))
            {
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Settings WHERE section = @section";
                    cmd.AddParameter("section", section.SectionName);
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = connection.CreateCommand())
                {
                    var index = 0;
                    foreach (var kvp in section.ToDictionary())
                    {
                        cmd.CommandText +=
                            string.Format(
                                "INSERT INTO Settings (Section, Name, Value) VALUES(@section, @name{0}, @value{0})",
                                index);
                        cmd.AddParameter("name" + index, kvp.Key);
                        cmd.AddParameter("value" + index, kvp.Value);
                        ++index;
                    }
                    cmd.AddParameter("section", section.SectionName);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        ///     Allow connections to be created by another peep.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>open connection</returns>
        protected virtual IDbConnection OpenConnectionFor<T>()
        {
            return OpenConnectionFor(typeof(T));
        }

        /// <summary>
        ///     Allow connections to be created by another peep.
        /// </summary>
        /// <param name="configClassType">A <c>IConfigurationSection</c> type</param>
        /// <returns>open connection</returns>
        protected virtual IDbConnection OpenConnectionFor(Type configClassType)
        {
            if (configClassType == null) throw new ArgumentNullException(nameof(configClassType));
            return _connectionFactory();
        }

        protected virtual void SetCache(IConfigurationSection section)
        {
            lock (_cachedItems)
            {
                _cachedItems[section.GetType()] = new Wrapper {AddedAtUtc = DateTime.UtcNow, Value = section};
            }
        }

        protected virtual bool TryGetCachedItem<T>(out T tValue)
        {
            lock (_cachedItems)
            {
                if (_cachedItems.TryGetValue(typeof(T), out var t) && !t.HasExpired())
                {
                    tValue = (T) t.Value;
                    return true;
                }
            }

            tValue = default(T);
            return false;
        }

        private class Wrapper
        {
            public DateTime AddedAtUtc { get; set; }
            public object Value { get; set; }

            public bool HasExpired()
            {
                return DateTime.UtcNow.Subtract(AddedAtUtc).TotalSeconds >= 60;
            }
        }
    }
}