using System;
using System.ComponentModel;

namespace DDrop.BE.Models
{
    public class Drop : INotifyPropertyChanged
    {
        private DropPhoto _dropPhoto;

        private double? _radiusInMeters;
        private Series _series;

        private double _volumeInCubicalMeters;
        private double _xDiameterInMeters;

        private double _yDiameterInMeters;

        private double _zDiameterInMeters;

        public Series Series
        {
            get => _series;
            set
            {
                _series = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Series"));
            }
        }

        public DropPhoto DropPhoto
        {
            get => _dropPhoto;
            set
            {
                _dropPhoto = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DropPhoto"));
            }
        }

        public Guid DropId { get; set; }

        public double XDiameterInMeters
        {
            get => _xDiameterInMeters;
            set
            {
                _xDiameterInMeters = value;
                OnPropertyChanged(new PropertyChangedEventArgs("XDiameterInMeters"));
            }
        }

        public double YDiameterInMeters
        {
            get => _yDiameterInMeters;
            set
            {
                _yDiameterInMeters = value;
                OnPropertyChanged(new PropertyChangedEventArgs("YDiameterInMeters"));
            }
        }

        public double ZDiameterInMeters
        {
            get => _zDiameterInMeters;
            set
            {
                _zDiameterInMeters = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ZDiameterInMeters"));
            }
        }

        public double VolumeInCubicalMeters
        {
            get => _volumeInCubicalMeters;
            set
            {
                _volumeInCubicalMeters = value;
                OnPropertyChanged(new PropertyChangedEventArgs("VolumeInCubicalMeters"));
            }
        }

        public double? RadiusInMeters
        {
            get => _radiusInMeters;
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