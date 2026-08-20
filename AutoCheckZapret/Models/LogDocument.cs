using System.Windows.Documents;

namespace AutoCheckZapret.Models
{
    public class LogDocument
    {
        public FlowDocument Document { get; set; }

        public LogDocument(FlowDocument document)
        {
            Document = document;
        }
    }
}
