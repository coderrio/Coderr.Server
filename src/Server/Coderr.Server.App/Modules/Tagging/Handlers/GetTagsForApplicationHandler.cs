using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Api.Modules.Tagging;
using Coderr.Server.Api.Modules.Tagging.Queries;
using Coderr.Server.Domain.Modules.Tags;
using DotNetCqs;
using Coderr.Server.ReportAnalyzer.Abstractions;

namespace Coderr.Server.App.Modules.Tagging.Handlers
{
    internal class GetTagsForApplicationHandler : IQueryHandler<GetTagsForApplication, TagDTO[]>
    {
        private readonly ITagsRepository _repository;

        public GetTagsForApplicationHandler(ITagsRepository repository)
        {
            _repository = repository;
        }

        public async Task<TagDTO[]> HandleAsync(IMessageContext context, GetTagsForApplication query)
        {
            return (await _repository.GetApplicationTagsAsync(query.ApplicationId)).Select(ConvertTag).ToArray();
        }

        private TagDTO ConvertTag(Tag arg)
        {
            return new TagDTO {Name = arg.Name, OrderNumber = arg.OrderNumber};
        }
    }
}