using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using DDrop.BE.Enums.Options;
using DDrop.BE.Models;
using DDrop.Db.DbEntities;
using DDrop.Utility.SeriesLocalStorageOperations;

namespace DDrop.Utility.Mappers
{
    public static class DDropDbEntitiesMapper
    {
        public static async Task<DbSeries> ImportLocalSeriesAsync(string fileName, DbUser user)
        {
            var series = await Task.Run(() => JsonSerializeProvider.DeserializeFromFileAsync<DbSeries>(fileName));
            series.CurrentUser = user;
            series.CurrentUserId = user.UserId;

            return series;
        }

        public static async Task ExportSeriesLocalAsync(string fileName, DbSeries dbSeries)
        {
            await Task.Run(() => JsonSerializeProvider.SerializeToFileAsync(dbSeries, fileName));
        }

        public static DbUser UserToDbUser(User user)
        {
            var dbUser = new DbUser
            {
                Email = user.Email,
                Password = user.Password,
                UserPhoto = user.UserPhoto,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserId = user.UserId
            };

            dbUser.UserSeries = SeriesToDbSeries(user.UserSeries, dbUser);

            return dbUser;
        }

        public static User DbUserToUser(DbUser dbUser)
        {
            var user = new User
            {
                Email = dbUser.Email,
                Password = dbUser.Password,
                UserPhoto = dbUser.UserPhoto,
                FirstName = dbUser.FirstName,
                LastName = dbUser.LastName,
                UserId = dbUser.UserId,
                IsLoggedIn = true
            };

            user.UserSeries = DbSeriesToSeries(dbUser.UserSeries, user);

            return user;
        }

        public static List<DbSeries> SeriesToDbSeries(ObservableCollection<Series> series, DbUser dbUser)
        {
            var dbSeries = new List<DbSeries>();
            foreach (var userSeries in series) dbSeries.Add(SingleSeriesToSingleDbSeries(userSeries, dbUser));

            return dbSeries;
        }

        public static DbSeries SingleSeriesToSingleDbSeries(Series userSeries, DbUser user)
        {
            var singleSeries = new DbSeries
            {
                IntervalBetweenPhotos = userSeries.IntervalBetweenPhotos,
                AddedDate = userSeries.AddedDate,
                SeriesId = userSeries.SeriesId,
                Title = userSeries.Title,
                CurrentUser = user,
                CurrentUserId = user.UserId,
                UseCreationDateTime = userSeries.UseCreationDateTime
            };
            var dropPhotosSeries = new List<DbDropPhoto>();

            foreach (var dropPhoto in userSeries.DropPhotosSeries)
            {
                var newDbDropPhoto = new DbDropPhoto
                {
                    Name = dropPhoto.Name,
                    Content = dropPhoto.Content,
                    AddedDate = dropPhoto.AddedDate,
                    DropPhotoId = dropPhoto.DropPhotoId,
                    XDiameterInPixels = dropPhoto.XDiameterInPixels,
                    YDiameterInPixels = dropPhoto.YDiameterInPixels,
                    ZDiameterInPixels = dropPhoto.ZDiameterInPixels,
                    CurrentSeries = singleSeries,
                    CurrentSeriesId = singleSeries.SeriesId,
                    CreationDateTime = dropPhoto.CreationDateTime,
                    PhotoOrderInSeries = dropPhoto.PhotoOrderInSeries
                };
                if (dropPhoto.SimpleHorizontalLine != null)
                {
                    var newHorizontalDbSimpleLine = new DbSimpleLine
                    {
                        X1 = dropPhoto.SimpleHorizontalLine.X1,
                        X2 = dropPhoto.SimpleHorizontalLine.X2,
                        Y1 = dropPhoto.SimpleHorizontalLine.Y1,
                        Y2 = dropPhoto.SimpleHorizontalLine.Y2,
                        SimpleLineId = dropPhoto.SimpleHorizontalLine.SimpleLineId
                    };

                    newDbDropPhoto.SimpleHorizontalLine = newHorizontalDbSimpleLine;
                    newDbDropPhoto.SimpleHorizontalLineId = newHorizontalDbSimpleLine.SimpleLineId;
                }

                if (dropPhoto.SimpleVerticalLine != null)
                {
                    var newVerticalDbSimpleLine = new DbSimpleLine
                    {
                        X1 = dropPhoto.SimpleVerticalLine.X1,
                        X2 = dropPhoto.SimpleVerticalLine.X2,
                        Y1 = dropPhoto.SimpleVerticalLine.Y1,
                        Y2 = dropPhoto.SimpleVerticalLine.Y2,
                        SimpleLineId = dropPhoto.SimpleVerticalLine.SimpleLineId
                    };

                    newDbDropPhoto.SimpleVerticalLine = newVerticalDbSimpleLine;
                    newDbDropPhoto.SimpleVerticalLineId = newVerticalDbSimpleLine.SimpleLineId;
                }

                var newDbDrop = new DbDrop
                {
                    DropId = dropPhoto.Drop.DropId,
                    RadiusInMeters = dropPhoto.Drop.RadiusInMeters,
                    VolumeInCubicalMeters = dropPhoto.Drop.VolumeInCubicalMeters,
                    XDiameterInMeters = dropPhoto.Drop.XDiameterInMeters,
                    YDiameterInMeters = dropPhoto.Drop.YDiameterInMeters,
                    ZDiameterInMeters = dropPhoto.Drop.ZDiameterInMeters,
                    DropPhoto = newDbDropPhoto
                };

                if (dropPhoto.Contour != null)
                {
                    var newDbContour = new DbContour
                    {
                        CurrentDropPhoto = newDbDropPhoto,
                        CalculationParameters = JsonSerializeProvider.SerializeToString(dropPhoto.Contour.Parameters),
                        CalculationProvider = dropPhoto.Contour.CalculationVariants.ToString(),
                        ContourId = dropPhoto.Contour.ContourId
                    };

                    var newDbSimpleLines = new List<DbSimpleLine>();

                    foreach (var contourSimpleLine in dropPhoto.Contour.SimpleLines)
                        newDbSimpleLines.Add(new DbSimpleLine
                        {
                            SimpleLineId = contourSimpleLine.SimpleLineId,
                            X1 = contourSimpleLine.X1,
                            X2 = contourSimpleLine.X2,
                            Y1 = contourSimpleLine.Y1,
                            Y2 = contourSimpleLine.Y2,
                            ContourId = contourSimpleLine.ContourId
                        });

                    newDbContour.SimpleLines = newDbSimpleLines;

                    newDbDropPhoto.Contour = newDbContour;
                }

                newDbDropPhoto.Drop = newDbDrop;

                dropPhotosSeries.Add(newDbDropPhoto);
            }

            if (userSeries.ReferencePhotoForSeries != null)
            {
                var referencePhoto = new DbReferencePhoto
                {
                    Content = userSeries.ReferencePhotoForSeries.Content,
                    Name = userSeries.ReferencePhotoForSeries.Name,
                    PixelsInMillimeter = userSeries.ReferencePhotoForSeries.PixelsInMillimeter,
                    ReferencePhotoId = userSeries.ReferencePhotoForSeries.ReferencePhotoId,
                    Series = singleSeries
                };

                if (userSeries.ReferencePhotoForSeries.SimpleLine != null)
                {
                    var simpleLineForReferencePhoto = new DbSimpleLine
                    {
                        X1 = userSeries.ReferencePhotoForSeries.SimpleLine.X1,
                        X2 = userSeries.ReferencePhotoForSeries.SimpleLine.X2,
                        Y1 = userSeries.ReferencePhotoForSeries.SimpleLine.Y1,
                        Y2 = userSeries.ReferencePhotoForSeries.SimpleLine.Y2,
                        SimpleLineId = userSeries.ReferencePhotoForSeries.SimpleLine.SimpleLineId
                    };

                    referencePhoto.SimpleReferencePhotoLineId = simpleLineForReferencePhoto.SimpleLineId;
                    referencePhoto.SimpleReferencePhotoLine = simpleLineForReferencePhoto;
                }

                singleSeries.ReferencePhotoForSeries = referencePhoto;
            }

            singleSeries.DropPhotosSeries = dropPhotosSeries;

            return singleSeries;
        }

        public static ObservableCollection<Series> DbSeriesToSeries(List<DbSeries> series, User user)
        {
            var addSeriesViewModel = new ObservableCollection<Series>();
            if (series != null)
                for (var i = 0; i < series.Count; i++)
                    addSeriesViewModel.Add(SingleDbSerieToSerie(series[i], user));

            return addSeriesViewModel;
        }

        public static Series SingleDbSerieToSerie(DbSeries serie, User user, bool deserialization = false)
        {
            var addSingleSeriesViewModel = new Series
            {
                AddedDate = deserialization ? DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") : serie.AddedDate,
                Title = serie.Title,
                IntervalBetweenPhotos = serie.IntervalBetweenPhotos,
                SeriesId = deserialization ? Guid.NewGuid() : serie.SeriesId,
                CurrentUser = user,
                CurrentUserId = user.UserId,
                UseCreationDateTime = serie.UseCreationDateTime
            };

            if (serie.DropPhotosSeries != null)
                foreach (var dropPhoto in serie.DropPhotosSeries)
                {
                    var userDropPhoto = new DropPhoto
                    {
                        Name = dropPhoto.Name,
                        Content = dropPhoto.Content,
                        DropPhotoId = deserialization ? Guid.NewGuid() : dropPhoto.DropPhotoId,
                        XDiameterInPixels = dropPhoto.XDiameterInPixels,
                        YDiameterInPixels = dropPhoto.YDiameterInPixels,
                        ZDiameterInPixels = dropPhoto.ZDiameterInPixels,
                        AddedDate =
                            deserialization ? DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") : dropPhoto.AddedDate,
                        CurrentSeries = addSingleSeriesViewModel,
                        CurrentSeriesId = addSingleSeriesViewModel.SeriesId,
                        CreationDateTime = dropPhoto.CreationDateTime,
                        PhotoOrderInSeries = dropPhoto.PhotoOrderInSeries
                    };

                    if (dropPhoto.SimpleHorizontalLine != null)
                    {
                        var newSimpleHorizontalLine = new SimpleLine
                        {
                            X1 = dropPhoto.SimpleHorizontalLine.X1,
                            X2 = dropPhoto.SimpleHorizontalLine.X2,
                            Y1 = dropPhoto.SimpleHorizontalLine.Y1,
                            Y2 = dropPhoto.SimpleHorizontalLine.Y2,
                            SimpleLineId = deserialization
                                ? Guid.NewGuid()
                                : dropPhoto.SimpleHorizontalLine.SimpleLineId
                        };
                        userDropPhoto.SimpleHorizontalLine = newSimpleHorizontalLine;
                        userDropPhoto.SimpleHorizontalLineId = newSimpleHorizontalLine.SimpleLineId;
                    }

                    if (dropPhoto.SimpleVerticalLine != null)
                    {
                        var newSimpleVerticalLine = new SimpleLine
                        {
                            X1 = dropPhoto.SimpleVerticalLine.X1,
                            X2 = dropPhoto.SimpleVerticalLine.X2,
                            Y1 = dropPhoto.SimpleVerticalLine.Y1,
                            Y2 = dropPhoto.SimpleVerticalLine.Y2,
                            SimpleLineId = deserialization ? Guid.NewGuid() : dropPhoto.SimpleVerticalLine.SimpleLineId
                        };
                        userDropPhoto.SimpleVerticalLine = newSimpleVerticalLine;
                        userDropPhoto.SimpleVerticalLineId = newSimpleVerticalLine.SimpleLineId;
                    }

                    if (dropPhoto.SimpleHorizontalLine != null)
                        userDropPhoto.HorizontalLine = new Line
                        {
                            X1 = dropPhoto.SimpleHorizontalLine.X1,
                            X2 = dropPhoto.SimpleHorizontalLine.X2,
                            Y1 = dropPhoto.SimpleHorizontalLine.Y1,
                            Y2 = dropPhoto.SimpleHorizontalLine.Y2,
                            Stroke = Brushes.DeepPink
                        };

                    if (dropPhoto.SimpleVerticalLine != null)
                        userDropPhoto.VerticalLine = new Line
                        {
                            X1 = dropPhoto.SimpleVerticalLine.X1,
                            X2 = dropPhoto.SimpleVerticalLine.X2,
                            Y1 = dropPhoto.SimpleVerticalLine.Y1,
                            Y2 = dropPhoto.SimpleVerticalLine.Y2,
                            Stroke = Brushes.Green
                        };

                    if (dropPhoto.Drop != null)
                    {
                        var userDrop = new Drop
                        {
                            DropId = deserialization ? userDropPhoto.DropPhotoId : dropPhoto.Drop.DropId,
                            RadiusInMeters = dropPhoto.Drop.RadiusInMeters,
                            VolumeInCubicalMeters = dropPhoto.Drop.VolumeInCubicalMeters,
                            XDiameterInMeters = dropPhoto.Drop.XDiameterInMeters,
                            YDiameterInMeters = dropPhoto.Drop.YDiameterInMeters,
                            ZDiameterInMeters = dropPhoto.Drop.ZDiameterInMeters,
                            Series = addSingleSeriesViewModel,
                            DropPhoto = userDropPhoto
                        };

                        userDropPhoto.Drop = userDrop;
                    }

                    if (dropPhoto.Contour != null)
                    {
                        var userContour = new Contour
                        {
                            CurrentDropPhoto = userDropPhoto,
                            Parameters =
                                JsonSerializeProvider.DeserializeFromString<AutoCalculationParameters>(dropPhoto.Contour
                                    .CalculationParameters),
                            CalculationVariants = (CalculationVariants) Enum.Parse(typeof(CalculationVariants),
                                dropPhoto.Contour.CalculationProvider, true),
                            ContourId = userDropPhoto.DropPhotoId
                        };

                        var userSimpleLines = new ObservableCollection<SimpleLine>();
                        var userLines = new ObservableCollection<Line>();

                        foreach (var contourSimpleLine in dropPhoto.Contour.SimpleLines)
                        {
                            if (contourSimpleLine.ContourId != null || deserialization)
                                userSimpleLines.Add(new SimpleLine
                                {
                                    SimpleLineId = deserialization ? Guid.NewGuid() : contourSimpleLine.SimpleLineId,
                                    X1 = contourSimpleLine.X1,
                                    X2 = contourSimpleLine.X2,
                                    Y1 = contourSimpleLine.Y1,
                                    Y2 = contourSimpleLine.Y2,
                                    ContourId = userDropPhoto.DropPhotoId
                                });

                            userLines.Add(new Line
                            {
                                X1 = contourSimpleLine.X1,
                                X2 = contourSimpleLine.X2,
                                Y1 = contourSimpleLine.Y1,
                                Y2 = contourSimpleLine.Y2,
                                Stroke = Brushes.Red,
                                StrokeThickness = 2
                            });
                        }

                        userContour.Lines = userLines;
                        userContour.SimpleLines = userSimpleLines;
                        userDropPhoto.Contour = userContour;
                    }

                    addSingleSeriesViewModel.DropPhotosSeries.Add(userDropPhoto);
                }

            if (serie.ReferencePhotoForSeries != null)
            {
                addSingleSeriesViewModel.ReferencePhotoForSeries = new ReferencePhoto
                {
                    Content = serie.ReferencePhotoForSeries.Content,
                    Name = serie.ReferencePhotoForSeries.Name,
                    PixelsInMillimeter = serie.ReferencePhotoForSeries.PixelsInMillimeter,
                    ReferencePhotoId = deserialization
                        ? addSingleSeriesViewModel.SeriesId
                        : serie.ReferencePhotoForSeries.ReferencePhotoId,
                    Series = addSingleSeriesViewModel
                };

                if (serie.ReferencePhotoForSeries.SimpleReferencePhotoLine != null)
                {
                    var newSimpleLine = new SimpleLine
                    {
                        X1 = serie.ReferencePhotoForSeries.SimpleReferencePhotoLine.X1,
                        X2 = serie.ReferencePhotoForSeries.SimpleReferencePhotoLine.X2,
                        Y1 = serie.ReferencePhotoForSeries.SimpleReferencePhotoLine.Y1,
                        Y2 = serie.ReferencePhotoForSeries.SimpleReferencePhotoLine.Y2,
                        SimpleLineId = deserialization
                            ? Guid.NewGuid()
                            : serie.ReferencePhotoForSeries.SimpleReferencePhotoLine.SimpleLineId
                    };
                    addSingleSeriesViewModel.ReferencePhotoForSeries.SimpleLine = newSimpleLine;
                    addSingleSeriesViewModel.ReferencePhotoForSeries.SimpleReferencePhotoLineId =
                        newSimpleLine.SimpleLineId;
                }
            }

            if (serie.ReferencePhotoForSeries?.SimpleReferencePhotoLine != null)
                addSingleSeriesViewModel.ReferencePhotoForSeries.Line = new Line
                {
                    X1 = serie.ReferencePhotoForSeries.SimpleReferencePhotoLine.X1,
                    X2 = serie.ReferencePhotoForSeries.SimpleReferencePhotoLine.X2,
                    Y1 = serie.ReferencePhotoForSeries.SimpleReferencePhotoLine.Y1,
                    Y2 = serie.ReferencePhotoForSeries.SimpleReferencePhotoLine.Y2,
                    Stroke = Brushes.DeepPink
                };

            return addSingleSeriesViewModel;
        }

        public static List<DbDropPhoto> ListOfDropPhotosToListOfDbDropPhotos(ObservableCollection<DropPhoto> dropPhotos,
            Guid dbSeriesId)
        {
            var dbDropPhotos = new List<DbDropPhoto>();

            foreach (var dropPhoto in dropPhotos) dbDropPhotos.Add(DropPhotoToDbDropPhoto(dropPhoto, dbSeriesId));

            return dbDropPhotos;
        }

        public static DbDropPhoto DropPhotoToDbDropPhoto(DropPhoto dropPhotoViewModel, Guid dbSeriesId)
        {
            var dbDropPhoto = new DbDropPhoto
            {
                AddedDate = dropPhotoViewModel.AddedDate,
                Content = dropPhotoViewModel.Content,
                DropPhotoId = dropPhotoViewModel.DropPhotoId,
                Name = dropPhotoViewModel.Name,
                XDiameterInPixels = dropPhotoViewModel.XDiameterInPixels,
                YDiameterInPixels = dropPhotoViewModel.YDiameterInPixels,
                ZDiameterInPixels = dropPhotoViewModel.ZDiameterInPixels,
                CreationDateTime = dropPhotoViewModel.CreationDateTime,
                PhotoOrderInSeries = dropPhotoViewModel.PhotoOrderInSeries,
                CurrentSeriesId = dbSeriesId
            };

            if (dropPhotoViewModel.Drop != null)
            {
                var newDbDrop = new DbDrop
                {
                    DropId = dropPhotoViewModel.Drop.DropId,
                    RadiusInMeters = dropPhotoViewModel.Drop.RadiusInMeters,
                    VolumeInCubicalMeters = dropPhotoViewModel.Drop.VolumeInCubicalMeters,
                    XDiameterInMeters = dropPhotoViewModel.Drop.XDiameterInMeters,
                    YDiameterInMeters = dropPhotoViewModel.Drop.YDiameterInMeters,
                    ZDiameterInMeters = dropPhotoViewModel.Drop.ZDiameterInMeters,
                    DropPhoto = dbDropPhoto
                };
                dbDropPhoto.Drop = newDbDrop;
            }

            if (dropPhotoViewModel.SimpleHorizontalLine != null)
            {
                var newDbSimpleHorizontalLine = new DbSimpleLine
                {
                    X1 = dropPhotoViewModel.SimpleHorizontalLine.X1,
                    X2 = dropPhotoViewModel.SimpleHorizontalLine.X2,
                    Y1 = dropPhotoViewModel.SimpleHorizontalLine.Y1,
                    Y2 = dropPhotoViewModel.SimpleHorizontalLine.Y2,
                    SimpleLineId = dropPhotoViewModel.SimpleHorizontalLine.SimpleLineId
                };

                dbDropPhoto.SimpleHorizontalLineId = newDbSimpleHorizontalLine.SimpleLineId;
                dbDropPhoto.SimpleHorizontalLine = newDbSimpleHorizontalLine;
            }

            if (dropPhotoViewModel.SimpleVerticalLine != null)
            {
                var newDbSimpleVerticalLine = new DbSimpleLine
                {
                    X1 = dropPhotoViewModel.SimpleVerticalLine.X1,
                    X2 = dropPhotoViewModel.SimpleVerticalLine.X2,
                    Y1 = dropPhotoViewModel.SimpleVerticalLine.Y1,
                    Y2 = dropPhotoViewModel.SimpleVerticalLine.Y2,
                    SimpleLineId = dropPhotoViewModel.SimpleVerticalLine.SimpleLineId
                };

                dbDropPhoto.SimpleVerticalLineId = newDbSimpleVerticalLine.SimpleLineId;
                dbDropPhoto.SimpleVerticalLine = newDbSimpleVerticalLine;
            }

            if (dropPhotoViewModel.Contour != null)
            {
                var newDbContour = new DbContour
                {
                    CurrentDropPhoto = null,
                    CalculationParameters =
                        JsonSerializeProvider.SerializeToString(dropPhotoViewModel.Contour.Parameters),
                    CalculationProvider = dropPhotoViewModel.Contour.CalculationVariants.ToString(),
                    ContourId = dropPhotoViewModel.Contour.ContourId
                };

                var newDbSimpleLines = new List<DbSimpleLine>();

                foreach (var contourSimpleLine in dropPhotoViewModel.Contour.SimpleLines)
                    newDbSimpleLines.Add(new DbSimpleLine
                    {
                        SimpleLineId = contourSimpleLine.SimpleLineId,
                        X1 = contourSimpleLine.X1,
                        X2 = contourSimpleLine.X2,
                        Y1 = contourSimpleLine.Y1,
                        Y2 = contourSimpleLine.Y2,
                        ContourId = contourSimpleLine.ContourId
                    });

                newDbContour.SimpleLines = newDbSimpleLines;

                dbDropPhoto.Contour = newDbContour;
            }

            return dbDropPhoto;
        }

        public static DbReferencePhoto ReferencePhotoToDbReferencePhoto(ReferencePhoto referencePhoto)
        {
            var dbReferencePhoto = new DbReferencePhoto
            {
                Content = referencePhoto.Content,
                ReferencePhotoId = referencePhoto.ReferencePhotoId,
                Name = referencePhoto.Name,
                PixelsInMillimeter = referencePhoto.PixelsInMillimeter
            };

            if (referencePhoto.SimpleLine != null)
            {
                var newDbSimpleLine = new DbSimpleLine
                {
                    X1 = referencePhoto.SimpleLine.X1,
                    X2 = referencePhoto.SimpleLine.X2,
                    Y1 = referencePhoto.SimpleLine.Y1,
                    Y2 = referencePhoto.SimpleLine.Y2,
                    SimpleLineId = referencePhoto.SimpleLine.SimpleLineId
                };

                dbReferencePhoto.SimpleReferencePhotoLineId = newDbSimpleLine.SimpleLineId;
                dbReferencePhoto.SimpleReferencePhotoLine = newDbSimpleLine;
            }

            return dbReferencePhoto;
        }
    }
}