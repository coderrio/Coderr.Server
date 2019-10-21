using System.Linq;
using Coderr.Server.Domain.Modules.Similarities;

namespace Coderr.Server.PostgreSQL.Modules.Similarities.Entities
{
    public class ContextCollectionPropertyDbEntity
    {
        public ContextCollectionPropertyDbEntity()
        {
        }

        public ContextCollectionPropertyDbEntity(Similarity similarity)
        {
            Name = similarity.PropertyName;
            Values = similarity.Values.Select(x => new ContextCollectionPropertyValueDbEntity(x)).ToArray();
        }

        public string Name { get; set; }
        public ContextCollectionPropertyValueDbEntity[] Values { get; set; }
    }
}