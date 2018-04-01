using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Api.Modules.Tagging;
using Coderr.Server.Api.Modules.Tagging.Queries;
using Coderr.Server.Domain.Modules.Tags;
using DotNetCqs;
using Coderr.Server.ReportAnalyzer.Abstractions;

namespace Coderr.Server.App.Modules.Tagging.Handlers
{
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