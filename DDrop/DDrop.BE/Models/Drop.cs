using System;
using System.ComponentModel;
using System.Linq;

namespace DDrop.BE.Models
{
    public class Drop : INotifyPropertyChanged
    {
        private Series _series;
        public Series Series
        {
            get
            {
                return _series;
            }
            set
            {
                _series = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Series"));
            }
        }

        private DropPhoto _dropPhoto;
        public DropPhoto DropPhoto
        {
            get
            {
                return _dropPhoto;
            }
            set
            {
                _dropPhoto = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DropPhoto"));
            }
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
                if (_series != null)
                    _series.OnPropertyChanged(new PropertyChangedEventArgs(nameof(Series.CanDrawPlot)));
                if (_dropPhoto != null)
                    _dropPhoto.OnPropertyChanged(new PropertyChangedEventArgs(nameof(DropPhoto.Processed)));
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
