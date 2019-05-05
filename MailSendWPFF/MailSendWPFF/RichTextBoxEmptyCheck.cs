using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MailSendWPFF
{
    public static class RichTextBoxEmptyCheck
    {
        public static bool isRichTextBoxEmpty(RichTextBox MyRTB1)
        {
            TextPointer startPointer = MyRTB1.Document.ContentStart.GetNextInsertionPosition(LogicalDirection.Forward);
            TextPointer endPointer = MyRTB1.Document.ContentEnd.GetNextInsertionPosition(LogicalDirection.Backward);
            if (startPointer.CompareTo(endPointer) == 0)
                return true;
            else
                return false;
        }
    }
}
