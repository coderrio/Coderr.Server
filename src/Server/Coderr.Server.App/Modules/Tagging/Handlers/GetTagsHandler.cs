using System.Linq;
using System.Threading.Tasks;
using codeRR.Server.Api.Modules.Tagging;
using codeRR.Server.Api.Modules.Tagging.Queries;
using codeRR.Server.App.Modules.Tagging.Domain;
using DotNetCqs;
using Griffin.Container;

namespace codeRR.Server.App.Modules.Tagging.Handlers
{
    [Component]
    internal class GetTagsHandler : IQueryHandler<GetTags, TagDTO[]>
    {
        private readonly ITagsRepository _repository;

        public GetTagsHandler(ITagsRepository repository)
        {
            _repository = repository;
        }

        public async Task<TagDTO[]> HandleAsync(IMessageContext context, GetTags query)
        {
            return (await _repository.GetTagsAsync(query.ApplicationId, query.IncidentId)).Select(ConvertTag).ToArray();
        }

        private TagDTO ConvertTag(Tag arg)
        {
            return new TagDTO {Name = arg.Name, OrderNumber = arg.OrderNumber};
        }
    }
}