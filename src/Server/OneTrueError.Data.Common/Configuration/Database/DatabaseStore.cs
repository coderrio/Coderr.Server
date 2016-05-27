using System.Collections.Generic;
using Griffin.Data;

namespace OneTrueError.Infrastructure.Configuration.Database
{
    public class DatabaseStore : ConfigurationStore
    {
        /// <summary>
        ///     Load a settings section
        /// </summary>
        /// <typeparam name="T">Type of section</typeparam>
        /// <returns>Category if found; otherwise <c>null</c>.</returns>
        public override T Load<T>()
        {
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

            return section;
        }

        public override void Store(IConfigurationSection section)
        {
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
                    foreach (var kvp in section.ToConfigDictionary())
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
    }
}