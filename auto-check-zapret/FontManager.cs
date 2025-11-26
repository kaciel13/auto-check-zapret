using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace auto_check_zapret
{
    partial class FontManager
    {
        public void InstallCustomFont()
        {
            string pathToFont = ExtractFontResource();
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
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

                    writer.WriteLine($"copy \"{pathToFont}\" \"C:\\Windows\\Fonts\\\"");
                    Thread.Sleep(100);
                    writer.WriteLine("Y"); // Если шрифт уже есть
                    Thread.Sleep(100);
                    writer.WriteLine("reg add \"HKLM\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Fonts\" /v \"LCD5x8HRU.ttf (TrueType)\" /t REG_SZ /d \"LCD5x8HRU.ttf\" /f"); // Ваши команды
                    Thread.Sleep(1000);
                }
            }

            process.WaitForExit();


        }

        public string ExtractFontResource()
        {
            string outputPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "LCD5x8HRU.ttf");
            using (FileStream fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
            {
                
                byte[] fontData = Properties.Resources.LCD5x8HRU; 
                fileStream.Write(fontData, 0, fontData.Length);
            }

            return outputPath;
        }
    }
}
