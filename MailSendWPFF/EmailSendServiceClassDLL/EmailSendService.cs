using System;
using System.Net;
using System.Net.Mail;
using CodePasswordDLL;
using System.Collections.ObjectModel;

namespace EmailSendServiceClassDLL
{
    public class EmailSendService
    {
        #region vars
        private string strLogin;
        private string strPassword;
        private string strSmtp;
        private int iSmtpPort;
        private string strBody;
        private string strSubject;
        #endregion
        public EmailSendService(string sLogin, string sPassword, string smtp, int port, string letterText, string letterSub)
        {
            strLogin = sLogin;
            strPassword = PasswordClass.getCodPassword(sPassword);
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
                SendMail(email.Value, email.Name);
            }
        }
    }
}
