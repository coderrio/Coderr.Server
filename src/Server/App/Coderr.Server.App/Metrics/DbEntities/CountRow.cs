using System;
using Griffin.Data.Mapper;

namespace Coderr.Server.App.Metrics.DbEntities
{
    public class CountRow
    {
        public int? ApplicationId { get; set; }
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }

    public class CountRowMapper : EntityMapper<CountRow>
    {

    }
}