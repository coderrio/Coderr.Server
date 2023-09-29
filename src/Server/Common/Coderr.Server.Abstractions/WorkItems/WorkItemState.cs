namespace Coderr.Server.Abstractions.WorkItems
{
    /// <summary>
    /// State for <see cref="WorkItemMapping"/>
    /// </summary>
    public enum WorkItemState
    {
        /// <summary>
        /// Not known
        /// </summary>
        Unspecified,

        /// <summary>
        /// Have not started working on it
        /// </summary>
        New,

        /// <summary>
        /// Active
        /// </summary>
        Active,

        /// <summary>
        /// Solved
        /// </summary>
        Solved,

        /// <summary>
        /// Removed from the other system
        /// </summary>
        Deleted
    }
}