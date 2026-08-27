using AutoCheckZapret.ViewModels;

namespace AutoCheckZapret.Models
{
    /// <summary>
    /// Данные приложения (настройки, выбранные версии Zapret) и другое, которые
    /// будут сохраняться в файле рядом с приложением и загружаться из него
    /// </summary>
    public class SavedApplicationData
    {
        /// <summary>
        /// Последняя выбранная версия Zapret в программе
        /// </summary>
        public ZapretVersionViewModel LastSelectedZapretVersion { get; set; }

        /// <summary>
        /// Скачанные версии Zapret (в них также будут данные о подобранных для них обходах)
        /// </summary>
        public List<ZapretVersion> DownloadedZapretVersions { get; set; }
    }
}
