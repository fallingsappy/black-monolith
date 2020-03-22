﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace DDrop.BE.Models
{
    public class Series : INotifyPropertyChanged
    {
        private Guid _currentUserId;
        public Guid CurrentUserId
        {
            get
            {
                return _currentUserId;
            }
            set
            {
                _currentUserId = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentUserId"));
            }
        }

        private User _currentUser;
        public User CurrentUser
        {
            get
            {
                return _currentUser;
            }
            set
            {
                _currentUser = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentUser"));
            }
        }

        public Series()
        {
            _dropPhotosSeries = new ObservableCollection<DropPhoto>();
            _dropPhotosSeries.CollectionChanged += _dropPhotosSeries_CollectionChanged;
        }

        private void _dropPhotosSeries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(CanDrawPlot)));
            foreach (var photo in _dropPhotosSeries)
            {
                photo.PhotoOrderInSeries = _dropPhotosSeries.IndexOf(photo);
            }
            CurrentUser.OnPropertyChanged(new PropertyChangedEventArgs(nameof(User.IsAnySelectedSeriesCanDrawPlot)));
        }

        public Guid SeriesId { get; set; }

        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Title"));
            }
        }

        private ObservableCollection<DropPhoto> _dropPhotosSeries;
        public ObservableCollection<DropPhoto> DropPhotosSeries
        {
            get
            {
                return _dropPhotosSeries;
            }
            set
            {
                _dropPhotosSeries = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DropPhotosSeries"));
            }
        }

        private ReferencePhoto _referencePhotoForSeries;
        public ReferencePhoto ReferencePhotoForSeries
        {
            get
            {
                return _referencePhotoForSeries;
            }
            set
            {
                _referencePhotoForSeries = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ReferencePhotoForSeries"));
            }
        }

        private bool _exactCalculationModel;
        public bool ExactCalculationModel
        {
            get
            {
                return _exactCalculationModel;
            }
            set
            {
                _exactCalculationModel = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ExactCalculationModel"));
            }
        }

        private double _intervalBetweenPhotos;
        public double IntervalBetweenPhotos
        {
            get
            {
                return _intervalBetweenPhotos;
            }
            set
            {
                _intervalBetweenPhotos = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IntervalBetweenPhotos"));
            }
        }

        private bool _isChecked;
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                _isChecked = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsChecked"));
            }
        }

        private bool _isCheckedForAdd;
        public bool IsCheckedForAdd
        {
            get
            {
                return _isCheckedForAdd;
            }
            set
            {
                _isCheckedForAdd = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsCheckedForAdd"));
            }
        }

        private bool _canDrawPlot;
        public bool CanDrawPlot
        {
            get
            {
                if (_dropPhotosSeries?.Where(x => x.Drop?.RadiusInMeters != null).ToList().Count > 1 &&
                    _dropPhotosSeries?.Where(x => x.Drop?.RadiusInMeters == null).ToList().Count == 0 &&
                    _dropPhotosSeries.All(x => x.Drop?.RadiusInMeters != 0) && (IntervalBetweenPhotos != 0 || 
                    _dropPhotosSeries?.Where(x => x.CreationDateTime == null).ToList().Count == 0 && UseCreationDateTime))
                {
                    return true;
                }

                return false;
            }
            set
            {
                _canDrawPlot = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CanDrawPlot"));
            }
        }

        private string _addedDate;
        public string AddedDate
        {
            get
            {
                return _addedDate;
            }
            set
            {
                _addedDate = value;
                OnPropertyChanged(new PropertyChangedEventArgs("AddedDate"));
            }
        }

        private bool _useCreationDateTime;
        public bool UseCreationDateTime
        {
            get
            {
                return _useCreationDateTime;
            }
            set
            {
                _useCreationDateTime = value;
                OnPropertyChanged(new PropertyChangedEventArgs("UseCreationDateTime"));
            }
        }

        private bool _enableDropPhotosOrderChange;

        public bool EnableDropPhotosOrderChange
        {
            get
            {
                return _enableDropPhotosOrderChange;
            }
            set
            {
                _enableDropPhotosOrderChange = value;
                OnPropertyChanged(new PropertyChangedEventArgs("EnableDropPhotosOrderChange"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IntervalBetweenPhotos))
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(CanDrawPlot)));

            if (e.PropertyName == nameof(UseCreationDateTime))
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(CanDrawPlot)));

            if (e.PropertyName == nameof(IsChecked))
                CurrentUser.OnPropertyChanged(new PropertyChangedEventArgs(nameof(User.IsAnySelectedSeriesCanDrawPlot))); ;

            PropertyChanged?.Invoke(this, e);
        }
    }
}
