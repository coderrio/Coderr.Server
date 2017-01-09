using Griffin.Data.Mapper;
using OneTrueError.App.Modules.Similarities.Domain;

namespace OneTrueError.SqlServer.Modules.Similarities.Mappers
{
    public class SimilarityCollectionMapper : CrudEntityMapper<SimilarityCollection>
    {
        public SimilarityCollectionMapper()
            : base("SimilarityCollections")
        {
            Property(x => x.Id)
                .PrimaryKey(true);

            Property(x => x.Properties)
                .NotForCrud()
                .NotForQueries();

            Property(x => x.Name)
                .ColumnName("ContextName");
        }
    }
}