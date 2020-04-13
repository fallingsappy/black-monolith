using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DDrop.BE.Models
{
    public class AutoCalculationParameters : INotifyPropertyChanged
    {
        private int _ksize;
        public int Ksize
        {
            get
            {
                return _ksize;
            }
            set
            {
                _ksize = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Ksize"));
            }
        }

        private int _treshold1;
        public int Treshold1
        {
            get
            {
                return _treshold1;
            }
            set
            {
                _treshold1 = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Treshold1"));
            }
        }

        private int _treshold2;
        public int Treshold2
        {
            get
            {
                return _treshold2;
            }
            set
            {
                _treshold2 = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Treshold2"));
            }
        }

        private int _size1;
        public int Size1
        {
            get
            {
                return _size1;
            }
            set
            {
                _size1 = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Size1"));
            }
        }

        private int _size2;
        public int Size2
        {
            get
            {
                return _size2;
            }
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