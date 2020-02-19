using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace DDrop.BE.Models
{
    public class Series : INotifyPropertyChanged
    {
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

        private bool _canDrawPlot;
        public bool CanDrawPlot
        {
            get
            {
                return _dropPhotosSeries?.Where(x => x.Drop.RadiusInMeters != null).ToList().Count > 1 && _dropPhotosSeries?.Where(x => x.Drop.RadiusInMeters == null).ToList().Count == 0;
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
            PropertyChanged?.Invoke(this, e);
        }
    }
}
