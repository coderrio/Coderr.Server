using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;

namespace Coderr.Server.Common.AzureDevOps.App.Clients
{
    public static class PatchDocumentExtensions
    {
        public static void Add(this JsonPatchDocument document, string fieldPath, string value)
        {
            if (!fieldPath.StartsWith("/fields/"))
                fieldPath = $"/fields/{fieldPath}";

            document.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = fieldPath,
                    Value = value
                }
            );
        }
    }
}