using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using Coderr.Server.App;
using Griffin.Data;

namespace Coderr.Server.SqlServer.Tests
{
    public class SimpleDbTest : IDisposable
    {
        private readonly List<DbConnection> _connections = new List<DbConnection>();

        public void Dispose()
        {
            foreach (var connection in _connections)
            {
                connection.Dispose();
            }
        }

        public void FillCommonIncidentData()
        {
            var random = new Random();
            using (var uow = OpenUow())
            {
                using (var cmd = uow.CreateDbCommand())
                {
                    cmd.CommandText =
                        @"DELETE FROM CommonIncidentProgressTracking";
                    cmd.ExecuteNonQuery();
                }

                var date = DateTime.Today.AddMonths(-12).ToFirstDayOfMonth();
                var incidentId = 1000;
                int monthIndex = 0;
                while (date < DateTime.Today)
                {
                    using (var cmd = uow.CreateDbCommand())
                    {
                        cmd.CommandText =
                            @"INSERT INTO CommonIncidentProgressTracking (IncidentId, ApplicationId, CreatedAtUtc, AssignedAtUtc, AssignedToId, ClosedById, ClosedAtUtc, ReOpenCount, ReOpenedAtUtc, VersionCount, Versions)
                                        VALUES (@incidentId, @applicationId, @creationDate, @assignDate, @accountId, @accountId, @closeDate, 1, GetUtcDate(), 1, '1.0')";
                        cmd.AddParameter("incidentId", incidentId++);
                        cmd.AddParameter("applicationId", 1);
                        cmd.AddParameter("creationDate", date.AddMinutes(random.Next(0, 60 * 24)));
                        var multiplier = GetDaysMultiplier(monthIndex);
                        cmd.AddParameter("assignDate", date.AddDays(1 * multiplier).AddMinutes(random.Next(0, 60 * 24)));
                        cmd.AddParameter("accountId", 1);
                        cmd.AddParameter("closeDate", date.AddDays(2 * multiplier).AddMinutes(random.Next(0, 60 * 24)));
                        cmd.ExecuteNonQuery();

                        if (monthIndex >= 11)
                        {
                            cmd.Parameters["incidentId"].Value = incidentId++;
                            cmd.ExecuteNonQuery();
                        }
                        if (monthIndex >= 12)
                        {
                            cmd.Parameters["incidentId"].Value = incidentId++;
                            cmd.ExecuteNonQuery();
                        }
                    }

                    date = date.AddMonths(1);
                    monthIndex++;
                }

                monthIndex = 0;
                date = DateTime.Today.AddMonths(-12).ToFirstDayOfMonth();
                while (date < DateTime.Today)
                {
                    using (var cmd = uow.CreateDbCommand())
                    {
                        cmd.CommandText =
                            @"INSERT INTO CommonIncidentProgressTracking (IncidentId, ApplicationId, CreatedAtUtc, AssignedAtUtc, AssignedToId, ClosedById, ClosedAtUtc, ReOpenCount, ReOpenedAtUtc, VersionCount, Versions)
                                        VALUES (@incidentId, @applicationId, @creationDate, @assignDate, @accountId, @accountId, @closeDate, 1, GetUtcDate(), 1, '1.0')";
                        cmd.AddParameter("incidentId", incidentId++);
                        cmd.AddParameter("applicationId", 2);
                        cmd.AddParameter("creationDate", date.AddMinutes(random.Next(0, 60 * 24)));
                        var multiplier = GetDaysMultiplier(monthIndex);
                        cmd.AddParameter("assignDate", date.AddDays(2 * multiplier).AddMinutes(random.Next(0, 60 * 24)));
                        cmd.AddParameter("accountId", 1);
                        cmd.AddParameter("closeDate", date.AddDays(3 * multiplier).AddMinutes(random.Next(0, 60 * 24)));
                        cmd.ExecuteNonQuery();

                        if (monthIndex >= 11)
                        {
                            cmd.Parameters["incidentId"].Value = incidentId++;
                            cmd.ExecuteNonQuery();
                        }
                        if (monthIndex >= 12)
                        {
                            cmd.Parameters["incidentId"].Value = incidentId++;
                            cmd.ExecuteNonQuery();
                        }
                    }

                    date = date.AddMonths(1);
                    monthIndex++;
                }

                uow.SaveChanges();
            }
        }

        private static int GetDaysMultiplier(int monthIndex)
        {
            var multiplier = monthIndex == 10
                ? 2
                : monthIndex == 11
                    ? 3
                    : 1;
            return multiplier;
        }

        public DbConnection OpenDb()
        {
            var conStr = ConfigurationManager.ConnectionStrings["IntegrationDb"];

            var connection = new SqlConnection(conStr.ConnectionString);
            connection.Open();
            _connections.Add(connection);
            return connection;
        }

        public AdoNetUnitOfWork OpenUow()
        {
            var con = OpenDb();
            return new AdoNetUnitOfWork(con);
        }
    }
}