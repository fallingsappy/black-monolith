using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Shapes;
using DDrop.BE.Enums.Options;

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

        private AutoCalculationParameters _parameters;
        public AutoCalculationParameters Parameters
        {
            get
            {
                return _parameters;
            }
            set
            {
                _parameters = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Parameters"));
            }
        }

        private CalculationVariants _calculationVariants;
        public CalculationVariants CalculationVariants
        {
            get
            {
                return _calculationVariants;
            }
            set
            {
                _calculationVariants = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CalculationVariants"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }
}
