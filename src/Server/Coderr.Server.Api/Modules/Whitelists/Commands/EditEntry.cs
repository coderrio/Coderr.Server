namespace Coderr.Server.Api.Modules.Whitelists.Commands
{
    /// <summary>
    ///     Edit a domain that may post error reports without using a shared secret (javascript applications)
    /// </summary>
    [Command]
    public class EditEntry
    {
        /// <summary>
        /// PK for the entry being edited.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Applications that the domain is allowed for.
        /// </summary>
        public int[] ApplicationIds { get; set; } = new int[0];


        /// <summary>
        ///     Only manually specified ip addresses.
        /// </summary>
        public string[] IpAddresses { get; set; } = new string[0];
    }
}