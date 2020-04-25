using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Shapes;
using DDrop.BE.Enums.Options;

namespace DDrop.BE.Models
{
    public class Contour : INotifyPropertyChanged
    {
        private CalculationVariants _calculationVariants;
        private Guid _contourId;

        private DropPhoto _currentDropPhoto;

        private ObservableCollection<Line> _lines;

        private AutoCalculationParameters _parameters;

        private ObservableCollection<SimpleLine> _simpleLines;

        public Guid ContourId
        {
            get => _contourId;
            set
            {
                _contourId = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentSeriesId"));
            }
        }

        public ObservableCollection<SimpleLine> SimpleLines
        {
            get => _simpleLines;
            set
            {
                _simpleLines = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SimpleLines"));
            }
        }

        public ObservableCollection<Line> Lines
        {
            get => _lines;
            set
            {
                _lines = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Lines"));
            }
        }

        public DropPhoto CurrentDropPhoto
        {
            get => _currentDropPhoto;
            set
            {
                _currentDropPhoto = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentDropPhoto"));
            }
        }

        public AutoCalculationParameters Parameters
        {
            get => _parameters;
            set
            {
                _parameters = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Parameters"));
            }
        }

        public CalculationVariants CalculationVariants
        {
            get => _calculationVariants;
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