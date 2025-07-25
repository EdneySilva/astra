using Astra.Api.Requests.Country;
using Astra.Domain;
using Astra.Manager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using System.Net;

namespace Astra.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class CountriesController : ControllerBase
    {
        private readonly ILogger<CountriesController> _logger;
        private readonly ICountryManager _countryManager;

        public CountriesController(ILogger<CountriesController> logger, ICountryManager countryManager)
        {
            _logger = logger;
            _countryManager = countryManager;
        }

        [HttpGet(Name = "GetCountries")]
        [OutputCache(Duration = 600, Tags = ["countries", "country"])]
        public async Task<ActionResult<IEnumerable<Country>>> Get(CancellationToken cancellationToken)
        {
            var countries = await _countryManager.GetAllAsync(cancellationToken);
            if (countries.IsFailure)
            {
                return new StatusCodeResult((int)HttpStatusCode.FailedDependency);
            }
            if (!countries.Value!.Any())
                return NotFound(Enumerable.Empty<Country>());
            return Ok(countries.Value);
        }

        [HttpGet("{name}", Name = "GetCountryByName")]
        [OutputCache(Duration = 600, Tags = ["country"], VaryByRouteValueNames = [ "name" ])]
        public async Task<ActionResult<IEnumerable<Country>>> Get([FromRoute] string name, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var findCountryResult = await _countryManager.FindByNameAsync(name, cancellationToken);
            if (findCountryResult.IsFailure)
            {
                ModelState.AddModelError("FindByName", findCountryResult.Error!);
                return BadRequest(findCountryResult);
            }
            if (findCountryResult.Value is null)
                return NotFound();
            return Ok(findCountryResult.Value);
        }

        [HttpPost(Name = "CreateCountry")]
        public async Task<ActionResult<Country>> Post([FromBody] CreateCountryRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var country = request.Translate();
            if ((await _countryManager.ExistsAsync(country, cancellationToken)).Value)
            {
                ModelState.AddModelError("Exists", "CountryAlreadyExists");
                return Conflict(ModelState);
            }
            var addCountryResult = await _countryManager.AddCountryAsync(country, cancellationToken);
            if (addCountryResult.IsFailure)
            {
                ModelState.AddModelError("AddCountry", addCountryResult.Error!);
                return UnprocessableEntity(ModelState);
            }
            return Created("/countries/{name}", addCountryResult.Value);
        }

        [HttpPatch("{id}", Name = "CountryPartialUpdate")]
        public async Task<ActionResult<Country>> Patch([FromRoute] int id, [FromBody] JsonPatchDocument<Country> operations, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var findCountryAddResult = await _countryManager.FindByIdAsync(id, cancellationToken);
            if (findCountryAddResult.IsFailure)
            {
                ModelState.AddModelError("FindById", findCountryAddResult.Error!);
                return BadRequest(ModelState);
            }
            if (findCountryAddResult.Value is null)
            {
                ModelState.AddModelError("FindById", "CountryNotFound");
                return NotFound(ModelState);
            }

            operations.ApplyTo(findCountryAddResult.Value!);
            await _countryManager.UpdateCountryAsync(findCountryAddResult.Value!, cancellationToken);
            return Ok(findCountryAddResult.Value);
        }

        [HttpDelete(Name = "DeleteCountry")]
        public async Task<ActionResult<Country>> Delete([FromBody] Country country, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!(await _countryManager.ExistsAsync(country, cancellationToken)).Value)
            {
                ModelState.AddModelError("Exists", "CountryNotFound");
                return NotFound(ModelState);
            }
            var result = await _countryManager.DeleteCountryAsync(country, cancellationToken);
            if (result.IsFailure)
            {
                ModelState.AddModelError("DeleteCountry", result.Error!);
                return UnprocessableEntity(ModelState);
            }
            return Ok(result.Value);
        }
    }
}
