using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using DDrop.BE.Models;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace DDrop.Controls.ScatterPlot
{
    /// <summary>
    ///     Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class ScatterPlot : INotifyPropertyChanged
    {
        public static readonly DependencyProperty UserProperty =
            DependencyProperty.Register("User", typeof(User), typeof(ScatterPlot));

        public static readonly DependencyProperty ParticularSeriesIndexProperty =
            DependencyProperty.Register("ParticularSeriesIndex", typeof(int?), typeof(ScatterPlot));

        private SeriesCollection _series;

        public ScatterPlot()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this)) return;

            Loaded += ScatterPlot_Loaded;
        }

        public int? ParticularSeriesIndex
        {
            get => (int?) GetValue(ParticularSeriesIndexProperty);
            set
            {
                SetValue(ParticularSeriesIndexProperty, value);
                OnPropertyChanged(new PropertyChangedEventArgs("ParticularSeriesIndex"));
            }
        }

        public User User
        {
            get => (User) GetValue(UserProperty);
            set
            {
                SetValue(UserProperty, value);
                OnPropertyChanged(new PropertyChangedEventArgs("User"));
            }
        }

        public SeriesCollection SeriesCollection
        {
            get => _series;
            set
            {
                _series = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SeriesCollection"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void ScatterPlot_Loaded(object sender, RoutedEventArgs e)
        {
            SeriesCollection = new SeriesCollection();

            if (User?.UserSeries != null)
            {
                if (ParticularSeriesIndex != null)
                    SingleSeriesPlot();
                else
                    MultiSeriesPlot();
            }

            DataContext = this;
        }

        private void MultiSeriesPlot()
        {
            for (var i = 0; i < User.UserSeries.Count; i++)
            {
                var temp = new ChartValues<ObservablePoint>();
                if (User.UserSeries[i].CanDrawPlot && User.UserSeries[i].IsChecked)
                {
                    if (!User.UserSeries[i].UseCreationDateTime)
                    {
                        for (var j = 0; j < User.UserSeries[i].DropPhotosSeries.Count; j++)
                        {
                            var dropRadiusInMeters = User.UserSeries[i].DropPhotosSeries[j].Drop.RadiusInMeters;
                            if (dropRadiusInMeters != null)
                                temp.Add(new ObservablePoint(j * User.UserSeries[i].IntervalBetweenPhotos,
                                    dropRadiusInMeters.Value));
                        }
                    }
                    else
                    {
                        var orderedDropPhotos = User.UserSeries[i].DropPhotosSeries
                            .OrderBy(x => DateTime.Parse(x.CreationDateTime, CultureInfo.InvariantCulture)).ToList();

                        for (var j = 0; j < orderedDropPhotos.Count; j++)
                        {
                            var dropRadiusInMeters = orderedDropPhotos[j].Drop.RadiusInMeters;
                            if (dropRadiusInMeters != null)
                                temp.Add(new ObservablePoint(
                                    (DateTime.Parse(orderedDropPhotos[j].CreationDateTime,
                                         CultureInfo.InvariantCulture) -
                                     DateTime.Parse(orderedDropPhotos[0].CreationDateTime,
                                         CultureInfo.InvariantCulture)).TotalSeconds,
                                    dropRadiusInMeters
                                        .Value));
                        }
                    }

                    SeriesCollection.Add(new LineSeries
                    {
                        Title = User.UserSeries[i].Title,
                        Values = temp,
                        LineSmoothness = 0
                    });
                }
            }
        }

        private void SingleSeriesPlot()
        {
            if (ParticularSeriesIndex != null && User.UserSeries[ParticularSeriesIndex.Value].CanDrawPlot)
            {
                var temp = new ChartValues<ObservablePoint>();
                if (!User.UserSeries[ParticularSeriesIndex.Value].UseCreationDateTime)
                {
                    for (var j = 0; j < User.UserSeries[ParticularSeriesIndex.Value].DropPhotosSeries.Count; j++)
                    {
                        var dropRadiusInMeters = User.UserSeries[ParticularSeriesIndex.Value].DropPhotosSeries[j].Drop
                            .RadiusInMeters;
                        if (dropRadiusInMeters != null)
                            temp.Add(new ObservablePoint(
                                j * User.UserSeries[ParticularSeriesIndex.Value].IntervalBetweenPhotos,
                                dropRadiusInMeters
                                    .Value));
                    }
                }
                else
                {
                    var orderedDropPhotos = User.UserSeries[ParticularSeriesIndex.Value].DropPhotosSeries
                        .OrderBy(x => DateTime.Parse(x.CreationDateTime, CultureInfo.InvariantCulture)).ToList();

                    for (var j = 0; j < orderedDropPhotos.Count; j++)
                    {
                        var dropRadiusInMeters = orderedDropPhotos[j].Drop.RadiusInMeters;
                        if (dropRadiusInMeters != null)
                            temp.Add(new ObservablePoint(
                                (DateTime.Parse(orderedDropPhotos[j].CreationDateTime, CultureInfo.InvariantCulture) -
                                 DateTime.Parse(orderedDropPhotos[0].CreationDateTime, CultureInfo.InvariantCulture))
                                .TotalSeconds,
                                dropRadiusInMeters
                                    .Value));
                    }
                }

                SeriesCollection.Add(new LineSeries
                {
                    Title = User.UserSeries[ParticularSeriesIndex.Value].Title,
                    Values = temp,
                    LineSmoothness = 0
                });
            }
        }

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }
}