using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using codeRR.Server.Api.Core.Reports;

namespace codeRR.Server.App.Modules.Similarities.Domain
{
    /// <summary>
    ///     A collection corresponding to <see cref="ContextCollectionDTO" />, but where value usage have been analyzed.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix",
        Justification = "Namespace is named 'Similarities'.")]
    public class SimilarityCollection
    {
        private readonly List<Similarity> _items = new List<Similarity>();

        /// <summary>
        ///     Creates a new instance of <see cref="SimilarityCollection" />.
        /// </summary>
        /// <param name="incidentId">Incident that the collection belongs to</param>
        /// <param name="contextName">Name of the context collection that this is a analysis for.</param>
        /// <exception cref="ArgumentNullException">contextName</exception>
        /// <exception cref="ArgumentOutOfRangeException">incidentId</exception>
        public SimilarityCollection(int incidentId, string contextName)
        {
            if (incidentId <= 0) throw new ArgumentOutOfRangeException(nameof(incidentId));
            IncidentId = incidentId;
            Name = contextName ?? throw new ArgumentNullException(nameof(contextName));
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected SimilarityCollection()
        {
        }

        /// <summary>
        ///     Similarity collection identity
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        ///     Incident that this collection belongs to
        /// </summary>
        public int IncidentId { get; private set; }

        /// <summary>
        ///     Name of the context collection
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     All analysed properties
        /// </summary>
        public IList<Similarity> Properties
        {
            get { return _items; }
        }

        /// <summary>
        ///     Add a new property value
        /// </summary>
        /// <param name="propertyName">Property name, same as in the context collection that we are analysing</param>
        /// <param name="adaptedValue">normalized value</param>
        public void Add(string propertyName, object adaptedValue)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");
            if (adaptedValue == null) throw new ArgumentNullException("adaptedValue");

            Similarity item = null;

            // ReSharper disable once ForCanBeConvertedToForeach
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var i = 0; i < _items.Count; i++)
            {
                if (_items[i].PropertyName != propertyName)
                    continue;

                item = _items[i];
                break;
            }

            if (item == null)
            {
                item = new Similarity(propertyName);
                _items.Add(item);
            }

            item.AddValue(adaptedValue.ToString());
        }

        /// <summary>
        ///     Add a new similarity
        /// </summary>
        /// <param name="similarity">similarity</param>
        /// <exception cref="ArgumentNullException">similarity</exception>
        public void Add(Similarity similarity)
        {
            if (similarity == null) throw new ArgumentNullException("similarity");
            _items.Add(similarity);
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     Context name
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return Name;
        }
    }
}

/*CREATE TABLE [dbo].[Similarities] (
    Id int identity NOT NULL primary key,
    CollectionId int not null,
	Name varchar(40) not null,
	MostCommonValue varchar(40) not null,
	MostCommonValuePercentage smallint not null,
	ValueCount int not null,
);


CREATE TABLE [dbo].[SimilarityValues] (
    Id int identity NOT NULL primary key,
    SimilarityId int not null,
	Name varchar(40) not null,
	Count int not null,
	Percentage int not null,
	Value nvarchar(max) not null,
);
*/