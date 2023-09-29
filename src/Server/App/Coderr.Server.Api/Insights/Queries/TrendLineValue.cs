namespace Coderr.Server.Api.Insights.Queries
{
    public class TrendLineValue
    {
        public TrendLineValue(object value)
        {
            Value = value;
        }

        public TrendLineValue(object value, object normalizedValue)
        {
            Value = value;
            Normalized = normalizedValue;
        }

        public object Value { get; set; }
        public object Normalized { get; set; }

        public override string ToString()
        {
            return $"{Normalized} ({Value})";
        }
    }
}