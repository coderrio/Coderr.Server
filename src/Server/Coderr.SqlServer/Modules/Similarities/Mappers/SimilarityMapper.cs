using codeRR.Server.App.Modules.Similarities.Domain;
using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Modules.Similarities.Mappers
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