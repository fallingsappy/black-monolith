using System.ComponentModel;

namespace DDrop.BE.Models
{
    public class AutoCalculationParameters : INotifyPropertyChanged
    {
        private int _ksize;

        private int _size1;

        private int _size2;

        private int _treshold1;

        private int _treshold2;

        public int Ksize
        {
            get => _ksize;
            set
            {
                _ksize = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Ksize"));
            }
        }

        public int Treshold1
        {
            get => _treshold1;
            set
            {
                _treshold1 = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Treshold1"));
            }
        }

        public int Treshold2
        {
            get => _treshold2;
            set
            {
                _treshold2 = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Treshold2"));
            }
        }

        public int Size1
        {
            get => _size1;
            set
            {
                _size1 = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Size1"));
            }
        }

        public int Size2
        {
            get => _size2;
            set
            {
                _size2 = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Size2"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }
}