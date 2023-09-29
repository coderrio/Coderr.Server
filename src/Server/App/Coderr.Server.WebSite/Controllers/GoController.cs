using System.Threading.Tasks;
using Coderr.Server.Domain.Core.Incidents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coderr.Server.WebSite.Controllers
{
    public class GoController : Controller
    {
        private IIncidentRepository _incidentRepository;

        public GoController(IIncidentRepository incidentRepository)
        {
            _incidentRepository = incidentRepository;
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> Incident(int id)
        {
            var incident = await _incidentRepository.GetAsync(id);
            return Redirect(incident.State == IncidentState.Active
                ? $"/analyze/incident/{id}"
                : $"/discover/incidents/{incident.ApplicationId}/incident/{id}");
        }
    }
}
