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
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                WorkingDirectory = path,
                RedirectStandardInput = true,
                //RedirectStandardOutput = true,
                UseShellExecute = false, // Это должно быть false для перенаправления
                CreateNoWindow = false, // Можно установить true, если не нужно отображать окно

            };

            Process process = new Process() { StartInfo = startInfo };
            process.Start();

            Thread.Sleep(1000);
            using (var writer = process.StandardInput)
            {
                if (writer.BaseStream.CanWrite)
                {
                    info.AppendText($"Запуск обхода... Пункт {choice}");
                    writer.WriteLine("service.bat admin"); // Пример команды
                    Thread.Sleep(200);
                    writer.WriteLine("1"); // Переход в каталог
                    Thread.Sleep(500);
                    writer.WriteLine(choice.ToString());
                    Thread.Sleep(3000);
                    writer.WriteLine(""); // Ваши команды
                    writer.WriteLine("^C"); // Ваши команды
                    writer.WriteLine("Y");
                }
            }

            process.WaitForExit();
        }

        public void ZapretRemove(string path)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                WorkingDirectory = path,
                RedirectStandardInput = true,
                //RedirectStandardOutput = true,
                UseShellExecute = false, // Это должно быть false для перенаправления
                CreateNoWindow = false, // Можно установить true, если не нужно отображать окно

            };

            Process process = new Process() { StartInfo = startInfo };
            process.Start();

            Thread.Sleep(1000);
            using (var writer = process.StandardInput)
            {
                if (writer.BaseStream.CanWrite)
                {
                    info.AppendText($"Выключение запрет...");
                    writer.WriteLine("service.bat admin"); // Пример команды
                    Thread.Sleep(200);
                    writer.WriteLine("2"); // Переход в каталог
                    Thread.Sleep(2000);
                    writer.WriteLine(""); // Ваши команды
                    writer.WriteLine("^C"); // Ваши команды
                    writer.WriteLine("Y");
                }
            }

            process.WaitForExit();
        }
    }
}
