
using System.Diagnostics;
using System.Net;
using System.Net;
using System.Reflection;
using System.Reflection;
using System.Text.Json;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace auto_check_zapret
{
    /// <summary>
    /// Класс для осуществления обновлений программы
    /// </summary>
    partial class Updater
    {
        string curver = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        

        public void CheckAndUpdate()
        {
            curver = curver.Substring(0, curver.Length - 2);
            using (WebClient wc = new WebClient())
            {
                if (OK())
                {

                    wc.Headers.Add(HttpRequestHeader.UserAgent, "MyApp/1.0");
                    var json = wc.DownloadString("https://api.github.com/repos/kaciel13/auto-check-zapret/releases/latest");
                    var release = JsonSerializer.Deserialize<Release>(json);

                    if (release.tag_name != curver)
                    { 
                        if (MessageBox.Show($"Вышло новое обновление. \n {curver} => {release.tag_name} \n Обновить?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            wc.DownloadFile($"https://github.com/kaciel13/auto-check-zapret/releases/download/{release.tag_name}/auto-check-zapret.exe", "auto-check-zapret.exe");

                            Cmd(@"/c taskkill /f /im auto-check-zapret.exe && timeout /t 1 && del auto-check-zapret.exe && ren app.exe auto-check-zapret.exe && auto-check-zapret.exe");
                        }
                    }
                    else
                    {
                        MessageBox.Show("У вас последняя версия");
                    }
                }
                else
                {
                    MessageBox.Show("Нет доступа к github.com");
                }
            }
        }

        private class Release
        {
            public string tag_name { get; set; }
        }


        public static bool OK()
        {
            try
            {
                Dns.GetHostEntry("github.com");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void Cmd(string comand)
        {


            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "cmd";
            psi.Arguments = comand;
            psi.UseShellExecute = false;       // обязательно false для перенаправления и скрытия окна
            psi.CreateNoWindow = true;           // не показывать консольное окно

            Process.Start(psi);

        }
    }
}
