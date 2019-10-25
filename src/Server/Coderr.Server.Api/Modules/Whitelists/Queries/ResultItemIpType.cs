namespace Coderr.Server.Api.Modules.Whitelists.Queries
{
    /// <summary>
    ///     Typ of stored IP record.
    /// </summary>
    public enum ResultItemIpType
    {
        /// <summary>
        ///     Added when doing a lookup for the domain
        /// </summary>
        Lookup = 0,

        /// <summary>
        ///     Manually specified by the user
        /// </summary>
        Manual = 1,

        /// <summary>
        ///     We got a request from this IP and a lookup didn't match it.
        /// </summary>
        Denied = 2
    }
}