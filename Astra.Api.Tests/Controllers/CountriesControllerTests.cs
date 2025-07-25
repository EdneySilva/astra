using Astra.Api.Tests.Factories;
using Astra.Domain;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http.Json;
using System.Net;
using Astra.Api.Requests.Country;
using FluentAssertions;

namespace Astra.Api.Tests.Controllers
{

    public class CountriesControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public CountriesControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]        
        public async Task Post_Creates_New_Country()
        {
            var request = new CreateCountryRequest
            {
                Name = "Testland",
                Code = "TST"
            };

            var client = await _client.AuthenticateAsync();
            var response = await client.PostAsJsonAsync("/countries", request);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await response.Content.ReadFromJsonAsync<Country>();
            result.Should().NotBeNull();
            result!.Name.Should().Be("Testland");
            result.Code.Should().Be("TST");
        }

        [Fact]
        public async Task Get_Returns_List_Of_Countries()
        {
            var response = await _client.GetAsync("/countries");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var countries = await response.Content.ReadFromJsonAsync<IEnumerable<Country>>();
            countries.Should().NotBeNull();
            countries.Should().Contain(c => c.Name == "Testland");
        }
    }

}
