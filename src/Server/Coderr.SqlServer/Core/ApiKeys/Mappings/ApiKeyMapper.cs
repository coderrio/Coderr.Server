using Griffin.Data.Mapper;
using codeRR.App.Core.ApiKeys;

namespace codeRR.SqlServer.Core.ApiKeys.Mappings
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