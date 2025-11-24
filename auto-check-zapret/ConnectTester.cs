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


        

        public async Task<bool> OK(string url)
        {
            try
            {
                // Проверка разрешения хоста
                var hostEntry = Dns.GetHostEntry(new Uri(url).Host);

                // Отправляем GET-запрос
                HttpResponseMessage response = await httpClient.GetAsync(url);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false; // Если происходит ошибка, возвращаем false
            }
        }

        public async Task<bool> TestConnections(){
            info.AppendText("Подключение к discord..." + Environment.NewLine);
            bool discordConnection = await OK("http://discord.com");
            info.AppendText($"Результат: {discordConnection}" + Environment.NewLine);

            info.AppendText("Подключение к youtube..." + Environment.NewLine);
            bool youtubeConnection = await OK("https://www.youtube.com");
            info.AppendText($"Результат: {youtubeConnection}" + Environment.NewLine);

            if (discordConnection && youtubeConnection) 
                return true;
            else
                return false;
        }
           
            
    }
}
