using Griffin.Data.Mapper;
using OneTrueError.App.Modules.Similarities.Domain;

namespace OneTrueError.SqlServer.Modules.Similarities.Mappers
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