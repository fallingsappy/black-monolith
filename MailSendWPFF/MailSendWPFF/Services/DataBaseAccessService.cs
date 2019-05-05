using MailSender.Services;
using MailSendWPFF;
using System.Collections.ObjectModel;
using System.Linq;

namespace MailSender.Service
{
    public class DataBaseAccessService : IDataAccessService
    {
        private readonly EmailsDataContext _dataContext = new
            EmailsDataContext();
        public ObservableCollection<Email> GetEmails() => new
            ObservableCollection<Email>(_dataContext.Email);
        public int CreateEmail(Email email)
        {
            if (_dataContext.Email.Contains(email)) return email.Id;
            _dataContext.Email.InsertOnSubmit(email);
            _dataContext.SubmitChanges();
            return email.Id;
        }
    }
}
