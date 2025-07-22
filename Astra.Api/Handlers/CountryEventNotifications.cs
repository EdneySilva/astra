using Astra.Domain.Events.Country;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;

namespace Astra.Api.Handlers
{
    public class CountryEventNotifications : INotificationHandler<CountryAdded>
    {
        private readonly IOutputCacheStore _outputCacheStore;

        public CountryEventNotifications(IOutputCacheStore outputCacheStore)
        {
            _outputCacheStore = outputCacheStore;
        }

        public async Task Handle(CountryAdded notification, CancellationToken cancellationToken)
        {
            await _outputCacheStore.EvictByTagAsync("countries", cancellationToken);
            await _outputCacheStore.EvictByTagAsync("country", cancellationToken);
        }
    }
}
