using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;
using System.Timers;
namespace auto_check_zapret
{
    partial class BypassTester
    {
        private Dictionary<string, int> countOfBypass = new Dictionary<string, int>
        {
            { "1.6.0", 5 },
            { "1.6.1", 3 },
            { "1.6.4", 2 },
            { "1.6.5", 1 },
            { "1.6.6", 4 },
            { "1.7.1", 6 },
            { "1.7.2", 2 },
            { "1.7.2b", 12 },
            { "1.8.0", 12 },
            { "1.8.1", 14 },
            { "1.8.2", 14 },
            { "1.8.3", 14 },
            { "1.8.4", 14 },
            { "1.8.5", 15 },
            { "1.9.0", 17 },
            { "1.9.0b", 3 }
        };

        TextBox info;
        ProgressBar progress;
        public BypassTester(TextBox infoTextBox, ProgressBar _progress)
        {
            info = infoTextBox;
            progress = _progress;
        }




        public async Task<List<int>> BypassTest(string path, string version, bool autoMode = false)
        {
            progress.Value = 0;
            try
            {
                GetCountOfBypass(version);
            }
            catch (ArgumentException e)
            {
                info.AppendText(e.Message + Environment.NewLine);
                
            }
            int count = GetCountOfBypass(version);
            info.AppendText($"Версия {version}. Кол-во вариантов обхода: {count}" + Environment.NewLine);
            int progressSpan = 95 / count;
            List<int> trueChoice = new List<int>();

            try
            {
                Stopwatch timer = new Stopwatch();
                info.AppendText($"Запускаю тестирование... Auto mode: {autoMode}" + Environment.NewLine);
                timer.Start();
                progress.Value = 5;
                ConnectTester connectTester = new ConnectTester(info);
                ZapretService zapretService = new ZapretService(info);
                
                

                for (int i = 1; i <= count; i++)
                {
             
                    zapretService.BypassStart(path, i);
                    
                    if (await connectTester.TestConnections())
                    {
                        if(autoMode == true) {
                            info.AppendText($"Тестирование заверешено. Найден рабочий обход №{i}");
                            progress.Value = 100;
                            return trueChoice;
                        }
                        trueChoice.Add(i);
                    };
                    zapretService.ZapretRemove(path, version);
                    progress.Value += progressSpan;
                }
                progress.Value = 100;
                timer.Stop();
                info.AppendText($"Тестирование завершено. Время тестирования: {timer.Elapsed.Minutes}:{timer.Elapsed.Seconds}:{timer.Elapsed.Milliseconds}" + Environment.NewLine);
                return trueChoice;
            }
            catch (Exception e)
            {
                progress.Value = 0;
                info.AppendText($"Ошибка тестирования: {e.Message}" + Environment.NewLine);
                return trueChoice;
            }



        }



        private int GetCountOfBypass(string version)
        {
            if (countOfBypass.TryGetValue(version, out int count))
            {
                return count;
            }
            else
            {
                throw new ArgumentException("Версия не поддерживается: " + version);
            }
        }
    }
}
