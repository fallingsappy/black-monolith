using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MailSender.Service;
using MailSender.Services;
using MailSendWPFF;

namespace MailSender.ViewModel
{
    public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private string _filterName;

        public string FilterName
        {
            get => _filterName;
            set
            {
                if(!Set(ref _filterName, value)) return;
                EmailsView.Refresh();
            }
        }

        private CollectionViewSource _emailsView;
        public ICollectionView EmailsView => _emailsView?.View;

        private readonly IDataAccessService _dataService;
        private ObservableCollection<Email> _emails = new ObservableCollection<Email>();

        public ObservableCollection<Email> Emails
        {
            get => _emails;
            set
            {
                if(!Set(ref _emails, value)) return;
                _emailsView = new CollectionViewSource {Source = value};
                _emailsView.Filter += OnEmailsCollectionViewSourceFilter;
                RaisePropertyChanged(nameof(EmailsView));
            }
        }

        private void OnEmailsCollectionViewSourceFilter(object sender, FilterEventArgs e)
        {
            if(!(e.Item is Email email) || string.IsNullOrWhiteSpace(_filterName)) return;
            if (!email.Name.Contains(_filterName))
                e.Accepted = false;
        }

        private Email _currentEmail = new Email();
        public Email CurrentEmail
        {
            get => _currentEmail;
            set => Set(ref _currentEmail, value);
        }

        public RelayCommand<Email> SaveEmailCommand { get; }
        public RelayCommand ReadAllMailsCommand { get; }
        public RelayCommand DeleteListViewItemsCommand { get; }

        public MainWindowViewModel(IDataAccessService dataService)
        {
            _dataService = dataService;
            ReadAllMailsCommand = new RelayCommand(GetEmails);
            SaveEmailCommand = new RelayCommand<Email>(SaveEmail);
            DeleteListViewItemsCommand = new RelayCommand(DeleteListViewItems);
        }

        private void DeleteListViewItems()
        {
            
        }

        private void SaveEmail(Email email)
        {
            email.Id = _dataService.CreateEmail(email);
            if (email.Id == 0) return;
            Emails.Add(email);
        }

        private void GetEmails() => Emails = _dataService.GetEmails();
    }
}
