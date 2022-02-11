using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Kalantyr.RemoteControl.WPF.Client
{
    public class RemoteControlClient: IHealthCheck, IRemoteControlClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private const string ContentType = "application/json";

        public RemoteControlClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }


        protected async Task<T> Get<T>(string path, CancellationToken cancellationToken)
        {
            return await SendAsync<T>(HttpMethod.Get, path, null, cancellationToken);
        }

        protected async Task<T> Post<T>(string path, string body, CancellationToken cancellationToken)
        {
            return await SendAsync<T>(HttpMethod.Post, path, body, cancellationToken);
        }

        protected async Task<T> SendAsync<T>(HttpMethod method, string path, string body, CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.CreateClient(GetType().Name);

            var uri = string.Join("/", httpClient.BaseAddress.AbsoluteUri.TrimEnd('/'), path.TrimStart('/'));
            using var requestMessage = new HttpRequestMessage(method, new Uri(uri, UriKind.Absolute));

            if (Debugger.IsAttached)
            {
                Debug.WriteLine(method + " " + httpClient.BaseAddress + path);
                if (method != HttpMethod.Get)
                    Debug.WriteLine(body);
            }

            if (method != HttpMethod.Get)
                requestMessage.Content = new StringContent(body, Encoding.UTF8, ContentType);

            requestMessage.Headers.Add("Accept", ContentType);

            using var result = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead, cancellationToken);
            return await FromResponse<T>(result);
        }

        internal async Task<T> FromResponse<T>(HttpResponseMessage result)
        {
            var response = await result.Content.ReadAsStringAsync();

            switch (result.StatusCode)
            {
                case HttpStatusCode.OK:

                    if (string.IsNullOrWhiteSpace(response))
                        return default;

                    if (typeof(T) == typeof(string))
                        return (T)(object)response;

                    return JsonSerializer.Deserialize<T>(response);

                case HttpStatusCode.NoContent:
                    return default;

                case HttpStatusCode.Unauthorized:
                case HttpStatusCode.Forbidden:
                    throw new Exception("Доступ запрещён");

                default:
                    if (string.IsNullOrWhiteSpace(response))
                        throw new Exception($"HTTP {(int)result.StatusCode} {result.StatusCode}");
                    else
                        throw new Exception($"HTTP {(int)result.StatusCode} {result.StatusCode}", new Exception(response));
            }
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                var s = await Get<Version>("check/version", cancellationToken);
                s.Equals(null);

                return HealthCheckResult.Healthy(nameof(RemoteControlClient));
            }
            catch (Exception e)
            {
                return HealthCheckResult.Unhealthy(nameof(RemoteControlClient), e);
            }
        }

        public async Task<TimeSpan?> GetPowerOffAsync(CancellationToken cancellationToken)
        {
            return await Get<TimeSpan?>("power/off", cancellationToken);
        }

        public async Task PowerOffAsync(TimeSpan delay, CancellationToken cancellationToken)
        {
            await Post<bool>("power/off", JsonSerializer.Serialize(delay), cancellationToken);
        }

        public async Task CancelPowerOffAsync(CancellationToken cancellationToken)
        {
            await Post<bool>("power/off", JsonSerializer.Serialize(TimeSpan.Zero), cancellationToken);
        }
    }
}
