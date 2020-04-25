using System;
using System.ComponentModel;
using System.Windows.Shapes;

namespace DDrop.BE.Models
{
    public class DropPhoto : INotifyPropertyChanged
    {
        private string _addedDate;

        private byte[] _content;

        private Contour _contour;

        private string _creationDateTime;

        private Series _currentSeries;
        private Guid _currentSeriesId;

        private Drop _drop;

        private Line _horizontalLine;

        private bool _isChecked;

        private string _name;
        private string _path;

        private int _photoOrderInSeries;

        private bool _processed;

        private bool _requireSaving;
        private SimpleLine _simpleHorizontalLine;

        private Guid? _simpleHorizontalLineId;
        private SimpleLine _simpleVerticalLine;

        private Guid? _simpleVerticalLineId;

        private Line _verticalLine;

        private int _xDiameterInPixels;

        private int _yDiameterInPixels;

        private int _zDiameterInPixels;

        public Guid CurrentSeriesId
        {
            get => _currentSeriesId;
            set
            {
                _currentSeriesId = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentSeriesId"));
            }
        }

        public Series CurrentSeries
        {
            get => _currentSeries;
            set
            {
                _currentSeries = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentSeries"));
            }
        }

        public Guid DropPhotoId { get; set; }

        public string Path
        {
            get => _path;
            set
            {
                _path = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Path"));
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Name"));
            }
        }

        public int XDiameterInPixels
        {
            get => _xDiameterInPixels;
            set
            {
                _xDiameterInPixels = value;
                OnPropertyChanged(new PropertyChangedEventArgs("XDiameterInPixels"));
            }
        }

        public int YDiameterInPixels
        {
            get => _yDiameterInPixels;
            set
            {
                _yDiameterInPixels = value;
                OnPropertyChanged(new PropertyChangedEventArgs("YDiameterInPixels"));
            }
        }

        public int ZDiameterInPixels
        {
            get => _zDiameterInPixels;
            set
            {
                _zDiameterInPixels = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ZDiameterInPixels"));
            }
        }

        public Guid? SimpleHorizontalLineId
        {
            get => _simpleHorizontalLineId;
            set
            {
                _simpleHorizontalLineId = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SimpleHorizontalLineId"));
            }
        }

        public SimpleLine SimpleHorizontalLine
        {
            get => _simpleHorizontalLine;
            set
            {
                _simpleHorizontalLine = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SimpleHorizontalLine"));
            }
        }

        public Line HorizontalLine
        {
            get => _horizontalLine;
            set
            {
                _horizontalLine = value;
                OnPropertyChanged(new PropertyChangedEventArgs("HorizontalLine"));
            }
        }

        public Guid? SimpleVerticalLineId
        {
            get => _simpleVerticalLineId;
            set
            {
                _simpleVerticalLineId = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SimpleVerticalLineId"));
            }
        }

        public SimpleLine SimpleVerticalLine
        {
            get => _simpleVerticalLine;
            set
            {
                _simpleVerticalLine = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SimpleVerticalLine"));
            }
        }

        public Line VerticalLine
        {
            get => _verticalLine;
            set
            {
                _verticalLine = value;
                OnPropertyChanged(new PropertyChangedEventArgs("VerticalLine"));
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

        public Drop Drop
        {
            get => _drop;
            set
            {
                _drop = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Drop"));
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

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsChecked"));
            }
        }

        public bool Processed
        {
            get
            {
                if (_drop.RadiusInMeters > 0)
                    return true;

                return false;
            }
            set
            {
                _processed = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Processed"));
            }
        }

        public string CreationDateTime
        {
            get => _creationDateTime;
            set
            {
                _creationDateTime = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CreationDateTime"));
            }
        }

        public bool RequireSaving
        {
            get => _requireSaving;
            set
            {
                _requireSaving = value;
                OnPropertyChanged(new PropertyChangedEventArgs("RequireSaving"));
            }
        }

        public int PhotoOrderInSeries
        {
            get => _photoOrderInSeries;
            set
            {
                _photoOrderInSeries = value;
                OnPropertyChanged(new PropertyChangedEventArgs("PhotoOrderInSeries"));
            }
        }

        public Contour Contour
        {
            get => _contour;
            set
            {
                _contour = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Contour"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }
}