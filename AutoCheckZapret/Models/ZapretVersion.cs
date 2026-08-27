namespace AutoCheckZapret.Models
{
    /// <summary>
    /// Модель версии Zapret
    /// </summary>
    public class ZapretVersion
    {
        /// <summary>
        /// Номер версии, например "1.9.7b"
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Ссылка для скачивания версии с официального репозитория
        /// </summary>
        public string DownloadUrl { get; set; }

        /// <summary>
        /// Скачана ли версия у пользователя
        /// </summary>
        public bool IsDownloaded { get; set; }

        /// <summary>
        /// Название .bat-файла с обходом, который работает для данной версии Zapret
        /// </summary>
        public string BypassMethodName { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="number">Номер версии</param>
        /// <param name="downloadUrl">Ссылка для скачивания версии с официального репозитория</param>
        ///  <param name="isDownloaded">Значение скачана ли версия</param>
        public ZapretVersion(string number, string downloadUrl, bool isDownloaded = false)
        {
            Number = number;
            DownloadUrl = downloadUrl;
            IsDownloaded = isDownloaded;
            BypassMethodName = string.Empty;
        }
    }
}