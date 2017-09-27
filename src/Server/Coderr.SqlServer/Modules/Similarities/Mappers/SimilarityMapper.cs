using Griffin.Data.Mapper;
using codeRR.App.Modules.Similarities.Domain;

namespace codeRR.SqlServer.Modules.Similarities.Mappers
{
    public class SimilarityMapper : CrudEntityMapper<Similarity>
    {
        public SimilarityMapper()
            : base("Similarities")
        {
            Property(x => x.Id)
                .PrimaryKey(true);

            Property(x => x.PropertyName)
                .ColumnName("Name");

            Property(x => x.Values)
                .NotForCrud()
                .NotForQueries();
        }
    }
}