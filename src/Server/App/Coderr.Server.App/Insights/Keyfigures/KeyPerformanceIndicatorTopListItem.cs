namespace Coderr.Server.App.Insights.Keyfigures
{
    public class KeyPerformanceIndicatorTopListItem
    {
        /// <summary>
        ///     value id (must then be of the same type as the indicator ValueType)
        /// </summary>
        public int? ValueId { get; set; }

        /// <summary>
        ///     Name to display (if ValueId is not set).
        /// </summary>
        public string ValueName { get; set; }

        public object Value { get; set; }
        public string Comment { get; set; }
    }
}