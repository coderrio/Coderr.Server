using System.Data;
using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Core.ApiKeys.Mappings
{
    public class IntMapper : IEntityMapper<int>
    {
        public object Create(IDataRecord record)
        {
            return record[0];
        }

        public void Map(IDataRecord source, object destination)
        {
        }

        public void Map(IDataRecord source, int destination)
        {
        }
    }
}