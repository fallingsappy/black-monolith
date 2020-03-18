using System;
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
            var dbUser = new DbUser
            {
                Email = user.Email,
                Password = user.Password,
                UserPhoto = user.UserPhoto,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserId = user.UserId,
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
                IsLoggedIn = true,
            };

            user.UserSeries = DbSeriesToSeries(dbUser.UserSeries, user);

            return user;
        }

        public static List<DbSeries> SeriesToDbSeries(ObservableCollection<Series> series ,DbUser dbUser)
        {
            List<DbSeries> dbSeries = new List<DbSeries>();
            foreach (var userSeries in series)
            {
                dbSeries.Add(SingleSeriesToSingleDbSeries(userSeries, dbUser));
            }

            return dbSeries;
        }

        public static DbSeries SingleSeriesToSingleDbSeries(Series userSeries, DbUser user)
        {
            DbSeries singleSeries = new DbSeries();
            singleSeries.IntervalBetweenPhotos = userSeries.IntervalBetweenPhotos;
            singleSeries.AddedDate = userSeries.AddedDate;
            singleSeries.SeriesId = userSeries.SeriesId;
            singleSeries.Title = userSeries.Title;
            singleSeries.CurrentUser = user;
            singleSeries.CurrentUserId = user.UserId;
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
                    CurrentSeries = singleSeries,
                    CurrentSeriesId = singleSeries.SeriesId,
                };
                if (dropPhoto.SimpleHorizontalLine != null)
                {
                    DbSimpleLine newHorizontalDbSimpleLine = new DbSimpleLine
                    {
                        X1 = dropPhoto.SimpleHorizontalLine.X1,
                        X2 = dropPhoto.SimpleHorizontalLine.X2,
                        Y1 = dropPhoto.SimpleHorizontalLine.Y1,
                        Y2 = dropPhoto.SimpleHorizontalLine.Y2,
                        SimpleLineId = dropPhoto.SimpleHorizontalLine.SimpleLineId,
                    };

                    newDbDropPhoto.SimpleHorizontalLine = newHorizontalDbSimpleLine;
                    newDbDropPhoto.SimpleHorizontalLineId = newHorizontalDbSimpleLine.SimpleLineId;
                }

                if (dropPhoto.SimpleVerticalLine != null)
                {
                    DbSimpleLine newVerticalDbSimpleLine = new DbSimpleLine
                    {
                        X1 = dropPhoto.SimpleVerticalLine.X1,
                        X2 = dropPhoto.SimpleVerticalLine.X2,
                        Y1 = dropPhoto.SimpleVerticalLine.Y1,
                        Y2 = dropPhoto.SimpleVerticalLine.Y2,
                        SimpleLineId = dropPhoto.SimpleVerticalLine.SimpleLineId,
                    };

                    newDbDropPhoto.SimpleVerticalLine = newVerticalDbSimpleLine;
                    newDbDropPhoto.SimpleVerticalLineId = newVerticalDbSimpleLine.SimpleLineId;
                }


                DbDrop newDbDrop = new DbDrop()
                {
                    DropId = dropPhoto.Drop.DropId,
                    RadiusInMeters = dropPhoto.Drop.RadiusInMeters,
                    VolumeInCubicalMeters = dropPhoto.Drop.VolumeInCubicalMeters,
                    XDiameterInMeters = dropPhoto.Drop.XDiameterInMeters,
                    YDiameterInMeters = dropPhoto.Drop.YDiameterInMeters,
                    ZDiameterInMeters = dropPhoto.Drop.ZDiameterInMeters,
                    DropPhoto = newDbDropPhoto,
                };

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
                    Series = singleSeries,                    
                };

                if (userSeries.ReferencePhotoForSeries.SimpleLine != null)
                {
                    var simpleLineForReferencePhoto = new DbSimpleLine
                    {
                        X1 = userSeries.ReferencePhotoForSeries.SimpleLine.X1,
                        X2 = userSeries.ReferencePhotoForSeries.SimpleLine.X2,
                        Y1 = userSeries.ReferencePhotoForSeries.SimpleLine.Y1,
                        Y2 = userSeries.ReferencePhotoForSeries.SimpleLine.Y2,
                        SimpleLineId = userSeries.ReferencePhotoForSeries.SimpleLine.SimpleLineId,
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
            ObservableCollection<Series> addSeriesViewModel = new ObservableCollection<Series>();
            if (series != null)
            {
                for (int i = 0; i < series.Count; i++)
                {
                    Series addSingleSeriesViewModel = new Series();
                    addSingleSeriesViewModel.AddedDate = series[i].AddedDate;
                    addSingleSeriesViewModel.Title = series[i].Title;
                    
                    addSingleSeriesViewModel.IntervalBetweenPhotos = series[i].IntervalBetweenPhotos;
                    addSingleSeriesViewModel.SeriesId = series[i].SeriesId;
                    addSingleSeriesViewModel.CurrentUser = user;
                    addSingleSeriesViewModel.CurrentUserId = user.UserId;
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
                                XDiameterInPixels = dropPhoto.XDiameterInPixels,
                                YDiameterInPixels = dropPhoto.YDiameterInPixels,
                                ZDiameterInPixels = dropPhoto.ZDiameterInPixels,
                                AddedDate = dropPhoto.AddedDate,
                                CurrentSeries = addSingleSeriesViewModel,
                                CurrentSeriesId = addSingleSeriesViewModel.SeriesId,
                            };

                            if (dropPhoto.SimpleHorizontalLine != null)
                            {
                                var newSimpleHorizontalLine = new SimpleLine
                                {
                                    X1 = dropPhoto.SimpleHorizontalLine.X1,
                                    X2 = dropPhoto.SimpleHorizontalLine.X2,
                                    Y1 = dropPhoto.SimpleHorizontalLine.Y1,
                                    Y2 = dropPhoto.SimpleHorizontalLine.Y2,
                                    SimpleLineId = dropPhoto.SimpleHorizontalLine.SimpleLineId
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
                                    SimpleLineId = dropPhoto.SimpleVerticalLine.SimpleLineId
                                };
                                userDropPhoto.SimpleVerticalLine = newSimpleVerticalLine;
                                userDropPhoto.SimpleVerticalLineId = dropPhoto.SimpleHorizontalLineId ?? Guid.NewGuid();
                            }

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
                                DropPhoto = userDropPhoto
                            };

                            userDropPhoto.Drop = userDrop;

                            dropPhotosSeries.Add(userDropPhoto);
                        }
                    }

                    addSingleSeriesViewModel.DropPhotosSeries = dropPhotosSeries;

                    if (series[i].ReferencePhotoForSeries != null)
                    {
                        addSingleSeriesViewModel.ReferencePhotoForSeries = new ReferencePhoto()
                        {
                            Content = series[i].ReferencePhotoForSeries.Content,
                            Name = series[i].ReferencePhotoForSeries.Name,
                            PixelsInMillimeter = series[i].ReferencePhotoForSeries.PixelsInMillimeter,
                            ReferencePhotoId = series[i].ReferencePhotoForSeries.ReferencePhotoId,
                            Series = addSingleSeriesViewModel,
                        };

                        if (series[i].ReferencePhotoForSeries.SimpleReferencePhotoLine != null)
                        {
                            var newSimpleLine = new SimpleLine
                            {
                                X1 = series[i].ReferencePhotoForSeries.SimpleReferencePhotoLine.X1,
                                X2 = series[i].ReferencePhotoForSeries.SimpleReferencePhotoLine.X2,
                                Y1 = series[i].ReferencePhotoForSeries.SimpleReferencePhotoLine.Y1,
                                Y2 = series[i].ReferencePhotoForSeries.SimpleReferencePhotoLine.Y2,
                                SimpleLineId = series[i].ReferencePhotoForSeries.SimpleReferencePhotoLine.SimpleLineId
                            };
                            addSingleSeriesViewModel.ReferencePhotoForSeries.SimpleLine = newSimpleLine;
                            addSingleSeriesViewModel.ReferencePhotoForSeries.SimpleReferencePhotoLineId = newSimpleLine.SimpleLineId;
                        }
                    }

                    if (series[i].ReferencePhotoForSeries?.SimpleReferencePhotoLine != null)
                    {
                        addSingleSeriesViewModel.ReferencePhotoForSeries.Line = new Line
                        {
                            X1 = series[i].ReferencePhotoForSeries.SimpleReferencePhotoLine.X1,
                            X2 = series[i].ReferencePhotoForSeries.SimpleReferencePhotoLine.X2,
                            Y1 = series[i].ReferencePhotoForSeries.SimpleReferencePhotoLine.Y1,
                            Y2 = series[i].ReferencePhotoForSeries.SimpleReferencePhotoLine.Y2,
                            Stroke = Brushes.DeepPink
                        };
                    }


                    addSeriesViewModel.Add(addSingleSeriesViewModel);
                }
            }

            return addSeriesViewModel;
        }

        public static DbDropPhoto DropPhotoToDbDropPhoto(DropPhoto dropPhotoViewModel, DbSeries dbSeries)
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
                CurrentSeries = dbSeries,
                CurrentSeriesId = dbSeries.SeriesId,
            };

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

            if (dropPhotoViewModel.SimpleHorizontalLine != null)
            {
                var newDbSimpleHorizontalLine = new DbSimpleLine()
                {
                    X1 = dropPhotoViewModel.SimpleHorizontalLine.X1,
                    X2 = dropPhotoViewModel.SimpleHorizontalLine.X2,
                    Y1 = dropPhotoViewModel.SimpleHorizontalLine.Y1,
                    Y2 = dropPhotoViewModel.SimpleHorizontalLine.Y2,
                    SimpleLineId = dropPhotoViewModel.SimpleHorizontalLine.SimpleLineId,
                };

                dbDropPhoto.SimpleHorizontalLineId = newDbSimpleHorizontalLine.SimpleLineId;
                dbDropPhoto.SimpleHorizontalLine = newDbSimpleHorizontalLine;
            }

            if (dbDropPhoto.SimpleHorizontalLine != null)
            {
                var newDbSimpleVerticalLine = new DbSimpleLine()
                {
                    X1 = dropPhotoViewModel.SimpleVerticalLine.X1,
                    X2 = dropPhotoViewModel.SimpleVerticalLine.X2,
                    Y1 = dropPhotoViewModel.SimpleVerticalLine.Y1,
                    Y2 = dropPhotoViewModel.SimpleVerticalLine.Y2,
                    SimpleLineId = dropPhotoViewModel.SimpleVerticalLine.SimpleLineId,
                };

                dbDropPhoto.SimpleVerticalLineId = newDbSimpleVerticalLine.SimpleLineId;
                dbDropPhoto.SimpleVerticalLine = newDbSimpleVerticalLine;
            }
           
            return dbDropPhoto;
        }

        public static DbReferencePhoto ReferencePhotoToDbReferencePhoto(ReferencePhoto referencePhoto, DbSeries dbSeries)
        {
            var dbReferencePhoto = new DbReferencePhoto
            {
                Content = referencePhoto.Content,
                ReferencePhotoId = referencePhoto.ReferencePhotoId,
                Name = referencePhoto.Name,              
                Series = dbSeries,
                PixelsInMillimeter = referencePhoto.PixelsInMillimeter
            };

            if (referencePhoto.SimpleLine != null)
            {
                var newDbSimpleLine = new DbSimpleLine()
                {
                    X1 = referencePhoto.SimpleLine.X1,
                    X2 = referencePhoto.SimpleLine.X2,
                    Y1 = referencePhoto.SimpleLine.Y1,
                    Y2 = referencePhoto.SimpleLine.Y2,
                    SimpleLineId = referencePhoto.SimpleLine.SimpleLineId,
                };

                dbReferencePhoto.SimpleReferencePhotoLineId = newDbSimpleLine.SimpleLineId;
                dbReferencePhoto.SimpleReferencePhotoLine = newDbSimpleLine;
            }

            return dbReferencePhoto;
        }      
    }
}