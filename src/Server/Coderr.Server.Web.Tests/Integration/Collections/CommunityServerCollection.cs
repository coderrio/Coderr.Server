using codeRR.Server.Web.Tests.Integration.Fixtures;
using Xunit;

namespace codeRR.Server.Web.Tests.Integration.Collections
{
    [CollectionDefinition("CommunityServerCollection")]
    public class CommunityServerCollection : ICollectionFixture<CommunityServerFixture>
    {
    }
}
