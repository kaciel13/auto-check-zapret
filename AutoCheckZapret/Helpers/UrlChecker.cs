using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace AutoCheckZapret.Helpers
{
    public static class UrlChecker
    {
        private static readonly SocketsHttpHandler handler = new()
        {
            UseProxy = false,
            AllowAutoRedirect = false,

            // Не использовать старые соединения
            PooledConnectionLifetime = TimeSpan.Zero,
            PooledConnectionIdleTimeout = TimeSpan.Zero
        };

        private static readonly HttpClient client = new(handler);

        public static async Task<bool> IsUrlRespondingAsync(
            string url,
            int timeoutSeconds = 10)
        {
            try
            {
                using var request = new HttpRequestMessage(
                    HttpMethod.Get,
                    url)
                {
                    // Принудительно используем HTTP/1.1
                    Version = HttpVersion.Version11,
                    VersionPolicy = HttpVersionPolicy.RequestVersionExact
                };

                // Закрыть соединение после этого запроса
                request.Headers.ConnectionClose = true;

                request.Headers.CacheControl = new CacheControlHeaderValue
                {
                    NoCache = true,
                    NoStore = true,
                    MustRevalidate = true,
                    MaxAge = TimeSpan.Zero
                };

                request.Headers.Pragma.ParseAdd("no-cache");

                using var cts = new CancellationTokenSource(
                    TimeSpan.FromSeconds(timeoutSeconds));

                Debug.WriteLine($"Тест: {url}");

                using var response = await client.SendAsync(
                    request,
                    HttpCompletionOption.ResponseHeadersRead,
                    cts.Token);

                Debug.WriteLine(
                    $"{url} Ответ: {(int)response.StatusCode} {response.StatusCode}");

                foreach (var header in response.Headers)
                {
                    Debug.WriteLine(
                        $"{header.Key}: {string.Join(", ", header.Value)}");
                }

                foreach (var header in response.Content.Headers)
                {
                    Debug.WriteLine(
                        $"{header.Key}: {string.Join(", ", header.Value)}");
                }

                // Любой ответ сервера считается валидным
                return true;
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine($"Тайм-аут: {url}");
                return false;
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"Ошибка HTTP: {ex.Message}");
                return false;
            }
            catch (UriFormatException ex)
            {
                Debug.WriteLine($"Некорректный URL: {ex.Message}");
                return false;
            }
        }
    }
}
