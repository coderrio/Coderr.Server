namespace Coderr.Server.Domain.Core.Incidents
{
    /// <summary>
    ///     If the incident is escalated.
    /// </summary>
    public enum EscalationState
    {
        /// <summary>
        ///     Not escalated, prioritize according to the normal rules.
        /// </summary>
        Normal = 0,

        /// <summary>
        ///     Incident is important.
        /// </summary>
        Important = 1,

        /// <summary>
        ///     Error is escalated to critical.
        /// </summary>
        Critical = 2
    }
}