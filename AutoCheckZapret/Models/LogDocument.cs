using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
