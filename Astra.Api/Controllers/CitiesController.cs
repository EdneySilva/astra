using Astra.Api.Requests.Country;
using Astra.Data.GraphQl;
using Astra.Domain;
using Astra.Manager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Astra.Api.Controllers
{
    [ApiController]
    [Route("countries/{countryId}/[controller]")]
    [Authorize]
    public class CitiesController : ControllerBase
    {
        [HttpGet(Name = "GetCities")]
        [OutputCache(Duration = 600, Tags = ["country", "city"])]
        public async Task<ActionResult<IEnumerable<City>>> Get([FromRoute] int countryId, [FromServices] IQueryable<Country> queryable, [FromServices] CountryManager countryManager, CancellationToken cancellationToken)
        {
            var cities = await countryManager.GetCitiesFromCountryAsync(countryId, cancellationToken);
            if (cities.IsFailure)
            {
                ModelState.AddModelError("GetCitiesFromCountry", "CitiesNotFound");
                return NotFound(ModelState);
            }
            return Ok(cities.Value);
        }

        [HttpGet("{id}", Name = "GetCityById")]
        [OutputCache(Duration = 600, Tags = ["country", "city"])]
        public async Task<ActionResult<IEnumerable<City>>> GetById([FromRoute] int countryId, [FromRoute] int id, [FromServices] CountryManager countryManager, CancellationToken cancellationToken)
        {
            if (!(await countryManager.ExistsAsync(countryId, cancellationToken)).Value)
            {
                ModelState.AddModelError("CountryExists", "CountryNotFound");
                return NotFound(ModelState);
            }

            var cities = await countryManager.FindCityByIdAsync(countryId, id, cancellationToken);
            if(cities.IsFailure)
            {
                ModelState.AddModelError("FindCityByIdAsync", "CitiesNotFound");
                return NotFound(ModelState);
            }
            return Ok(cities.Value);
        }

        [HttpPost(Name = "CreateCity")]
        public async Task<ActionResult<City>> Post([FromRoute] int countryId, [FromBody] CreateCityRequest request, [FromServices] CountryManager countryManager, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var city = request.Translate();
            var result = (await countryManager.CountryContainsCityAsync(countryId, city, cancellationToken));
            if(result.IsFailure)
            {
                ModelState.AddModelError("CountryContainsCity", result.Error!);
                if (result.Error!.Contains("NotFound"))
                    return NotFound(ModelState);
                return UnprocessableEntity(ModelState);
            }
            if (result.Value)
            {
                ModelState.AddModelError("CountryContainsCity", "CountryContainsCity");
                return Conflict(ModelState);
            }
            var addCityResult = await countryManager.AddCityAsync(countryId, city, cancellationToken);
            if (addCityResult.IsFailure)
            {
                ModelState.AddModelError("AddCity", addCityResult.Error!);
                return UnprocessableEntity(ModelState);
            }
            var uri = new Uri(this.Request.GetDisplayUrl());
            var url = uri.Scheme + "://" + (uri.IsLoopback ? ($"localhost:{uri.Port}") : uri.DnsSafeHost);
            return Created($"{uri}/countries/{addCityResult.Value!.Name}", addCityResult.Value);
        }

        [HttpDelete("{id}", Name = "Delete")]
        public async Task<ActionResult<City>> Delete([FromRoute] int countryId, [FromRoute] int id, [FromServices] CountryManager countryManager, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var addCityResult = await countryManager.RemoveCityAsync(countryId, City.Initialize(id), cancellationToken);
            if (addCityResult.IsFailure)
            {
                ModelState.AddModelError("AddCity", addCityResult.Error!);
                return UnprocessableEntity(ModelState);
            }
            var uri = new Uri(this.Request.GetDisplayUrl());
            var url = uri.Scheme + "://" + (uri.IsLoopback ? ($"localhost:{uri.Port}") : uri.DnsSafeHost);
            return Created($"{uri}/countries/{addCityResult.Value!.Name}", addCityResult.Value);
        }
    }
}
