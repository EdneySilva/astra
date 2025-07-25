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
        private readonly ICountryManager _countryManager;

        public CitiesController(ICountryManager countryManager)
        {
            _countryManager = countryManager;    
        }

        [HttpGet(Name = "GetCities")]
        [OutputCache(Duration = 600, Tags = ["country", "city"])]
        public async Task<ActionResult<IEnumerable<City>>> Get([FromRoute] int countryId, CancellationToken cancellationToken)
        {
            var cities = await _countryManager.GetCitiesFromCountryAsync(countryId, cancellationToken);
            if (cities.IsFailure)
            {
                ModelState.AddModelError("GetCitiesFromCountry", "CitiesNotFound");
                return NotFound(ModelState);
            }
            return Ok(cities.Value);
        }

        [HttpGet("{id}", Name = "GetCityById")]
        [OutputCache(Duration = 600, Tags = ["country", "city"])]
        public async Task<ActionResult<IEnumerable<City>>> GetById([FromRoute] int countryId, [FromRoute] int id, CancellationToken cancellationToken)
        {
            if (!(await _countryManager.ExistsAsync(countryId, cancellationToken)).Value)
            {
                ModelState.AddModelError("CountryExists", "CountryNotFound");
                return NotFound(ModelState);
            }

            var cities = await _countryManager.FindCityByIdAsync(countryId, id, cancellationToken);
            if(cities.IsFailure)
            {
                ModelState.AddModelError("FindCityByIdAsync", "CitiesNotFound");
                return NotFound(ModelState);
            }
            return Ok(cities.Value);
        }

        [HttpPost(Name = "CreateCity")]
        public async Task<ActionResult<City>> Post([FromRoute] int countryId, [FromBody] CreateCityRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var city = request.Translate();
            var result = (await _countryManager.CountryContainsCityAsync(countryId, city, cancellationToken));
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
            var addCityResult = await _countryManager.AddCityAsync(countryId, city, cancellationToken);
            if (addCityResult.IsFailure)
            {
                ModelState.AddModelError("AddCity", addCityResult.Error!);
                return UnprocessableEntity(ModelState);
            }
            var uri = new Uri(this.Request.GetDisplayUrl());
            var url = uri.Scheme + "://" + (uri.IsLoopback ? ($"localhost:{uri.Port}") : uri.DnsSafeHost);
            return Created($"{url}/countries/{addCityResult.Value!.Name}", new City 
            {
                Name = addCityResult.Value.Name,
                Id = addCityResult.Value.Id,
                Deleted = addCityResult.Value.Deleted,
                Province = addCityResult.Value.Province,
                Country = new Country
                {
                    Name = addCityResult.Value.Country.Name,
                    Deleted = addCityResult.Value.Country.Deleted,
                    Id = addCityResult.Value.Country.Id,
                    Code = addCityResult.Value.Country.Code
                }
            });
        }

        [HttpDelete("{id}", Name = "Delete")]
        public async Task<ActionResult<City>> Delete([FromRoute] int countryId, [FromRoute] int id, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var addCityResult = await _countryManager.RemoveCityAsync(countryId, City.Initialize(id), cancellationToken);
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
