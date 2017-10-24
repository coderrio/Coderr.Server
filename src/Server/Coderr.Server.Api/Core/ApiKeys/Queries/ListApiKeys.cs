using DotNetCqs;

namespace codeRR.Server.Api.Core.ApiKeys.Queries
{
    /// <summary>
    ///     List all created keys
    /// </summary>
    [Message]
    public class ListApiKeys : Query<ListApiKeysResult>
    {
    }
}