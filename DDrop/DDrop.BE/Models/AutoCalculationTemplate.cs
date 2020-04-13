using System;
using System.ComponentModel;
using DDrop.BE.Enums.Options;

namespace DDrop.BE.Models
{
    public class AutoCalculationTemplate : INotifyPropertyChanged
    {
        private Guid _id;
        public Guid Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Id"));
            }
        }

        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Title"));
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

        private CalculationVariants _templateType;

        public CalculationVariants TemplateType
        {
            get
            {
                return _templateType;

            }
            set
            {
                _templateType = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TemplateType"));
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