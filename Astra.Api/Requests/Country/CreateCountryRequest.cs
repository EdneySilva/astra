namespace Astra.Api.Requests.Country
{
    public class CreateCountryRequest
    {
        public required string Code { get; set; }
        public required string Name { get; set; }

        public Domain.Country Translate()
        {
            return new Domain.Country(Name, Code);
        }
    }
}
