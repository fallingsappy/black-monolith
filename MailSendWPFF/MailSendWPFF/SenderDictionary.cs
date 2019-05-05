using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodePasswordDLL;

namespace MailSendWPFF
{
    public static class SenderDictionary
    {
        public static Dictionary<string, string> Senders
        {
            get { return dicSenders; }
        }
        private static Dictionary<string, string> dicSenders = new
            Dictionary<string, string>()
            {
                { "vovask3@yandex.ru" , PasswordClass.getPassword ( "0p9o8i7u6y" ) },
                { "vovask2@gmail.com" , PasswordClass.getPassword ( "Bl1zzardnovikov" ) }
            };
    }
}
