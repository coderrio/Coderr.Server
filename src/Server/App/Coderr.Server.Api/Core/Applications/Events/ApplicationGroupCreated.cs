namespace Coderr.Server.Api.Core.Applications.Events
{
    public class ApplicationGroupCreated
    {
        public ApplicationGroupCreated(int id, string name, int createdById)
        {
            Id = id;
            Name = name;
            CreatedById = createdById;
        }

        protected ApplicationGroupCreated()
        {

        }

        public int CreatedById { get; private set; }
        public int Id { get; private set; }
        public string Name { get; private set; }
    }
}