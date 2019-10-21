using Coderr.Server.Domain.Modules.Similarities;
using Griffin.Data.Mapper;

namespace Coderr.Server.PostgreSQL.Modules.Similarities.Mappers
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