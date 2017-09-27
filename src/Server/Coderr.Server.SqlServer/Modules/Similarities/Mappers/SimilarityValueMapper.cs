using codeRR.Server.App.Modules.Similarities.Domain;
using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Modules.Similarities.Mappers
{
    public class SimilarityValueMapper : CrudEntityMapper<SimilarityValue>
    {
        public SimilarityValueMapper()
            : base("SimilarityValues")
        {
            //Property(x => x.Id).PrimaryKey(true);
        }
    }
}