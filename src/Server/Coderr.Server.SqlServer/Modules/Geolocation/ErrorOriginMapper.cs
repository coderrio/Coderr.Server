using System;
using Coderr.Server.Domain.Modules.ErrorOrigins;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Modules.Geolocation
{
    public class ErrorOriginMapper : CrudEntityMapper<ErrorOrigin>
    {
        public ErrorOriginMapper() : base("ErrorOrigins")
        {
            Property(x => x.Longitude)
                .ToColumnValue2(x => Convert.ToDecimal(x.Value))
                .ToPropertyValue2(x => Convert.ToDouble(x.Value));
            Property(x => x.Latitude)
                .ToColumnValue2(x => Convert.ToDecimal(x.Value))
                .ToPropertyValue2(x => Convert.ToDouble(x.Value));

        }
    }
}