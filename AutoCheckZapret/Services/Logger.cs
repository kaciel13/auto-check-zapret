using System.Windows.Documents;
using System.Windows.Media;

public class Logger
{
    private readonly FlowDocument _document;

    private static readonly SolidColorBrush InfoBrush = new SolidColorBrush(Color.FromRgb(0xFE, 0xFE, 0xFE));
    private static readonly SolidColorBrush ErrorBrush = new SolidColorBrush(Color.FromRgb(0xFF, 0x4A, 0x4A));
    private static readonly SolidColorBrush SuccessBrush = new SolidColorBrush(Color.FromRgb(0x83, 0xFF, 0x87));

    public FlowDocument LogDocument => _document;

    /// <summary>
    /// Событие возникает при добавлении нового сообщения в лог.
    /// </summary>
    public event EventHandler MessageAdded;

    public Logger()
    {
        _document = new FlowDocument();
    }

    public void AddInfo(string message, bool newLine = true) => AppendMessage(message, InfoBrush, newLine);
    public void AddError(string message, bool newLine = true) => AppendMessage(message, ErrorBrush, newLine);
    public void AddSuccess(string message, bool newLine = true) => AppendMessage(message, SuccessBrush, newLine);

    private void AppendMessage(string message, Brush color, bool newLine = true)
    {
        if (newLine || _document.Blocks.Count == 0)
        {
            var run = new Run(message) { Foreground = color };
            var paragraph = new Paragraph(run);
            _document.Blocks.Add(paragraph);
        }
        else
        {
            var lastBlock = _document.Blocks.LastBlock;
            if (lastBlock is Paragraph lastParagraph)
            {
                var run = new Run(" " + message) { Foreground = color };
                lastParagraph.Inlines.Add(run);
            }
            else
            {
                var run = new Run(message) { Foreground = color };
                var paragraph = new Paragraph(run);
                _document.Blocks.Add(paragraph);
            }
        }

        // Уведомление для автоскролла
       MessageAdded?.Invoke(this, EventArgs.Empty);
    }
}