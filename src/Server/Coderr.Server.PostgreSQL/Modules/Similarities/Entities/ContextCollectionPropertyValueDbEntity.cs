using System;
using Coderr.Server.Domain.Modules.Similarities;

namespace Coderr.Server.PostgreSQL.Modules.Similarities.Entities
{
    public class ContextCollectionPropertyValueDbEntity
    {
        public ContextCollectionPropertyValueDbEntity(SimilarityValue x)
        {
            if (x == null) throw new ArgumentNullException(nameof(x));
            Value = x.Value;
            Count = x.Count;
            Percentage = x.Percentage;
        }

        protected ContextCollectionPropertyValueDbEntity()
        {
        }


        public int Count { get; set; }

        public int Percentage { get; set; }
        public string Value { get; set; }
    }
}