namespace Astra.Domain
{
    public class Country
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool Deleted { get; set; }
        public ICollection<City> Cities { get; set; }
    }
}
