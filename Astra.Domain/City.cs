namespace Astra.Domain
{
    public class City
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Province { get; set; }
        public bool Deleted { get; set; }
        public required Country Country { get; set; }

        public static City Initialize(int id)
        {
            return new City
            {
                Id = id,
                Name = string.Empty,
                Province = string.Empty,
                Country = new Country(),
                Deleted = false
            };
        }
    }
}
