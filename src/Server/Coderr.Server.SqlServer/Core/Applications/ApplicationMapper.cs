using System;
using Coderr.Server.Domain.Core.Applications;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Core.Applications
{
    public class ApplicationMapper : CrudEntityMapper<Application>
    {
        public ApplicationMapper() : base("Applications")
        {
            Property(x => x.ApplicationType)
                .ToPropertyValue(o => (TypeOfApplication) Enum.Parse(typeof(TypeOfApplication), (string) o));
        }
    }
}