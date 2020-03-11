using DDrop.BE.Models;
using DDrop.BE.Models.Entities;
using DDrop.Utility.SeriesLocalStorageOperations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DDrop.BL.Series
{
    public class SeriesBL : ISeriesBL
    {
        public async Task<ObservableCollection<BE.Models.Series>> ImportLocalSeriesAsync(string fileName, BE.Models.User user)
        {
            List<BE.Models.Entities.Series> series = new List<BE.Models.Entities.Series>();

            series = await Task.Run(() => LocalSeriesProvider.DeserializeAsync<List<BE.Models.Entities.Series>>(fileName));

            return ConvertSeriesToSeriesViewModel(series, user);
        }

        public ObservableCollection<BE.Models.Series> ConvertSeriesToSeriesViewModel(List<BE.Models.Entities.Series> series, BE.Models.User user)
        {
            ObservableCollection<BE.Models.Series> addSeriesViewModel = new ObservableCollection<BE.Models.Series>();
            for (int i = 0; i < series.Count; i++)
            {
                BE.Models.Series addSingleSeriesViewModel = new BE.Models.Series(user);
                ObservableCollection<BE.Models.DropPhoto> dropPhotosSeries = new ObservableCollection<BE.Models.DropPhoto>();

                if (series[i].DropPhotosSeries != null)
                {
                    foreach (var dropPhoto in series[i].DropPhotosSeries)
                    {
                        var userDropPhoto = new BE.Models.DropPhoto()
                        {
                            Name = dropPhoto.Name,
                            Content = dropPhoto.Content,
                            DropPhotoId = dropPhoto.DropPhotoId,
                            SimpleHorizontalLine = dropPhoto.SimpleHorizontalLine,
                            SimpleVerticalLine = dropPhoto.SimpleVerticalLine,
                            XDiameterInPixels = dropPhoto.XDiameterInPixels,
                            YDiameterInPixels = dropPhoto.YDiameterInPixels,
                            ZDiameterInPixels = dropPhoto.ZDiameterInPixels,
                            AddedDate = dropPhoto.AddedDate
                        };

                        if (dropPhoto.SimpleHorizontalLine != null)
                        {
                            userDropPhoto.HorizontalLine = new Line
                            {
                                X1 = dropPhoto.SimpleHorizontalLine.X1,
                                X2 = dropPhoto.SimpleHorizontalLine.X2,
                                Y1 = dropPhoto.SimpleHorizontalLine.Y1,
                                Y2 = dropPhoto.SimpleHorizontalLine.Y2,
                                Stroke = Brushes.DeepPink
                            };
                        }

                        if (dropPhoto.SimpleVerticalLine != null)
                        {
                            userDropPhoto.VerticalLine = new Line
                            {
                                X1 = dropPhoto.SimpleVerticalLine.X1,
                                X2 = dropPhoto.SimpleVerticalLine.X2,
                                Y1 = dropPhoto.SimpleVerticalLine.Y1,
                                Y2 = dropPhoto.SimpleVerticalLine.Y2,
                                Stroke = Brushes.Green
                            };
                        }
                        var userDrop = new BE.Models.Drop(addSingleSeriesViewModel, userDropPhoto)
                        {
                            DropId = dropPhoto.Drop.DropId,
                            RadiusInMeters = dropPhoto.Drop.RadiusInMeters,
                            VolumeInCubicalMeters = dropPhoto.Drop.VolumeInCubicalMeters,
                            XDiameterInMeters = dropPhoto.Drop.XDiameterInMeters,
                            YDiameterInMeters = dropPhoto.Drop.YDiameterInMeters,
                            ZDiameterInMeters = dropPhoto.Drop.ZDiameterInMeters
                        };

                        userDropPhoto.Drop = userDrop;

                        dropPhotosSeries.Add(userDropPhoto);
                    }
                }


                if (series[i].ReferencePhotoForSeries != null)
                {
                    addSingleSeriesViewModel.ReferencePhotoForSeries = new BE.Models.ReferencePhoto
                    {
                        Content = series[i].ReferencePhotoForSeries.Content,
                        Name = series[i].ReferencePhotoForSeries.Name,
                        PixelsInMillimeter = series[i].ReferencePhotoForSeries.PixelsInMillimeter,
                        ReferencePhotoId = series[i].ReferencePhotoForSeries.ReferencePhotoId,
                        SimpleLine = series[i].ReferencePhotoForSeries.SimpleLine,
                    };
                }

                if (series[i].ReferencePhotoForSeries?.SimpleLine != null)
                {
                    addSingleSeriesViewModel.ReferencePhotoForSeries.Line = new Line
                    {
                        X1 = series[i].ReferencePhotoForSeries.SimpleLine.X1,
                        X2 = series[i].ReferencePhotoForSeries.SimpleLine.X2,
                        Y1 = series[i].ReferencePhotoForSeries.SimpleLine.Y1,
                        Y2 = series[i].ReferencePhotoForSeries.SimpleLine.Y2,
                        Stroke = Brushes.DeepPink
                    };
                }
                addSingleSeriesViewModel.AddedDate = series[i].AddedDate;
                addSingleSeriesViewModel.Title = series[i].Title;
                addSingleSeriesViewModel.DropPhotosSeries = dropPhotosSeries;
                addSingleSeriesViewModel.IntervalBetweenPhotos = series[i].IntervalBetweenPhotos;
                addSingleSeriesViewModel.SeriesId = series[i].SeriesId;

                addSeriesViewModel.Add(addSingleSeriesViewModel);
            }

            return addSeriesViewModel;
        }

        public async Task ExportSeriesLocalAsync(string fileName, BE.Models.User user)
        {
            var series = SeriesViewModelToSeries(user);

            await Task.Run(() => LocalSeriesProvider.SerializeAsync(series, fileName));
        }

        public List<BE.Models.Entities.Series> SeriesViewModelToSeries(BE.Models.User user)
        {
            List<BE.Models.Entities.Series> series = new List<BE.Models.Entities.Series>();
            foreach (var userSeries in user.UserSeries)
            {
                series.Add(SingleSeriesViewModelToSingleSeries(userSeries));
            }

            return series;
        }

        public BE.Models.Entities.Series SingleSeriesViewModelToSingleSeries(BE.Models.Series userSeries)
        {
            BE.Models.Entities.Series SingleSeries = new BE.Models.Entities.Series();
            List<BE.Models.Entities.DropPhoto> dropPhotosSeries = new List<BE.Models.Entities.DropPhoto>();

            foreach (var dropPhoto in userSeries.DropPhotosSeries)
            {
                dropPhotosSeries.Add(new BE.Models.Entities.DropPhoto()
                {
                    Name = dropPhoto.Name,
                    Content = dropPhoto.Content,
                    Drop = new BE.Models.Entities.Drop
                    {
                        DropId = dropPhoto.Drop.DropId,
                        RadiusInMeters = dropPhoto.Drop.RadiusInMeters,
                        VolumeInCubicalMeters = dropPhoto.Drop.VolumeInCubicalMeters,
                        XDiameterInMeters = dropPhoto.Drop.XDiameterInMeters,
                        YDiameterInMeters = dropPhoto.Drop.YDiameterInMeters,
                        ZDiameterInMeters = dropPhoto.Drop.ZDiameterInMeters
                    },
                    AddedDate = dropPhoto.AddedDate,
                    DropPhotoId = dropPhoto.DropPhotoId,
                    SimpleHorizontalLine = dropPhoto.SimpleHorizontalLine,
                    SimpleVerticalLine = dropPhoto.SimpleVerticalLine,
                    XDiameterInPixels = dropPhoto.XDiameterInPixels,
                    YDiameterInPixels = dropPhoto.YDiameterInPixels,
                    ZDiameterInPixels = dropPhoto.ZDiameterInPixels
                });
            }

            if(userSeries.ReferencePhotoForSeries != null)
            {
                var referencePhoto = new BE.Models.Entities.ReferencePhoto
                {
                    Content = userSeries.ReferencePhotoForSeries.Content,
                    Name = userSeries.ReferencePhotoForSeries.Name,
                    PixelsInMillimeter = userSeries.ReferencePhotoForSeries.PixelsInMillimeter,
                    ReferencePhotoId = userSeries.ReferencePhotoForSeries.ReferencePhotoId,
                    SimpleLine = userSeries.ReferencePhotoForSeries.SimpleLine
                };

                SingleSeries.ReferencePhotoForSeries = referencePhoto;
            }

            SingleSeries.DropPhotosSeries = dropPhotosSeries;
            SingleSeries.IntervalBetweenPhotos = userSeries.IntervalBetweenPhotos;
            SingleSeries.AddedDate = userSeries.AddedDate;
            SingleSeries.SeriesId = userSeries.SeriesId;
            SingleSeries.Title = userSeries.Title;

            return SingleSeries;
        }
    }
}
