using codeRR.Server.App.Modules.Similarities.Domain;
using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Modules.Similarities.Mappers
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