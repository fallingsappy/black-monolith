using System;
using System.ComponentModel;
using DDrop.BE.Enums.Options;

namespace DDrop.BE.Models
{
    public class AutoCalculationTemplate : INotifyPropertyChanged
    {
        private Guid _id;

        private bool _isChecked;

        private AutoCalculationParameters _parameters;

        private CalculationVariants _templateType;

        private string _title;

        public Guid Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Id"));
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Title"));
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

        public CalculationVariants TemplateType
        {
            get => _templateType;
            set
            {
                _templateType = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TemplateType"));
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

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }
}