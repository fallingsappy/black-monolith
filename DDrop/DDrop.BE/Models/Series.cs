using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace DDrop.BE.Models
{
    public class Series : INotifyPropertyChanged
    {
        private string _addedDate;

        private bool _canDrawPlot;

        private User _currentUser;
        private Guid _currentUserId;

        private ObservableCollection<DropPhoto> _dropPhotosSeries;

        private bool _enableDropPhotosOrderChange;

        private bool _exactCalculationModel;

        private double _intervalBetweenPhotos;

        private bool _isChecked;

        private bool _isCheckedForAdd;

        private bool _loaded = true;

        private ReferencePhoto _referencePhotoForSeries;

        private string _title;

        private bool _useCreationDateTime;

        public Series()
        {
            _dropPhotosSeries = new ObservableCollection<DropPhoto>();
            _dropPhotosSeries.CollectionChanged += _dropPhotosSeries_CollectionChanged;
        }

        public Guid CurrentUserId
        {
            get => _currentUserId;
            set
            {
                _currentUserId = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentUserId"));
            }
        }

        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentUser"));
            }
        }

        public Guid SeriesId { get; set; }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Title"));
            }
        }

        public ObservableCollection<DropPhoto> DropPhotosSeries
        {
            get => _dropPhotosSeries;
            set
            {
                _dropPhotosSeries = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DropPhotosSeries"));
            }
        }

        public ReferencePhoto ReferencePhotoForSeries
        {
            get => _referencePhotoForSeries;
            set
            {
                _referencePhotoForSeries = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ReferencePhotoForSeries"));
            }
        }

        public bool ExactCalculationModel
        {
            get => _exactCalculationModel;
            set
            {
                _exactCalculationModel = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ExactCalculationModel"));
            }
        }

        public double IntervalBetweenPhotos
        {
            get => _intervalBetweenPhotos;
            set
            {
                _intervalBetweenPhotos = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IntervalBetweenPhotos"));
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

        public bool IsCheckedForAdd
        {
            get => _isCheckedForAdd;
            set
            {
                _isCheckedForAdd = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsCheckedForAdd"));
            }
        }

        public bool CanDrawPlot
        {
            get
            {
                if (_dropPhotosSeries?.Where(x => x.Drop?.RadiusInMeters != null).ToList().Count > 1 &&
                    _dropPhotosSeries?.Where(x => x.Drop?.RadiusInMeters == null).ToList().Count == 0 &&
                    _dropPhotosSeries.All(x => x.Drop?.RadiusInMeters != 0) && (IntervalBetweenPhotos != 0 ||
                                                                                _dropPhotosSeries?.Where(x =>
                                                                                        x.CreationDateTime == null)
                                                                                    .ToList()
                                                                                    .Count == 0 &&
                                                                                UseCreationDateTime) &&
                    _loaded)
                    return true;

                return false;
            }
            set
            {
                _canDrawPlot = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CanDrawPlot"));
            }
        }

        public bool Loaded
        {
            get => _loaded;
            set
            {
                _loaded = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Loaded"));
            }
        }

        public string AddedDate
        {
            get => _addedDate;
            set
            {
                _addedDate = value;
                OnPropertyChanged(new PropertyChangedEventArgs("AddedDate"));
            }
        }

        public bool UseCreationDateTime
        {
            get => _useCreationDateTime;
            set
            {
                _useCreationDateTime = value;
                OnPropertyChanged(new PropertyChangedEventArgs("UseCreationDateTime"));
            }
        }

        public bool EnableDropPhotosOrderChange
        {
            get => _enableDropPhotosOrderChange;
            set
            {
                _enableDropPhotosOrderChange = value;
                OnPropertyChanged(new PropertyChangedEventArgs("EnableDropPhotosOrderChange"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void _dropPhotosSeries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(CanDrawPlot)));
            foreach (var photo in _dropPhotosSeries) photo.PhotoOrderInSeries = _dropPhotosSeries.IndexOf(photo);
            CurrentUser.OnPropertyChanged(new PropertyChangedEventArgs(nameof(User.IsAnySelectedSeriesCanDrawPlot)));
        }

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Loaded))
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(CanDrawPlot)));

            if (e.PropertyName == nameof(IntervalBetweenPhotos))
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(CanDrawPlot)));

            if (e.PropertyName == nameof(UseCreationDateTime))
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(CanDrawPlot)));

            if (e.PropertyName == nameof(IsChecked))
                CurrentUser.OnPropertyChanged(
                    new PropertyChangedEventArgs(nameof(User.IsAnySelectedSeriesCanDrawPlot)));
            ;

            PropertyChanged?.Invoke(this, e);
        }
    }
}