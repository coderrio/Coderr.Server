using System.Linq;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using codeRR.Api.Modules.Tagging;
using codeRR.Api.Modules.Tagging.Queries;
using codeRR.App.Modules.Tagging.Domain;

namespace codeRR.App.Modules.Tagging.Handlers
{
    [Component]
    internal class GetTagsForIncidentHandler : IQueryHandler<GetTagsForIncident, TagDTO[]>
    {
        private readonly ITagsRepository _repository;

        public GetTagsForIncidentHandler(ITagsRepository repository)
        {
            _repository = repository;
        }

        public async Task<TagDTO[]> ExecuteAsync(GetTagsForIncident query)
        {
            return (await _repository.GetTagsAsync(query.IncidentId)).Select(ConvertTag).ToArray();
        }

        private TagDTO ConvertTag(Tag arg)
        {
            return new TagDTO {Name = arg.Name, OrderNumber = arg.OrderNumber};
        }
    }
}