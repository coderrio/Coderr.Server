namespace Coderr.Server.Api.Core.Applications.Commands
{
    [Command]
    public class UpdateRoles
    {
        public int UserToUpdate { get; set; }
        public int ApplicationId { get; set; }

        public string[] Roles { get; set; }
    }
}