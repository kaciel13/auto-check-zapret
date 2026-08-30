using System.Diagnostics;
using System.Net.Http;

namespace AutoCheckZapret.Helpers
{
    /// <summary>
    /// Служебный класс для проверки доступности URL-адресов
    /// </summary>
    public static class UrlChecker
    {
        private static readonly HttpClient client = new HttpClient();

        /// <summary>
        /// Отвечает ли запрашиваемый URL-адрес
        /// </summary>
        /// <param name="url">URL-адрес</param>
        /// <param name="timeoutSeconds">Таймаут в секундах</param>
        /// <returns>true - запрашиваемый ресурс ответил, false - запрашиваемый ресурс не отвечает</returns>
        public static async Task<bool> IsUrlRespondingAsync(string url, int timeoutSeconds = 10)
        {
            try
            {
                // Use a cancellation token to enforce a quick timeout
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));

                Debug.WriteLine("Тест " + url);
                
                // Отправляем Get запрос хосту
                HttpResponseMessage response = await client.GetAsync(url, cts.Token);

                // Возвращаем статус хоста
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                // Fails if there is a timeout, DNS error, 404, or network issue
                return false;
            }
        }
    }
}
