using System;
using System.ComponentModel;
using System.Windows.Shapes;

namespace DDrop.BE.Models
{
    public class ReferencePhoto : INotifyPropertyChanged
    {
        private byte[] _content;

        private Line _line;

        private string _name;

        private int _pixelsInMillimeter;
        private Series _series;
        private SimpleLine _simpleLine;

        private Guid? _simpleReferencePhotoLineId;

        public Series Series
        {
            get => _series;
            set
            {
                _series = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Series"));
            }
        }

        public Guid ReferencePhotoId { get; set; }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Name"));
            }
        }

        public byte[] Content
        {
            get => _content;
            set
            {
                _content = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Content"));
            }
        }

        public Guid? SimpleReferencePhotoLineId
        {
            get => _simpleReferencePhotoLineId;
            set
            {
                _simpleReferencePhotoLineId = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SimpleReferencePhotoLineId"));
            }
        }

        public SimpleLine SimpleLine
        {
            get => _simpleLine;
            set
            {
                _simpleLine = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SimpleLine"));
            }
        }

        public Line Line
        {
            get => _line;
            set
            {
                _line = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Line"));
            }
        }

        public int PixelsInMillimeter
        {
            get => _pixelsInMillimeter;
            set
            {
                _pixelsInMillimeter = value;
                OnPropertyChanged(new PropertyChangedEventArgs("PixelsInMillimeter"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }
}