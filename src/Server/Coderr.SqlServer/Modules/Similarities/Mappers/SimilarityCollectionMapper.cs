using Griffin.Data.Mapper;
using codeRR.App.Modules.Similarities.Domain;

namespace codeRR.SqlServer.Modules.Similarities.Mappers
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