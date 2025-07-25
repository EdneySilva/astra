using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace Astra.Api.Tests.Controllers
{
    public static class AuthHelper
    {
        public static async Task<HttpClient> AuthenticateAsync(this HttpClient client)
        {
            var login = new
            {
                username = "admin",
                password = "123456"
            };

            var response = await client.PostAsJsonAsync("/account", login);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<AuthTokenResponse>();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", content!.Token);
            return client;
        }

        private class AuthTokenResponse
        {
            public string Token { get; set; } = default!;
        }
    }

}
