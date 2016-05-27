using System;
using System.Collections.Generic;
using Griffin.Data.Mapper;
using OneTrueError.Api.Core.Applications;
using OneTrueError.App.Core.Applications;

namespace OneTrueError.SqlServer.Core.Applications
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