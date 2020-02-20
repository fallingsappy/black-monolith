using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Shapes;

namespace DDrop.BE.Models
{
    public class DropPhoto : INotifyPropertyChanged
    {
        public Guid DropPhotoId { get; set; }
        private string _path;
        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Path"));
            }
        }

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

        private int _xDiameterInPixels;
        public int XDiameterInPixels
        {
            get
            {
                return _xDiameterInPixels;
            }
            set
            {
                _xDiameterInPixels = value;
                OnPropertyChanged(new PropertyChangedEventArgs("XDiameterInPixels"));
            }
        }

        private int _yDiameterInPixels;
        public int YDiameterInPixels
        {
            get
            {
                return _yDiameterInPixels;
            }
            set
            {
                _yDiameterInPixels = value;
                OnPropertyChanged(new PropertyChangedEventArgs("YDiameterInPixels"));
            }
        }

        private int _zDiameterInPixels;
        public int ZDiameterInPixels
        {
            get
            {
                return _zDiameterInPixels;
            }
            set
            {
                _zDiameterInPixels = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ZDiameterInPixels"));
            }
        }

        private SimpleLine _simpleHorizontalLine;
        public SimpleLine SimpleHorizontalLine
        {
            get
            {
                return _simpleHorizontalLine;
            }
            set
            {
                _simpleHorizontalLine = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SimpleHorizontalLine"));
            }
        }

        private Line _horizontalLine;
        public Line HorizontalLine
        {
            get
            {
                return _horizontalLine;
            }
            set
            {
                _horizontalLine = value;
                OnPropertyChanged(new PropertyChangedEventArgs("HorizontalLine"));
            }
        }

        private SimpleLine _simpleVerticalLine;
        public SimpleLine SimpleVerticalLine
        {
            get
            {
                return _simpleVerticalLine;
            }
            set
            {
                _simpleHorizontalLine = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SimpleVerticalLine"));
            }
        }

        private Line _verticalLine;
        public Line VerticalLine
        {
            get
            {
                return _verticalLine;
            }
            set
            {
                _verticalLine = value;
                OnPropertyChanged(new PropertyChangedEventArgs("VerticalLine"));
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

        private int _time;
        public int Time
        {
            get
            {
                return _time;
            }
            set
            {
                _time = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Time"));
            }
        }

        private Drop _drop;
        public Drop Drop
        {
            get
            {
                return _drop;
            }
            set
            {
                _drop = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Drop"));
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

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }
}
