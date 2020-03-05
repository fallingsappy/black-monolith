using System;
using System.ComponentModel;
using System.Linq;

namespace DDrop.BE.Models
{
    public class DropViewModel : INotifyPropertyChanged
    {
        SeriesViewModel _series;
        DropPhotoViewModel _dropPhoto;
        public DropViewModel(SeriesViewModel series, DropPhotoViewModel dropPhoto)
        {
            _series = series;
            _dropPhoto = dropPhoto;
        }

        public Guid DropId { get; set; }
        private double _xDiameterInMeters;
        public double XDiameterInMeters
        {
            get
            {
                return _xDiameterInMeters;
            }
            set
            {
                _xDiameterInMeters = value;
                OnPropertyChanged(new PropertyChangedEventArgs("XDiameterInMeters"));
            }
        }

        private double _yDiameterInMeters;
        public double YDiameterInMeters
        {
            get
            {
                return _yDiameterInMeters;
            }
            set
            {
                _yDiameterInMeters = value;
                OnPropertyChanged(new PropertyChangedEventArgs("YDiameterInMeters"));
            }
        }

        private double _zDiameterInMeters;
        public double ZDiameterInMeters
        {
            get
            {
                return _zDiameterInMeters;
            }
            set
            {
                _zDiameterInMeters = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ZDiameterInMeters"));
            }
        }

        private double _volumeInCubicalMeters;
        public double VolumeInCubicalMeters
        {
            get
            {
                return _volumeInCubicalMeters;
            }
            set
            {
                _volumeInCubicalMeters = value;
                OnPropertyChanged(new PropertyChangedEventArgs("VolumeInCubicalMeters"));
            }
        }

        private double? _radiusInMeters;
        public double? RadiusInMeters
        {
            get
            {
                return _radiusInMeters;
            }
            set
            {
                _radiusInMeters = value;
                _series.OnPropertyChanged(new PropertyChangedEventArgs(nameof(SeriesViewModel.CanDrawPlot)));
                _dropPhoto.OnPropertyChanged(new PropertyChangedEventArgs(nameof(DropPhotoViewModel.Processed)));
                OnPropertyChanged(new PropertyChangedEventArgs("RadiusInMeters"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }
}
