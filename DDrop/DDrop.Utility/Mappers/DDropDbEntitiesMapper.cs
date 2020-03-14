using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Shapes;
using DDrop.BE.Models;
using DDrop.Db.DbEntities;

namespace DDrop.Utility.Mappers
{
    public static class DDropDbEntitiesMapper
    {
        public static DbUser UserToDbUser(User user)
        {
            return new DbUser
            {
                Email = user.Email,
                Password = user.Password,
                UserPhoto = user.UserPhoto,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserId = user.UserId,
                UserSeries = SeriesToDbSeries(user)
            };
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

        public static List<DbSeries> SeriesToDbSeries(User user)
        {
            List<DbSeries> series = new List<DbSeries>();
            foreach (var userSeries in user.UserSeries)
            {
                series.Add(SingleSeriesToSingleDbSeries(userSeries, user));
            }

            return series;
        }

        public static DbSeries SingleSeriesToSingleDbSeries(Series userSeries, User user)
        {
            DbSeries singleSeries = new DbSeries();
            List<DbDropPhoto> dropPhotosSeries = new List<DbDropPhoto>();

            foreach (var dropPhoto in userSeries.DropPhotosSeries)
            {
                DbDropPhoto newDbDropPhoto = new DbDropPhoto()
                {
                    Name = dropPhoto.Name,
                    Content = dropPhoto.Content,
                    AddedDate = dropPhoto.AddedDate,
                    DropPhotoId = dropPhoto.DropPhotoId,
                    XDiameterInPixels = dropPhoto.XDiameterInPixels,
                    YDiameterInPixels = dropPhoto.YDiameterInPixels,
                    ZDiameterInPixels = dropPhoto.ZDiameterInPixels,
                    CurrentSeries = SingleSeriesToSingleDbSeries(userSeries, user),
                    CurrentSeriesId = userSeries.SeriesId,
                };

                DbSimpleLine newHorizontalDbSimpleLine = new DbSimpleLine
                {
                    X1 = dropPhoto.SimpleHorizontalLine.X1,
                    X2 = dropPhoto.SimpleHorizontalLine.X2,
                    Y1 = dropPhoto.SimpleHorizontalLine.Y1,
                    Y2 = dropPhoto.SimpleHorizontalLine.Y2,
                    DropPhotoHorizontalLine = newDbDropPhoto,
                    SimpleLineId = dropPhoto.SimpleHorizontalLine.SimpleLineId,
                };

                DbSimpleLine newVerticalDbSimpleLine = new DbSimpleLine
                {
                    X1 = dropPhoto.SimpleVerticalLine.X1,
                    X2 = dropPhoto.SimpleVerticalLine.X2,
                    Y1 = dropPhoto.SimpleVerticalLine.Y1,
                    Y2 = dropPhoto.SimpleVerticalLine.Y2,
                    DropPhotoVerticalLine = newDbDropPhoto,
                    SimpleLineId = dropPhoto.SimpleHorizontalLine.SimpleLineId,
                };

                DbDrop newDbDrop = new DbDrop()
                {
                    DropId = dropPhoto.Drop.DropId,
                    RadiusInMeters = dropPhoto.Drop.RadiusInMeters,
                    VolumeInCubicalMeters = dropPhoto.Drop.VolumeInCubicalMeters,
                    XDiameterInMeters = dropPhoto.Drop.XDiameterInMeters,
                    YDiameterInMeters = dropPhoto.Drop.YDiameterInMeters,
                    ZDiameterInMeters = dropPhoto.Drop.ZDiameterInMeters,
                    DropPhoto = newDbDropPhoto
                };

                newDbDropPhoto.Drop = newDbDrop;
                newDbDropPhoto.SimpleHorizontalLine = newHorizontalDbSimpleLine;
                newDbDropPhoto.SimpleVerticalLine = newVerticalDbSimpleLine;

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
                    Series = singleSeries,
                };

                var simpleLineForReferencePhoto = new DbSimpleLine
                {
                    X1 = userSeries.ReferencePhotoForSeries.SimpleLine.X1,
                    X2 = userSeries.ReferencePhotoForSeries.SimpleLine.X2,
                    Y1 = userSeries.ReferencePhotoForSeries.SimpleLine.Y1,
                    Y2 = userSeries.ReferencePhotoForSeries.SimpleLine.Y2,
                    ReferencePhoto = referencePhoto,
                    SimpleLineId = userSeries.ReferencePhotoForSeries.SimpleLine.SimpleLineId,
                };

                referencePhoto.SimpleLine = simpleLineForReferencePhoto;

                singleSeries.ReferencePhotoForSeries = referencePhoto;
            }

            singleSeries.DropPhotosSeries = dropPhotosSeries;
            singleSeries.IntervalBetweenPhotos = userSeries.IntervalBetweenPhotos;
            singleSeries.AddedDate = userSeries.AddedDate;
            singleSeries.SeriesId = userSeries.SeriesId;
            singleSeries.Title = userSeries.Title;
            singleSeries.CurrentUser = UserToDbUser(user);
            singleSeries.SeriesId = user.UserId;

            return singleSeries;
        }

        public static ObservableCollection<Series> DbSeriesToSeries(List<DbSeries> series, User user)
        {
            ObservableCollection<Series> addSeriesViewModel = new ObservableCollection<Series>();
            if (series != null)
            {
                for (int i = 0; i < series.Count; i++)
                {
                    Series addSingleSeriesViewModel = new Series(user);
                    ObservableCollection<DropPhoto> dropPhotosSeries = new ObservableCollection<DropPhoto>();

                    if (series[i].DropPhotosSeries != null)
                    {
                        foreach (var dropPhoto in series[i].DropPhotosSeries)
                        {
                            var userDropPhoto = new DropPhoto(addSingleSeriesViewModel, addSingleSeriesViewModel.SeriesId)
                            {
                                Name = dropPhoto.Name,
                                Content = dropPhoto.Content,
                                DropPhotoId = dropPhoto.DropPhotoId,
                                SimpleHorizontalLine = new SimpleLine
                                {
                                    X1 = dropPhoto.SimpleHorizontalLine.X1,
                                    X2 = dropPhoto.SimpleHorizontalLine.X2,
                                    Y1 = dropPhoto.SimpleHorizontalLine.Y1,
                                    Y2 = dropPhoto.SimpleHorizontalLine.Y2
                                },
                                SimpleVerticalLine = new SimpleLine
                                {
                                    X1 = dropPhoto.SimpleVerticalLine.X1,
                                    X2 = dropPhoto.SimpleVerticalLine.X2,
                                    Y1 = dropPhoto.SimpleVerticalLine.Y1,
                                    Y2 = dropPhoto.SimpleVerticalLine.Y2
                                },
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
                            var userDrop = new Drop(addSingleSeriesViewModel, userDropPhoto)
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
                        addSingleSeriesViewModel.ReferencePhotoForSeries = new ReferencePhoto(addSingleSeriesViewModel)
                        {
                            Content = series[i].ReferencePhotoForSeries.Content,
                            Name = series[i].ReferencePhotoForSeries.Name,
                            PixelsInMillimeter = series[i].ReferencePhotoForSeries.PixelsInMillimeter,
                            ReferencePhotoId = series[i].ReferencePhotoForSeries.ReferencePhotoId,
                            SimpleLine = new SimpleLine
                            {
                                X1 = series[i].ReferencePhotoForSeries.SimpleLine.X1,
                                X2 = series[i].ReferencePhotoForSeries.SimpleLine.X2,
                                Y1 = series[i].ReferencePhotoForSeries.SimpleLine.Y1,
                                Y2 = series[i].ReferencePhotoForSeries.SimpleLine.Y2
                            },
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
            }

            return addSeriesViewModel;
        }

        public static DbDropPhoto DropPhotoToDbDropPhoto(DropPhoto dropPhotoViewModel)
        {
            return new DbDropPhoto
            {
                AddedDate = dropPhotoViewModel.AddedDate,
                Content = dropPhotoViewModel.Content,
                Drop = new DbDrop
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
                SimpleHorizontalLine = new DbSimpleLine()
                {
                    X1 = dropPhotoViewModel.SimpleHorizontalLine.X1,
                    X2 = dropPhotoViewModel.SimpleHorizontalLine.X2,
                    Y1 = dropPhotoViewModel.SimpleHorizontalLine.Y1,
                    Y2 = dropPhotoViewModel.SimpleHorizontalLine.Y2,
                },
                SimpleVerticalLine = new DbSimpleLine()
                {
                    X1 = dropPhotoViewModel.SimpleVerticalLine.X1,
                    X2 = dropPhotoViewModel.SimpleVerticalLine.X2,
                    Y1 = dropPhotoViewModel.SimpleVerticalLine.Y1,
                    Y2 = dropPhotoViewModel.SimpleVerticalLine.Y2
                },
                XDiameterInPixels = dropPhotoViewModel.XDiameterInPixels,
                YDiameterInPixels = dropPhotoViewModel.YDiameterInPixels,
                ZDiameterInPixels = dropPhotoViewModel.ZDiameterInPixels
            };
        }
    }
}