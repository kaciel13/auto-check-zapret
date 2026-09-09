using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace AutoCheckZapret.Services
{
    /// <summary>
    /// Класс для вывода сообщений в консоль в интерфейсе приложения
    /// </summary>
    public class Logger
    {
        private readonly FlowDocument _document;
        private readonly FlowDocumentScrollViewer _scrollViewer;

        private static readonly SolidColorBrush InfoBrush = new SolidColorBrush(Color.FromRgb(0xFE, 0xFE, 0xFE));
        private static readonly SolidColorBrush ErrorBrush = new SolidColorBrush(Color.FromRgb(0xFF, 0x4A, 0x4A));
        private static readonly SolidColorBrush SuccessBrush = new SolidColorBrush(Color.FromRgb(0x83, 0xFF, 0x87));

        public FlowDocument LogDocument => _document;

        public Logger(FlowDocumentScrollViewer scrollViewer = null)
        {
            _document = new FlowDocument();
            _scrollViewer = scrollViewer;
        }

        /// <summary>
        /// Устанавливает элемент FlowDocumentScrollViewer для авто-прокрутки
        /// </summary>
        public void SetScrollViewer(FlowDocumentScrollViewer scrollViewer)
        {
            // Убираем подписку на событие, если был привязан другой элемент
            // (можно реализовать при необходимости)
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

            // Автоматическая прокрутка к последнему сообщению
            ScrollToEnd();
        }

        /// <summary>
        /// Прокручивает FlowDocumentScrollViewer к последнему сообщению
        /// </summary>
        private void ScrollToEnd()
        {
            if (_scrollViewer == null)
                return;

            Application.Current?.Dispatcher?.BeginInvoke(new Action(() =>
            {
                try
                {
                    // Ищем ScrollViewer внутри FlowDocumentScrollViewer
                    var scrollViewer = FindVisualChild<ScrollViewer>(_scrollViewer);
                    scrollViewer?.ScrollToEnd();
                }
                catch
                {
                    // Игнорируем ошибки при прокрутке
                }
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        /// <summary>
        /// Рекурсивный поиск дочернего элемента указанного типа в визуальном дереве.
        /// </summary>
        /// <typeparam name="T">Тип искомого элемента (например, ScrollViewer)</typeparam>
        /// <param name="parent">Родительский DependencyObject, с которого начинается поиск</param>
        /// <returns>Найденный элемент или null, если элемент не найден</returns>
        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T result)
                    return result;
                var subResult = FindVisualChild<T>(child);
                if (subResult != null)
                    return subResult;
            }
            return null;
        }
    }
}