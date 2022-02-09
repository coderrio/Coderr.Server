namespace Coderr.Server.Web.Areas.Admin.Models.Partition
{
    public class ListItem
    {
        public int Id { get; set; }
            
        public string Name { get; set; }
        public string Key { get; set; }
        public int Weight { get; set; }
        public int NumberOfItems { get; set; }
    }
}