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
        /// Конструктор
        /// </summary>
        /// <param name="number">Номер версии</param>
        /// <param name="downloadUrl">Ссылка для скачивания версии с официального репозитория</param>
        public ZapretVersion(string number, string downloadUrl)
        {
            Number = number;
            DownloadUrl = downloadUrl;
        }
    }
}
