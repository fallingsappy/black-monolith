using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Mail;
using MailSendWPFF.View;
using MailSendWPFF.ViewModel;
using GalaSoft.MvvmLight.Command;

namespace MailSendWPFF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class WpfMailSender : Window
    {
        public WpfMailSender()
        {
            InitializeComponent();
            cbSenderSelect.ItemsSource = SenderDictionary.Senders;
            cbSenderSelect.DisplayMemberPath = "Key";
            cbSenderSelect.SelectedValuePath = "Value";

            cbSmtpSelect.ItemsSource = SMTPDictionary.Senders;
            cbSmtpSelect.DisplayMemberPath = "Key";
            cbSmtpSelect.SelectedValuePath = "Value";
        }

        private void MiClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnClock_Click(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedItem = tabPlanner;
        }

        private void TabSwitcherControl_OnBack(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedIndex == 0) return;
            tabControl.SelectedIndex--;
        }

        private void TabSwitcherControl_OnForward(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedIndex == tabControl.Items.Count - 1) return;
            tabControl.SelectedIndex++;
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            string strLogin;
            string strPassword;
            string SMTP;
            string richText = new TextRange(emailEditor.Document.ContentStart, emailEditor.Document.ContentEnd).Text;
            string subText = emailEditorSubj.Text;
            if (string.IsNullOrEmpty(strLogin = cbSenderSelect.Text))
            {
                MessageBox.Show("Выберите отправителя");
                return;
            }
            if (string.IsNullOrEmpty(strPassword = cbSenderSelect.SelectedValue.ToString()))
            {
                MessageBox.Show("Укажите пароль отправителя");
                return;
            }
            if (RichTextBoxEmptyCheck.isRichTextBoxEmpty(emailEditor)==true)
            {
                MessageBox.Show("Письмо не заполнено");
                tabControl.SelectedItem = tabEditor;
                return;
            }
            if (string.IsNullOrEmpty(SMTP = cbSmtpSelect.Text))
            {
                MessageBox.Show("Выберите SMTP");
                return;
            }
            int port = int.Parse(cbSmtpSelect.SelectedValue.ToString());
            EmailSendServiceClass emailSender = new EmailSendServiceClass(strLogin, strPassword, SMTP, port, richText, subText);
            var locator = (ViewModelLocator)FindResource("Locator");
            emailSender.SendMails(locator.Main.Emails);
        }

        private void btnSendAtOnce_Click(object sender, RoutedEventArgs e)
        {
            string strLogin;
            string strPassword;
            string SMTP;
            string richText = new TextRange(emailEditor.Document.ContentStart, emailEditor.Document.ContentEnd).Text;
            string subText = emailEditorSubj.Text;
            if (string.IsNullOrEmpty(strLogin = cbSenderSelect.Text))
            {
                MessageBox.Show("Выберите отправителя");
                return;
            }
            if (string.IsNullOrEmpty(strPassword = cbSenderSelect.SelectedValue.ToString()))
            {
                MessageBox.Show("Укажите пароль отправителя");
                return;
            }
            if (RichTextBoxEmptyCheck.isRichTextBoxEmpty(emailEditor) == true)
            {
                MessageBox.Show("Письмо не заполнено");
                tabControl.SelectedItem = tabEditor;
                return;
            }
            if (string.IsNullOrEmpty(SMTP = cbSmtpSelect.Text))
            {
                MessageBox.Show("Выберите SMTP");
                return;
            }
            SchedulerClass sc = new SchedulerClass();
            TimeSpan tsSendTime = sc.GetSendTime(tbTimePicker.Text);
            if (tsSendTime == new TimeSpan())
            {
                MessageBox.Show("Некорректный формат даты");
                return;
            }
            DateTime dtSendDateTime = (cldSchedulDateTimes.SelectedDate ??
            DateTime.Today).Add(tsSendTime);
            if (dtSendDateTime < DateTime.Now)
            {
                MessageBox.Show("Дата и время отправки писем не могут быть раньше, чем настоящее время" );
            return;
            }
            int port = int.Parse(cbSmtpSelect.SelectedValue.ToString());
            EmailSendServiceClass emailSender = new EmailSendServiceClass(strLogin, strPassword, SMTP, port, richText, subText);
            var locator = (ViewModelLocator)FindResource("Locator");
            sc.SendEmails(dtSendDateTime, emailSender, locator.Main.Emails);
        }

        /*private void Button_Click(object sender, RoutedEventArgs e)
        {
            listView.Items.Add(new ListViewItemScheduler());
        }*/
    }
}
