using Astra.Domain;

namespace Astra.Api.Requests.Country
{
    public class CreateCityRequest
    {
        public required string Name { get; set; }
        public required string Province { get; set; }

        public City Translate()
        {
            return new City
            {
                Country = new Domain.Country(string.Empty, string.Empty),
                Name = Name,
                Province = Province
            };
        }
    }
}
