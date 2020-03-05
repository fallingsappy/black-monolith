using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace DDrop.BE.Models
{
    public class UserViewModel : INotifyPropertyChanged
    {
        public UserViewModel()
        {
            _userSeries = new ObservableCollection<SeriesViewModel>();
            _userSeries.CollectionChanged += _userSeries_CollectionChanged;
        }

        private void _userSeries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsAnySelectedSeriesCanDrawPlot)));
        }

        public Guid UserId { get; set; }

        private string _firstName;
        public string FirstName
        {
            get
            {
                return _firstName;
            }
            set
            {
                _firstName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("FirstName"));
            }
        }

        private string _lastName;
        public string LastName
        {
            get
            {
                return _lastName;
            }
            set
            {
                _lastName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("LastName"));
            }
        }

        private byte[] _userPhoto;
        public byte[] UserPhoto
        {
            get
            {
                return _userPhoto;
            }
            set
            {
                _userPhoto = value;
                OnPropertyChanged(new PropertyChangedEventArgs("UserPhoto"));
            }
        }

        private string _password;
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Password"));
            }
        }

        private string _email;
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Email"));
            }
        }

        private ObservableCollection<SeriesViewModel> _userSeries;
        public ObservableCollection<SeriesViewModel> UserSeries
        {
            get
            {
                return _userSeries;
            }
            set
            {
                _userSeries = value;
                OnPropertyChanged(new PropertyChangedEventArgs("UserSeries"));
            }
        }

        private bool _isAnySelectedSeriesCantDrawPlot;
        [NotMapped]
        public bool IsAnySelectedSeriesCanDrawPlot
        {
            get
            {
                List<SeriesViewModel> checkedSeries = _userSeries?.Where(x => x.IsChecked).ToList();

                if (checkedSeries.Count != 0)
                {
                    bool isAnyCheckedCantDrawPlot = checkedSeries?.Where(x => x.CanDrawPlot != true).ToList().Count > 0;

                    if (isAnyCheckedCantDrawPlot)
                        return false;

                    return true;
                }

                return false;
            }
            set
            {
                _isAnySelectedSeriesCantDrawPlot = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsAnySelectedSeriesCantDrawPlot"));
            }
        }

        private bool _isLoggedIn;
        [NotMapped]
        public bool IsLoggedIn
        {
            get
            {
                return _isLoggedIn;
            }
            set
            {
                _isLoggedIn = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsLoggedIn"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }
}
