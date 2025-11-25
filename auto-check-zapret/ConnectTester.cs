using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Text;

namespace auto_check_zapret
{
    partial class ConnectTester
    {
        TextBox info;

        private static readonly HttpClient httpClient = new HttpClient();

        public ConnectTester(TextBox infoTextBox)
        {
            this.info = infoTextBox;
        }


        
        // Проверка доступности ресурса
        private async Task<bool> OK(string url)
        {
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3))) // Устанавливаем таймаут в 3 секунды
            {
                try
                {
                    // Проверка разрешения хоста
                    var hostEntry = Dns.GetHostEntry(new Uri(url).Host);

                    // Отправляем GET-запрос с использованием токена отмены
                    HttpResponseMessage response = await httpClient.GetAsync(url, cts.Token);
                    return response.IsSuccessStatusCode;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public async Task<bool> TestConnections(){
            info.AppendText("Подключение к discord...");
            bool discordConnection = await OK("http://discord.com");
            info.AppendText($"Результат: {discordConnection}" + Environment.NewLine);

            info.AppendText("Подключение к youtube...");
            bool youtubeConnection = await OK("https://www.youtube.com");
            info.AppendText($"Результат: {youtubeConnection}" + Environment.NewLine);

            if (discordConnection && youtubeConnection) 
                return true;
            else
                return false;
        }
           
            
    }
}
