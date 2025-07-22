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

        public CountriesController(ILogger<CountriesController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetCountries")]
        [OutputCache(Duration = 600, Tags = ["countries", "country"])]
        public async Task<ActionResult<IEnumerable<Country>>> Get([FromServices] CountryManager countryManager, CancellationToken cancellationToken)
        {
            var countries = await countryManager.GetAllAsync(cancellationToken);
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
        public async Task<ActionResult<IEnumerable<Country>>> Get([FromRoute] string name, [FromServices] CountryManager countryManager, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var findCountryResult = await countryManager.FindByNameAsync(name, cancellationToken);
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
        public async Task<ActionResult<Country>> Post([FromBody] CreateCountryRequest request, [FromServices] CountryManager countryManager, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var country = request.Translate();
            if ((await countryManager.ExistsAsync(country, cancellationToken)).Value)
            {
                ModelState.AddModelError("Exists", "CountryAlreadyExists");
                return Conflict(ModelState);
            }
            var addCountryResult = await countryManager.AddCountryAsync(country, cancellationToken);
            if (addCountryResult.IsFailure)
            {
                ModelState.AddModelError("AddCountry", addCountryResult.Error!);
                return UnprocessableEntity(ModelState);
            }
            return Created("/countries/{name}", addCountryResult.Value);
        }

        [HttpPatch("{id}", Name = "CountryPartialUpdate")]
        public async Task<ActionResult<Country>> Patch([FromRoute] int id, [FromBody] JsonPatchDocument<Country> operations, [FromServices] CountryManager countryManager, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var findCountryAddResult = await countryManager.FindByIdAsync(id, cancellationToken);
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
            await countryManager.UpdateCountryAsync(findCountryAddResult.Value!, cancellationToken);
            return Ok(findCountryAddResult.Value);
        }

        [HttpDelete(Name = "DeleteCountry")]
        public async Task<ActionResult<Country>> Delete([FromBody] Country country, [FromServices] CountryManager countryManager, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!(await countryManager.ExistsAsync(country, cancellationToken)).Value)
            {
                ModelState.AddModelError("Exists", "CountryNotFound");
                return NotFound(ModelState);
            }
            var result = await countryManager.DeleteCountryAsync(country, cancellationToken);
            if (result.IsFailure)
            {
                ModelState.AddModelError("DeleteCountry", result.Error!);
                return UnprocessableEntity(ModelState);
            }
            return Ok(result.Value);
        }
    }
}
