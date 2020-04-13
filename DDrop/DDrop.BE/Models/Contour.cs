using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Shapes;

namespace DDrop.BE.Models
{
    public class Contour : INotifyPropertyChanged
    {
        private Guid _contourId;
        public Guid ContourId
        {
            get
            {
                return _contourId;
            }
            set
            {
                _contourId = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentSeriesId"));
            }
        }

        private ObservableCollection<SimpleLine> _simpleLines;
        public ObservableCollection<SimpleLine> SimpleLines
        {
            get
            {
                return _simpleLines;
            }
            set
            {
                _simpleLines = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SimpleLines"));
            }
        }

        private ObservableCollection<Line> _lines;
        public ObservableCollection<Line> Lines
        {
            get
            {
                return _lines;
            }
            set
            {
                _lines = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Lines"));
            }
        }

        private DropPhoto _currentDropPhoto;
        public DropPhoto CurrentDropPhoto
        {
            get
            {
                return _currentDropPhoto;
            }
            set
            {
                _currentDropPhoto = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentDropPhoto"));
            }
        }

        private AutoCalculationParameters _pythonParameters;
        public AutoCalculationParameters PythonParameters
        {
            get
            {
                return _pythonParameters;
            }
            set
            {
                _pythonParameters = value;
                OnPropertyChanged(new PropertyChangedEventArgs("PythonParameters"));
            }
        }

        private AutoCalculationParameters _cSharParameters;
        public AutoCalculationParameters CSharpParameters
        {
            get
            {
                return _cSharParameters;

            }
            set
            {
                _cSharParameters = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CSharpParameters"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }
}
