using System;
using Griffin.Data.Mapper;
using codeRR.Api.Core.Applications;
using codeRR.App.Core.Applications;

namespace codeRR.SqlServer.Core.Applications
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