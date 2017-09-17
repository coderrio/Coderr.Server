using System;
using System.Collections.Generic;
using Griffin.Data;

namespace OneTrueError.Infrastructure.Configuration.Database
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
        private readonly Dictionary<Type, Wrapper> _items = new Dictionary<Type, Wrapper>();

        /// <summary>
        ///     Load a settings section
        /// </summary>
        /// <typeparam name="T">Type of section</typeparam>
        /// <returns>Category if found; otherwise <c>null</c>.</returns>
        public override T Load<T>()
        {
            lock (_items)
            {
                Wrapper t;
                if (_items.TryGetValue(typeof(T), out t) && !t.HasExpired())
                {
                    return (T)t.Value;
                }
            }

            var section = new T();
            using (var connection = ConnectionFactory.Create())
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
                            items[reader.GetString(0)] = reader.GetString(1);
                        }

                        if (items.Count == 0)
                            return default(T);

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
            using (var connection = ConnectionFactory.Create())
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

        private void SetCache(IConfigurationSection section)
        {
            lock (_items)
            {
                _items[section.GetType()] = new Wrapper {AddedAtUtc = DateTime.UtcNow, Value = section};
            }
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