using System.Collections.Generic;

namespace Coderr.Server.Common.AzureDevOps.App.Clients
{
    public class ClassificationNode
    {
        public List<ClassificationNode> Children { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }
}