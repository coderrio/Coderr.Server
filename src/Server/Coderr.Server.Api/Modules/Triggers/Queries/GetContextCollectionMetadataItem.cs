namespace codeRR.Server.Api.Modules.Triggers.Queries
{
    /// <summary>
    ///     Result item for <see cref="GetContextCollectionMetadata" />
    /// </summary>
    public class GetContextCollectionMetadataItem
    {
        /// <summary>
        ///     Context name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Property names
        /// </summary>
        public string[] Properties { get; set; }
    }
}