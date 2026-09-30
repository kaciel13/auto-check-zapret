using System.Runtime.InteropServices;

namespace AutoCheckZapret.Helpers
{
    /// <summary>
    /// Класс, реализующий метод StrCmpLogicalW из Windows API.
    /// Этот метот используется в Проводнике Windows и применяет "логическую сортировку" к именам файлов.
    /// Таким образом, файл file9 идёт до file10, а вот при алфавитной сортировке, которая применяется в .NET
    /// по умолчанию, файлы шли бы в порядке file10, file11, file9 и так далее
    /// </summary>
    public sealed class WindowsExplorerStringComparer : IComparer<string>
    {
        // Import the native Windows function responsible for logical sorting
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern int StrCmpLogicalW(string x, string y);

        public int Compare(string x, string y)
        {
            return StrCmpLogicalW(x, y);
        }
    }
}
