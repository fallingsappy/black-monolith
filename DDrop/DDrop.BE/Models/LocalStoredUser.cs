using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DDrop.BE.Models
{
    public class LocalStoredUser : INotifyPropertyChanged
    {
        private bool _isChecked;
        private string _login;

        private string _password;

        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Login"));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Password"));
            }
        }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsChecked"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }

    public class LocalStoredUsers : INotifyPropertyChanged
    {
        private bool _isEnabled;
        private ObservableCollection<LocalStoredUser> _users;

        public ObservableCollection<LocalStoredUser> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Users"));
            }
        }

        public bool IsEnabled
        {
            get => _users.Count > 0;
            set
            {
                _isEnabled = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsEnabled"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }
}