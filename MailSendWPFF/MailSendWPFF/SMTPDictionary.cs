using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailSendWPFF
{
    public static class SMTPDictionary
    {
        public static Dictionary<string, int> Senders
        {
            get { return dicSMTP; }
        }
        private static Dictionary<string, int> dicSMTP = new
            Dictionary<string, int>()
            {
                { "smtp.yandex.ru" , 25 },
                { "smtp.gmail.com" , 58 },
                { "smtp.mail.ru" , 25 },
            };
    }
}
