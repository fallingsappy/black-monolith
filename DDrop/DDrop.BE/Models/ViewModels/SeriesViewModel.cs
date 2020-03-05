using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace DDrop.BE.Models
{
    public class SeriesViewModel : INotifyPropertyChanged
    {
        UserViewModel _user;
        public SeriesViewModel(UserViewModel user)
        {
            _user = user;
            _dropPhotosSeries = new ObservableCollection<DropPhotoViewModel>();
            _dropPhotosSeries.CollectionChanged += _dropPhotosSeries_CollectionChanged;
        }

        private void _dropPhotosSeries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(CanDrawPlot)));
            _user.OnPropertyChanged(new PropertyChangedEventArgs(nameof(UserViewModel.IsAnySelectedSeriesCanDrawPlot)));
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

        private ObservableCollection<DropPhotoViewModel> _dropPhotosSeries;
        public ObservableCollection<DropPhotoViewModel> DropPhotosSeries
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

        private ReferencePhotoViewModel _referencePhotoForSeries;
        public ReferencePhotoViewModel ReferencePhotoForSeries
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

        private bool _canDrawPlot;
        public bool CanDrawPlot
        {
            get
            {
                if (_dropPhotosSeries?.Where(x => x.Drop?.RadiusInMeters != null).ToList().Count > 1 &&
                    _dropPhotosSeries?.Where(x => x.Drop?.RadiusInMeters == null).ToList().Count == 0 &&
                    _dropPhotosSeries.All(x => x.Drop?.RadiusInMeters != 0) && IntervalBetweenPhotos != 0)
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

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IntervalBetweenPhotos))
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(CanDrawPlot)));

            if (e.PropertyName == nameof(IsChecked))
                _user.OnPropertyChanged(new PropertyChangedEventArgs(nameof(UserViewModel.IsAnySelectedSeriesCanDrawPlot))); ;

            PropertyChanged?.Invoke(this, e);
        }
    }
}
