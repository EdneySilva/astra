using Astra.Domain.Abstractions.Data;
using Astra.Domain.Events.Country;

namespace Astra.Domain
{
    public class Country : DomainObject
    {
        private ICityRepository CitiesRepository { get; set; }

        public Country()
        {

        }

        public Country(string name, string code)
        {
            Name = name;
            Code = code;
            this.AddEvent(new CountryAdded(this));
        }

        private Country(ICityRepository citiesRepository) : this()
        {
            CitiesRepository = citiesRepository;
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool Deleted { get; set; }
        public ICollection<City> Cities { get; set; }
    }
}
