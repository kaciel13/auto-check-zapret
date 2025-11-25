using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml.Serialization;

namespace auto_check_zapret
{
    partial class ZapretService
    {
        TextBox info;

        public ZapretService(TextBox infoTextBox)
        {
            info = infoTextBox;
        }


        public void BypassStart(string path, int choice)
        {
            string version = path.Replace("zaprets\\zapret-discord-youtube-","");
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                WorkingDirectory = path,
                RedirectStandardInput = true,
                //RedirectStandardOutput = true,
                UseShellExecute = false, // Это должно быть false для перенаправления
                CreateNoWindow = true, // Можно установить true, если не нужно отображать окно

            };

            Process process = new Process() { StartInfo = startInfo };
            process.Start();

            Thread.Sleep(1000);
            using (var writer = process.StandardInput)
            {
                if (writer.BaseStream.CanWrite)
                {
                    info.AppendText($"Запуск обхода №{choice} | Версия zapret: {version}" + Environment.NewLine);
                    writer.WriteLine("service.bat admin");
                    Thread.Sleep(200);
                    writer.WriteLine("1"); 
                    Thread.Sleep(200);
                    writer.WriteLine(choice.ToString());
                    Thread.Sleep(200);
                    writer.WriteLine(""); 
                    writer.WriteLine("^C"); 
                    writer.WriteLine("Y");
                }
            }
            info.AppendText($"Обход запущен" + Environment.NewLine);
            process.WaitForExit();
        }

        public void ZapretRemove(string path, string version)
        {
            VersionChecker checker = new VersionChecker();
            if (checker.Check(version) == "new")
            {
                ProcessStartInfo startInfo = new ProcessStartInfo()
                {
                    FileName = "cmd.exe",
                    WorkingDirectory = path,
                    RedirectStandardInput = true,
                    //RedirectStandardOutput = true,
                    UseShellExecute = false, // Это должно быть false для перенаправления
                    CreateNoWindow = true, // Можно установить true, если не нужно отображать окно

                };

                Process process = new Process() { StartInfo = startInfo };
                process.Start();

                Thread.Sleep(1000);
                using (var writer = process.StandardInput)
                {
                    if (writer.BaseStream.CanWrite)
                    {
                        info.AppendText($"Отключение zapret..." + Environment.NewLine);
                        writer.WriteLine("service.bat admin"); // Пример команды
                        Thread.Sleep(200);
                        writer.WriteLine("2"); // Переход в каталог
                        Thread.Sleep(200);
                        writer.WriteLine(""); // Ваши команды
                        writer.WriteLine("^C"); // Ваши команды
                        writer.WriteLine("Y");
                    }
                }
                info.AppendText($"zapret отключен" + Environment.NewLine);
                process.WaitForExit();
            }
            else
            {
                info.AppendText($"Версия не поддерживается");
            }
        }

        

    }
}

