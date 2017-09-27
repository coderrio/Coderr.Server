using Griffin.Data.Mapper;
using codeRR.App.Modules.Similarities.Domain;

namespace codeRR.SqlServer.Modules.Similarities.Mappers
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