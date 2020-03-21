using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using DDrop.BE.Models;
using DDrop.BE.Models.Serializable;
using DDrop.Utility.SeriesLocalStorageOperations;

namespace DDrop.Utility.Mappers
{
    public static class DDropSerializableEntitiesMapper
    {
        public static async Task<ObservableCollection<Series>> ImportLocalSeriesAsync(string fileName, User user)
        {
            var series = await Task.Run(() => LocalSeriesProvider.DeserializeAsync<List<SerializableSeries>>(fileName));

            return ConvertSeriesToSeriesViewModel(series, user);
        }

        public static ObservableCollection<Series> ConvertSeriesToSeriesViewModel(List<SerializableSeries> series, User user)
        {
            ObservableCollection<Series> addSeriesViewModel = new ObservableCollection<Series>();
            for (int i = 0; i < series.Count; i++)
            {
                Series addSingleSeriesViewModel = new Series();
                ObservableCollection<DropPhoto> dropPhotosSeries = new ObservableCollection<DropPhoto>();

                if (series[i].DropPhotosSeries != null)
                {
                    foreach (var dropPhoto in series[i].DropPhotosSeries)
                    {
                        var userDropPhoto = new DropPhoto()
                        {
                            Name = dropPhoto.Name,
                            Content = dropPhoto.Content,
                            DropPhotoId = dropPhoto.DropPhotoId,
                            SimpleHorizontalLine = dropPhoto.SimpleHorizontalLine,
                            SimpleVerticalLine = dropPhoto.SimpleVerticalLine,
                            XDiameterInPixels = dropPhoto.XDiameterInPixels,
                            YDiameterInPixels = dropPhoto.YDiameterInPixels,
                            ZDiameterInPixels = dropPhoto.ZDiameterInPixels,
                            AddedDate = dropPhoto.AddedDate,
                            CurrentSeries = addSingleSeriesViewModel,
                            CurrentSeriesId = addSingleSeriesViewModel.SeriesId,
                            SimpleHorizontalLineId = dropPhoto.SimpleHorizontalLineId,
                            SimpleVerticalLineId = dropPhoto.SimpleVerticalLineId,
                            CreationDateTime = dropPhoto.CreationDateTime,
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
                        var userDrop = new Drop()
                        {
                            DropId = dropPhoto.Drop.DropId,
                            RadiusInMeters = dropPhoto.Drop.RadiusInMeters,
                            VolumeInCubicalMeters = dropPhoto.Drop.VolumeInCubicalMeters,
                            XDiameterInMeters = dropPhoto.Drop.XDiameterInMeters,
                            YDiameterInMeters = dropPhoto.Drop.YDiameterInMeters,
                            ZDiameterInMeters = dropPhoto.Drop.ZDiameterInMeters,
                            Series = addSingleSeriesViewModel,
                            DropPhoto = userDropPhoto,
                        };

                        userDropPhoto.Drop = userDrop;

                        dropPhotosSeries.Add(userDropPhoto);
                    }
                }


                if (series[i].ReferencePhotoForSeries != null)
                {
                    addSingleSeriesViewModel.ReferencePhotoForSeries = new ReferencePhoto()
                    {
                        Content = series[i].ReferencePhotoForSeries.Content,
                        Name = series[i].ReferencePhotoForSeries.Name,
                        PixelsInMillimeter = series[i].ReferencePhotoForSeries.PixelsInMillimeter,
                        ReferencePhotoId = series[i].ReferencePhotoForSeries.ReferencePhotoId,
                        SimpleLine = series[i].ReferencePhotoForSeries.SimpleLine,
                        Series = addSingleSeriesViewModel,
                        SimpleReferencePhotoLineId = series[i].ReferencePhotoForSeries.SimpleReferencePhotoLineId
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
                addSingleSeriesViewModel.CurrentUser = user;
                addSingleSeriesViewModel.CurrentUserId = user.UserId;

                addSeriesViewModel.Add(addSingleSeriesViewModel);
            }

            return addSeriesViewModel;
        }

        public static async Task ExportSeriesLocalAsync(string fileName, User user)
        {
            var series = SeriesViewModelToSeries(user);

            await Task.Run(() => LocalSeriesProvider.SerializeAsync(series, fileName));
        }

        public static List<SerializableSeries> SeriesViewModelToSeries(User user)
        {
            List<SerializableSeries> series = new List<SerializableSeries>();
            foreach (var userSeries in user.UserSeries)
            {
                series.Add(SingleSeriesViewModelToSingleSeries(userSeries, user));
            }

            return series;
        }

        public static SerializableSeries SingleSeriesViewModelToSingleSeries(Series userSeries, User user)
        {
            SerializableSeries singleSeries = new SerializableSeries();
            List<SerializableDropPhoto> dropPhotosSeries = new List<SerializableDropPhoto>();

            foreach (var dropPhoto in userSeries.DropPhotosSeries)
            {
                dropPhotosSeries.Add(new SerializableDropPhoto
                {
                    Name = dropPhoto.Name,
                    Content = dropPhoto.Content,
                    Drop = new SerializableDrop
                    {
                        DropId = dropPhoto.Drop.DropId,
                        RadiusInMeters = dropPhoto.Drop.RadiusInMeters,
                        VolumeInCubicalMeters = dropPhoto.Drop.VolumeInCubicalMeters,
                        XDiameterInMeters = dropPhoto.Drop.XDiameterInMeters,
                        YDiameterInMeters = dropPhoto.Drop.YDiameterInMeters,
                        ZDiameterInMeters = dropPhoto.Drop.ZDiameterInMeters,
                        DropPhoto = dropPhoto
                    },
                    AddedDate = dropPhoto.AddedDate,
                    DropPhotoId = dropPhoto.DropPhotoId,
                    SimpleHorizontalLine = dropPhoto.SimpleHorizontalLine,
                    SimpleVerticalLine = dropPhoto.SimpleVerticalLine,
                    XDiameterInPixels = dropPhoto.XDiameterInPixels,
                    YDiameterInPixels = dropPhoto.YDiameterInPixels,
                    ZDiameterInPixels = dropPhoto.ZDiameterInPixels,
                    SimpleHorizontalLineId = dropPhoto.SimpleHorizontalLineId,
                    SimpleVerticalLineId = dropPhoto.SimpleVerticalLineId,
                    CreationDateTime = dropPhoto.CreationDateTime,
                    CurrentSeries = dropPhoto.CurrentSeries,
                    CurrentSeriesId = dropPhoto.CurrentSeriesId
                });
            }

            if (userSeries.ReferencePhotoForSeries != null)
            {
                var referencePhoto = new SerializableReferencePhoto
                {
                    Content = userSeries.ReferencePhotoForSeries.Content,
                    Name = userSeries.ReferencePhotoForSeries.Name,
                    PixelsInMillimeter = userSeries.ReferencePhotoForSeries.PixelsInMillimeter,
                    ReferencePhotoId = userSeries.ReferencePhotoForSeries.ReferencePhotoId,
                    SimpleLine = userSeries.ReferencePhotoForSeries.SimpleLine,
                    Series = userSeries,
                    SimpleReferencePhotoLineId = userSeries.ReferencePhotoForSeries.SimpleReferencePhotoLineId
                };

                singleSeries.ReferencePhotoForSeries = referencePhoto;
            }

            singleSeries.DropPhotosSeries = dropPhotosSeries;
            singleSeries.IntervalBetweenPhotos = userSeries.IntervalBetweenPhotos;
            singleSeries.AddedDate = userSeries.AddedDate;
            singleSeries.SeriesId = userSeries.SeriesId;
            singleSeries.Title = userSeries.Title;
            singleSeries.CurrentUser = user;
            singleSeries.CurrentUserId = user.UserId;

            return singleSeries;
        }

        public static SerializableDropPhoto DropPhotoViewModelToDropPhoto(DropPhoto dropPhotoViewModel)
        {
            return new SerializableDropPhoto
            {
                AddedDate = dropPhotoViewModel.AddedDate,
                Content = dropPhotoViewModel.Content,
                Drop = new SerializableDrop
                {
                    DropId = dropPhotoViewModel.Drop.DropId,
                    RadiusInMeters = dropPhotoViewModel.Drop.RadiusInMeters,
                    VolumeInCubicalMeters = dropPhotoViewModel.Drop.VolumeInCubicalMeters,
                    XDiameterInMeters = dropPhotoViewModel.Drop.XDiameterInMeters,
                    YDiameterInMeters = dropPhotoViewModel.Drop.YDiameterInMeters,
                    ZDiameterInMeters = dropPhotoViewModel.Drop.ZDiameterInMeters
                },
                DropPhotoId = dropPhotoViewModel.DropPhotoId,
                Name = dropPhotoViewModel.Name,
                SimpleHorizontalLine = dropPhotoViewModel.SimpleHorizontalLine,
                SimpleVerticalLine = dropPhotoViewModel.SimpleVerticalLine,
                XDiameterInPixels = dropPhotoViewModel.XDiameterInPixels,
                YDiameterInPixels = dropPhotoViewModel.YDiameterInPixels,
                ZDiameterInPixels = dropPhotoViewModel.ZDiameterInPixels
            };
        }
    }
}