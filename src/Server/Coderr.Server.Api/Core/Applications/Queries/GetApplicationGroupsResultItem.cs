namespace Coderr.Server.Api.Core.Applications.Queries
{
    public class GetApplicationGroupsResultItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int[] ApplicationIds { get; set; }
    }
}