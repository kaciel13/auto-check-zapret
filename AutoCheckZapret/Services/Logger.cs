using AutoCheckZapret.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using System.Windows.Media;

public class Logger : INotifyPropertyChanged
{
    private readonly LogDocument _logDocument;

    private static readonly SolidColorBrush InfoBrush = new SolidColorBrush(Color.FromRgb(0xFE, 0xFE, 0xFE));
    private static readonly SolidColorBrush ErrorBrush = new SolidColorBrush(Color.FromRgb(0xFF, 0x4A, 0x4A));
    private static readonly SolidColorBrush SuccessBrush = new SolidColorBrush(Color.FromRgb(0x83, 0xFF, 0x87));

    public FlowDocument LogDocument => _logDocument.Document;

    public Logger()
    {
        _logDocument = new LogDocument(new FlowDocument());
    }

    /// <summary>
    /// Добавить сообщение с информацией (белый текст)
    /// </summary>
    /// <param name="message">Текст сообщения</param>
    /// <param name="newLine">Вывести сообщение с новой строки</param>
    public void AddInfo(string message, bool newLine = true) => AppendMessage(message, InfoBrush, newLine);

    /// <summary>
    /// Добавить сообщение с ошибкой (красный текст)
    /// </summary>
    /// <param name="message">Текст сообщения</param>
    /// <param name="newLine">Вывести сообщение с новой строки</param>
    public void AddError(string message, bool newLine = true) => AppendMessage(message, ErrorBrush, newLine);

    /// <summary>
    /// Добавить сообщение об успехе (зелёный текст)
    /// </summary>
    /// <param name="message">Текст сообщения</param>
    /// <param name="newLine">Вывести сообщение с новой строки</param>
    public void AddSuccess(string message, bool newLine = true) => AppendMessage(message, SuccessBrush, newLine);

    private void AppendMessage(string message, Brush color, bool newLine = true)
    {
        // Если newLine == true или в документе нет блоков, создаём новый параграф
        if (newLine || _logDocument.Document.Blocks.Count == 0)
        {
            var run = new Run(message) { Foreground = color };
            var paragraph = new Paragraph(run);
            _logDocument.Document.Blocks.Add(paragraph);
        }
        else
        {
            // Пытаемся добавить в последний блок (если это Paragraph)
            var lastBlock = _logDocument.Document.Blocks.LastBlock;
            if (lastBlock is Paragraph lastParagraph)
            {
                // Добавляем пробел перед новым текстом для разделения (можно убрать)
                var run = new Run(" " + message) { Foreground = color };
                lastParagraph.Inlines.Add(run);
            }
            else
            {
                // Если последний блок не Paragraph (например, Section) – создаём новый параграф
                var run = new Run(message) { Foreground = color };
                var paragraph = new Paragraph(run);
                _logDocument.Document.Blocks.Add(paragraph);
            }
        }

        OnPropertyChanged(nameof(LogDocument));
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}