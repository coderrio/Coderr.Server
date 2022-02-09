using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Web.Areas.Admin.Models.Partition;
using Coderr.Server.Web.Infrastructure;
using Coderr.Server.Common.App.Partitions;
using Coderr.Server.Common.Domain.Partitions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coderr.Server.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class PartitionController : Controller
    {
        private IPartitionRepository _repository;

        public PartitionController(IPartitionRepository repository)
        {
            _repository = repository;
        }


        public async Task<ActionResult> Manage(int applicationId)
        {
            var definitions  =await _repository.GetDefinitions(applicationId);
            var items = definitions.Select(x => new ListItem
            {
                Id = x.Id,
                Key = x.PartitionKey,
                Name = x.Name,
                Weight = x.Weight,
                NumberOfItems = x.NumberOfItems
            });
            var model = new ManageViewModel
            {
                ApplicationId = applicationId,
                Items = items.ToList()
            };
            return View(model);
        }

        public ActionResult Create(int applicationId)
        {
            var model = new CreateViewModel {ApplicationId = applicationId};
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var entity = new PartitionDefinition(model.ApplicationId.Value, model.Title, model.PartitionKey)
            {
                Weight = model.Weight,
                NumberOfItems = model.NumberOfItems ?? 0
            };
            await _repository.CreateAsync(entity);

            return RedirectToAction("Manage", new {applicationId = model.ApplicationId.Value});
        }

        public async Task<ActionResult> Edit(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            var model = new EditViewModel
            {
                Id = entity.Id,
                NumberOfItems = entity.NumberOfItems == 0 ? null : (int?)entity.NumberOfItems,
                Weight = entity.Weight,
                Title = entity.Name
            };
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(EditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var entity = await _repository.GetByIdAsync(model.Id);
            entity.NumberOfItems = model.NumberOfItems ?? 9;
            entity.Weight = model.Weight;
            entity.Name= model.Title;
            await _repository.UpdateAsync(entity);

            return RedirectToAction("Manage", new {applicationId = entity.ApplicationId});
        }
    }
}