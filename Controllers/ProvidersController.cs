using Microsoft.AspNetCore.Mvc;
using SchemeServe.Models;

namespace SchemeServe.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProvidersController : Controller
    {
        GetProviders _GetProviders;
        Individual _Individual;

        public ProvidersController(IConfiguration config)
        {
            _GetProviders = new GetProviders(config);
            _Individual = new Individual(config);
        }

        [HttpGet("{provider_id}")]
        public async Task<IActionResult> Get(string provider_id)
        {
            if (!string.IsNullOrEmpty(provider_id))
            {
                var result = await GetIndividual(provider_id);
                return Json(result);
            }
            else
            {
                return Json("Please provide provider_id");
            }
        }

        [HttpGet()]
        public async Task<IActionResult> Get()
        {
            var result = await GetProviders();
            return Json(result);
        }

        public async Task<Cqc> GetIndividual(string provider_id)
        {
            return await _Individual.ProviderAsync(provider_id);
        }

        public async Task<string> GetProviders()
        {
            return await _GetProviders.ProvidersAsync();
        }
    }
}
