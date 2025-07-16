using Astra.Domain;
using Astra.Manager;
using Microsoft.AspNetCore.Mvc;

namespace Astra.Api.Controllers
{
    [ApiController]
    [Route("countries/{countryId}/[controller]")]
    public class CitiesController : ControllerBase
    {
        [HttpGet(Name = "GetCities")]
        public async Task<ActionResult<IEnumerable<City>>> Get([FromRoute] Guid countryId, [FromServices] CountryManager countryManager, CancellationToken cancellationToken)
        {
            if (!(await countryManager.ExistsAsync(countryId, cancellationToken)).Value)
            {
                ModelState.AddModelError("CountryExists", "CountryNotFound");
                return NotFound(ModelState);
            }

            var cities = await countryManager.GetCitiesFromCountryAsync(countryId, cancellationToken);
            if(cities.IsFailure)
            {
                ModelState.AddModelError("GetCitiesFromCountry", "CitiesNotFound");
                return NotFound(ModelState);
            }
            return Ok(cities.Value);
        }

        [HttpGet(Name = "CreateCity")]
        public async Task<ActionResult<IEnumerable<City>>> Post([FromRoute] Guid countryId, [FromBody] City city, [FromServices] CountryManager countryManager, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if ((await countryManager.CountryContainsCityAsync(countryId, cancellationToken)).Value)
            {
                ModelState.AddModelError("CountryContainsCity", "CountryNotFound");
                return Conflict(ModelState);
            }

            var cities = await countryManager.GetCitiesFromCountryAsync(countryId, cancellationToken);
            if(cities.IsFailure)
            {
                ModelState.AddModelError("GetCitiesFromCountry", "CitiesNotFound");
                return NotFound(ModelState);
            }
            return Ok(cities.Value);
        }
    }
}
