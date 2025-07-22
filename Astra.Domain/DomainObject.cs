using Astra.Domain.Events;

namespace Astra.Domain
{
    public class DomainObject
    {
        private List<DomainEvent> events = new List<DomainEvent>();

        public IEnumerable<DomainEvent> Notify()
        {
            var array = events.ToArray();
            events.Clear();
            return array;
        }

        protected void AddEvent(DomainEvent domainEvent)
        {
            domainEvent.EventId = Guid.NewGuid();
            events.Add(domainEvent);
        }
    }
}
