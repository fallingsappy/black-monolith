using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using static MailSendWPFF.WpfMailSender;
using CodePasswordDLL;
using System.Collections.ObjectModel;

namespace MailSendWPFF
{
    public class EmailSendServiceClass
    {
        #region vars
        private string strLogin; 
        private string strPassword; 
        private string strSmtp; 
        private int iSmtpPort; 
        public string strBody; 
        public string strSubject; 
        #endregion
        public EmailSendServiceClass(string sLogin, string sPassword, string smtp, int port, string letterText, string letterSub)
        {
            strLogin = sLogin;
            strPassword =  PasswordClass.getCodPassword(sPassword); 
            strSmtp = smtp;
            iSmtpPort = port;
            strBody = letterText;
            strSubject = letterSub;
        }
        private void SendMail(string mail, string name) 
        {
            using (MailMessage mm = new MailMessage(strLogin, mail))
            {
                mm.Subject = strSubject;
                mm.Body = strBody;
                mm.IsBodyHtml = false;
                SmtpClient sc = new SmtpClient(strSmtp, iSmtpPort);
                sc.EnableSsl = true;
                sc.DeliveryMethod = SmtpDeliveryMethod.Network;
                sc.UseDefaultCredentials = false;
                sc.Credentials = new NetworkCredential(strLogin, strPassword);
                try
                {
                    sc.Send(mm);
                }
                catch (Exception ex)
                {
                    SendErrorWindow serw = new SendErrorWindow();
                    serw.lSError.Content = "Невозможно отправить письмо " + ex.ToString();
                    serw.ShowDialog();
                }
            }
        } 
        public void SendMails(ObservableCollection<Email> emails)
        {
            foreach (Email email in emails)
            {
                var thread = new System.Threading.Thread(() => SendMail(email.Value, email.Name));
                thread.Start();
            }
        }
    }
}
