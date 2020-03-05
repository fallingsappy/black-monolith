using DDrop.BE.Models;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace DDrop.Controls
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class ScatterPlot : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty UserProperty =
        DependencyProperty.Register("User", typeof(UserViewModel), typeof(ScatterPlot));

        public static readonly DependencyProperty ParticularSeriesIndexProperty =
        DependencyProperty.Register("ParticularSeriesIndex", typeof(int?), typeof(ScatterPlot));

        public int? ParticularSeriesIndex
        {
            get { return (int?)GetValue(ParticularSeriesIndexProperty); }
            set { SetValue(ParticularSeriesIndexProperty, value); OnPropertyChanged(new PropertyChangedEventArgs("ParticularSeriesIndex")); }
        }

        public UserViewModel User
        {
            get { return (UserViewModel)GetValue(UserProperty); }
            set { SetValue(UserProperty, value); OnPropertyChanged(new PropertyChangedEventArgs("User")); }
        }

        public ScatterPlot()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }

            Loaded += ScatterPlot_Loaded;
        }

        private void ScatterPlot_Loaded(object sender, RoutedEventArgs e)
        {
            List<ChartValues<ObservablePoint>> seriesValues = new List<ChartValues<ObservablePoint>>();
            SeriesCollection = new SeriesCollection();

            if (User?.UserSeries != null)
            {
                if (ParticularSeriesIndex != null)
                {
                    SingleSeriesPlot(seriesValues);
                }
                else
                {
                    MultiSeriesPlot(seriesValues);
                }
            }

            DataContext = this;
        }

        private void MultiSeriesPlot(List<ChartValues<ObservablePoint>> seriesValues)
        {
            for (int i = 0; i < User.UserSeries.Count; i++)
            {
                if (User.UserSeries[i].CanDrawPlot == true && User.UserSeries[i].IsChecked)
                {
                    seriesValues.Add(new ChartValues<ObservablePoint>());
                    for (int j = 0; j < User.UserSeries[i].DropPhotosSeries.Count; j++)
                    {
                        seriesValues[i].Add(new ObservablePoint(j * User.UserSeries[i].IntervalBetweenPhotos, User.UserSeries[i].DropPhotosSeries[j].Drop.RadiusInMeters.Value));
                    }

                    SeriesCollection.Add(new LineSeries
                    {
                        Title = User.UserSeries[i].Title,
                        Values = seriesValues[i],
                        LineSmoothness = 0,
                    });
                }
            }
        }

        private void SingleSeriesPlot(List<ChartValues<ObservablePoint>> seriesValues)
        {
            if (User.UserSeries[ParticularSeriesIndex.Value].CanDrawPlot == true)
            {
                seriesValues.Add(new ChartValues<ObservablePoint>());
                for (int j = 0; j < User.UserSeries[ParticularSeriesIndex.Value].DropPhotosSeries.Count; j++)
                {
                    seriesValues[0].Add(new ObservablePoint(j * User.UserSeries[ParticularSeriesIndex.Value].IntervalBetweenPhotos, User.UserSeries[ParticularSeriesIndex.Value].DropPhotosSeries[j].Drop.RadiusInMeters.Value));
                }

                SeriesCollection.Add(new LineSeries
                {
                    Title = User.UserSeries[ParticularSeriesIndex.Value].Title,
                    Values = seriesValues[0],
                    LineSmoothness = 0,
                });
            }
        }

        private SeriesCollection _series;
        public SeriesCollection SeriesCollection
        {
            get
            {
                return _series;
            }
            set
            {
                _series = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SeriesCollection"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }
}
