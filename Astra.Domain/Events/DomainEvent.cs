using MediatR;

namespace Astra.Domain.Events
{
    public class DomainEvent : INotification
    {
        public Guid EventId { get; set; }

        public DateTime At { get; } = DateTime.UtcNow;
    }
}
