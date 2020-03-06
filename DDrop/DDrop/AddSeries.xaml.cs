using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DDrop.Annotations;
using DDrop.BE.Models;
using DDrop.Utility.ImageOperations;

namespace DDrop
{
    /// <summary>
    /// Логика взаимодействия для AddSeries.xaml
    /// </summary>
    public partial class AddSeries : Window, INotifyPropertyChanged
    {
        private bool _allSelectedSeriesChanging;
        private bool? _allSelectedSeries = false;
        public static readonly DependencyProperty SeriesForAddProperty = DependencyProperty.Register("SeriesForAdd", typeof(ObservableCollection<SeriesViewModel>), typeof(AddSeries));
        public static readonly DependencyProperty CurrentSeriesProperty = DependencyProperty.Register("CurrentSeries", typeof(SeriesViewModel), typeof(AddSeries));
        public ObservableCollection<SeriesViewModel> SeriesForAdd
        {
            get { return (ObservableCollection<SeriesViewModel>)GetValue(SeriesForAddProperty); }
            set
            {
                SetValue(SeriesForAddProperty, value);
            }
        }

        public SeriesViewModel CurrentSeries
        {
            get { return (SeriesViewModel)GetValue(CurrentSeriesProperty); }
            set
            {
                SetValue(CurrentSeriesProperty, value);
            }
        }

        private void AllSelectedChanged()
        {
            if (SeriesForAdd != null)
            {
                if (_allSelectedSeriesChanging) return;

                try
                {
                    _allSelectedSeriesChanging = true;

                    if (AllSelectedSeries == true)
                    {
                        foreach (var userSeries in SeriesForAdd)
                            userSeries.IsChecked = true;
                    }
                    else if (AllSelectedSeries == false)
                    {
                        foreach (var userSeries in SeriesForAdd)
                            userSeries.IsChecked = false;
                    }
                }
                finally
                {
                    _allSelectedSeriesChanging = false;
                }
            }
            else
            {
                AllSelectedSeries = false;
            }
        }

        private void RecheckAllSelected()
        {
            if (_allSelectedSeriesChanging) return;

            try
            {
                _allSelectedSeriesChanging = true;

                if (SeriesForAdd.All(e => e.IsChecked))
                    AllSelectedSeries = true;
                else if (SeriesForAdd.All(e => !e.IsChecked))
                    AllSelectedSeries = false;
                else
                    AllSelectedSeries = null;
            }
            finally
            {
                _allSelectedSeriesChanging = false;
            }
        }

        public bool? AllSelectedSeries
        {
            get => _allSelectedSeries;
            set
            {
                if (value == _allSelectedSeries) return;
                _allSelectedSeries = value;

                AllSelectedChanged();
                OnPropertyChanged(new PropertyChangedEventArgs("AllSelectedSeries"));
            }
        }

        private void EntryOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(SeriesViewModel.IsChecked))
                RecheckAllSelected();
        }

        public AddSeries(ObservableCollection<SeriesViewModel> seriesViewModel)
        {
            SeriesForAdd = seriesViewModel;
            foreach (var series in SeriesForAdd)
            {
                series.PropertyChanged += EntryOnPropertyChanged;
            }
            InitializeComponent();
        }

        private void AddSeriesDataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SeriesForAdd.Count > 0 && AddSeriesDataGrid.SelectedItem != null)
            {
                CurrentSeries = SeriesForAdd[AddSeriesDataGrid.SelectedIndex];
                AddSeriesPreviewDataGrid.ItemsSource = CurrentSeries.DropPhotosSeries;
            }
        }

        private void AddSeriesPreviewDataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DropPhotoViewModel selectedFile = (DropPhotoViewModel)AddSeriesPreviewDataGrid.SelectedItem;
            ImgPreview.Source = selectedFile != null ? ImageInterpreter.LoadImage(selectedFile.Content) : null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        private void AddSeriesButton_OnClickies_OnClick(object sender, RoutedEventArgs e)
        {
            bool isAnyChecked = SeriesForAdd.Any(x => x.IsChecked);

            if (isAnyChecked)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Добавить выбранные серии?", "Подтверждение добавления", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                    Close();
                else
                    SeriesForAdd.Clear();
            }
            else
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Добавить серии?", "Подтверждение добавления", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                    Close();
                else
                {
                    SeriesForAdd.Clear();
                    Close();
                }                    
            }
        }

        private void AddSeries_OnClosing(object sender, CancelEventArgs e)
        {
            bool isAnyChecked = SeriesForAdd.Any(x => x.IsChecked);

            if (isAnyChecked)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Добавить выбранные серии?", "Подтверждение добавления", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.No)
                    SeriesForAdd.Clear();   
            }
            else
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Добавить серии?", "Подтверждение добавления", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.No)
                    SeriesForAdd.Clear();
            }
        }
    }
}
