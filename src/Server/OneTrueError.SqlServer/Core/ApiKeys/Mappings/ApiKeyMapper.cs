using Griffin.Data.Mapper;
using OneTrueError.App.Core.ApiKeys;

namespace OneTrueError.SqlServer.Core.ApiKeys.Mappings
{
    public class ApiKeyMapper : CrudEntityMapper<ApiKey>
    {
        public ApiKeyMapper() : base("ApiKeys")
        {
            Property(x => x.Id)
                .PrimaryKey(true);
            Property(x => x.Claims)
                .Ignore();
        }
    }
}