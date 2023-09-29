using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Coderr.Server.App.Insights.Keyfigures
{
    public static class TrendLineExtensions
    {
        public static IEnumerable<TEntity> FillGaps<TEntity>(this IEnumerable<TEntity> source,
            Func<TEntity, DateTime> keySelector,
            Func<DateTime, TEntity> itemFactory)
        {
            var it = source.GetEnumerator();
            using (it)
            {
                var lastDate = DateTime.MinValue;
                while (it.MoveNext())
                {
                    if (lastDate != DateTime.MinValue)
                    {
                        while (lastDate.AddMonths(1) != keySelector(it.Current))
                        {
                            lastDate = lastDate.AddMonths(1);
                            yield return itemFactory(lastDate);
                        }
                    }

                    yield return it.Current;
                    lastDate = keySelector(it.Current);
                }
            }
        }
        static IEnumerable<IGrouping<DateTime, TElement>> FillGaps<TElement>(
            this IEnumerable<IGrouping<DateTime, TElement>> source,
            Func<DateTime, TElement> valueFactory)
        {
            var it = source.GetEnumerator();
            using (it)
            {
                var lastDate = DateTime.MinValue;
                while (it.MoveNext())
                {
                    if (lastDate != DateTime.MinValue)
                    {
                        while (lastDate.AddMonths(1) != it.Current.Key)
                        {
                            lastDate = lastDate.AddMonths(1);
                            yield return new Grouping<DateTime, TElement>(lastDate, new[] { valueFactory(lastDate) });
                        }
                    }

                    yield return it.Current;
                    lastDate = it.Current.Key;
                }
            }

        }
    }

    class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        private TKey _key;
        private IEnumerable<TElement> _elements;

        public Grouping(TKey key, IEnumerable<TElement> elements)
        {
            _key = key;
            _elements = elements;
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public TKey Key
        {
            get { return _key; }
        }
    }
}