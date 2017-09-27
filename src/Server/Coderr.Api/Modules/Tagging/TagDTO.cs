namespace codeRR.Server.Api.Modules.Tagging
{
    /// <summary>
    ///     A stack overflow tag
    /// </summary>
    public class TagDTO
    {
        /// <summary>
        ///     Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Used to sort tags before displaying them.
        /// </summary>
        public int OrderNumber { get; set; }
    }
}