using System;
using codeRR.Server.Api.Core.Applications;
using codeRR.Server.App.Core.Applications;
using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Core.Applications
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