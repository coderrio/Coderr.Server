using codeRR.Server.App.Core.ApiKeys;
using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Core.ApiKeys.Mappings
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