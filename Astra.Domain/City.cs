namespace Astra.Domain
{
    public class City
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required Country Country { get; set; }
    }
}
