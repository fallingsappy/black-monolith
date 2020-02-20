using System;
using System.ComponentModel;
using System.Windows.Shapes;

namespace DDrop.BE.Models
{
    public class ReferencePhoto : INotifyPropertyChanged
    {
        public Guid ReferencePhotoId { get; set; }

        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Name"));
            }
        }

        private byte[] _content;
        public byte[] Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Content"));
            }
        }

        private SimpleLine _simpleLine;
        public SimpleLine SimpleLine
        {
            get
            {
                return _simpleLine;
            }
            set
            {
                _simpleLine = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SimpleLine"));
            }
        }

        private Line _line;
        public Line Line
        {
            get
            {
                return _line;
            }
            set
            {
                _line = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Line"));
            }
        }

        private int _pixelsInMillimeter;
        public int PixelsInMillimeter
        {
            get
            {
                return _pixelsInMillimeter;
            }
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
