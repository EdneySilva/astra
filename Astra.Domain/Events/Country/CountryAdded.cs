namespace Astra.Domain.Events.Country
{
    public class CountryAdded : DomainEvent
    {
        private readonly Domain.Country _ref;

        public CountryAdded(Domain.Country @ref)
        {
            _ref = @ref;
        }

        public int Id { get => _ref?.Id ?? 0; }
        public string Name { get => _ref?.Name ?? string.Empty; }
        public string Code { get => _ref?.Code ?? string.Empty; }
    }
}
