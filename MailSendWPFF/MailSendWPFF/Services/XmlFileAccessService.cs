using MailSender.Services;
using MailSendWPFF;
using System;
using System.Collections.ObjectModel;

namespace MailSender.Service
{
    class XmlFileAccessService : IDataAccessService
    {
        public ObservableCollection<Email> GetEmails() => throw new
            NotImplementedException();
        public int CreateEmail(Email email) => throw new
            NotImplementedException();
    }
}