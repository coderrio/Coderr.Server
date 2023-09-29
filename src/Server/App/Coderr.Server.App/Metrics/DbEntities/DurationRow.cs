using System;
using Griffin.Data.Mapper;

namespace Coderr.Server.App.Metrics.DbEntities
{
    public class DurationRow
    {
        public int? ApplicationId { get; set; }
        public DateTime Date { get; set; }
        public decimal Duration { get; set; }

    }

    public class DurationRowMapper : EntityMapper<DurationRow>
    {

    }

}
