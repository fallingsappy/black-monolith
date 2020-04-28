using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DDrop.Db;
using DDrop.Db.DbEntities;

namespace DDrop.DAL
{
    public class DDropRepository : IDDropRepository
    {
        #region Logger

        public async Task SaveLogEntry(DbLogEntry dbLogEntry)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    context.LogEntries.Add(dbLogEntry);

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        #endregion

        #region User

        public async Task CreateUserAsync(DbUser user)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    context.Users.Add(user);

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task UpdateUserAsync(DbUser user)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    var userToUpdate = await context.Users.FirstOrDefaultAsync(x => x.UserId == user.UserId);

                    context.Entry(userToUpdate).CurrentValues.SetValues(user);

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task<DbUser> GetUserByLogin(string email)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    return await context.Users.FirstOrDefaultAsync(x => x.Email == email);
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        #endregion

        #region Series

        public async Task CreateSeries(DbSeries series)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    context.Users.Attach(series.CurrentUser);
                    context.Series.Add(series);

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public List<DbSeries> GetSeriesByUserId(Guid dbUserId)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    var loadedSeries = context.Series.Where(x => x.CurrentUserId == dbUserId)
                        .Select(x => new
                        {
                            x.Title,
                            x.SeriesId,
                            x.IntervalBetweenPhotos,
                            x.AddedDate,
                            x.CurrentUserId,
                            x.CurrentUser,
                            x.UseCreationDateTime
                        }).ToList();

                    var dbSeries = new List<DbSeries>();

                    foreach (var series in loadedSeries)
                    {
                        var referencePhotoForSeries = context.ReferencePhotos
                            .Where(x => x.Series.SeriesId == series.SeriesId)
                            .Select(x => new
                            {
                                x.Name,
                                x.PixelsInMillimeter,
                                x.SimpleReferencePhotoLine,
                                x.SimpleReferencePhotoLineId,
                                x.ReferencePhotoId,
                                x.Series
                            }).FirstOrDefault();

                        var dropPhotoForSeries = context.DropPhotos.Where(x => x.CurrentSeriesId == series.SeriesId)
                            .Select(x => new
                            {
                                x.Name,
                                x.CurrentSeries,
                                x.CurrentSeriesId,
                                x.Drop,
                                x.DropPhotoId,
                                x.SimpleHorizontalLine,
                                x.SimpleHorizontalLineId,
                                x.SimpleVerticalLine,
                                x.SimpleVerticalLineId,
                                x.XDiameterInPixels,
                                x.YDiameterInPixels,
                                x.ZDiameterInPixels,
                                x.AddedDate,
                                x.CreationDateTime,
                                x.PhotoOrderInSeries,
                                x.Contour
                            }).ToList();

                        var dbDropPhotoForAdd = new List<DbDropPhoto>();

                        foreach (var dropPhoto in dropPhotoForSeries)
                        {
                            if (dropPhoto.Contour != null)
                                dropPhoto.Contour.SimpleLines = context.SimpleLines
                                    .Where(x => x.ContourId == dropPhoto.Contour.ContourId).ToList();

                            dbDropPhotoForAdd.Add(new DbDropPhoto
                            {
                                AddedDate = dropPhoto.AddedDate,
                                CurrentSeries = dropPhoto.CurrentSeries,
                                CurrentSeriesId = dropPhoto.CurrentSeriesId,
                                Drop = dropPhoto.Drop,
                                DropPhotoId = dropPhoto.DropPhotoId,
                                Name = dropPhoto.Name,
                                SimpleHorizontalLine = dropPhoto.SimpleHorizontalLine,
                                SimpleHorizontalLineId = dropPhoto.SimpleHorizontalLineId,
                                SimpleVerticalLine = dropPhoto.SimpleVerticalLine,
                                SimpleVerticalLineId = dropPhoto.SimpleVerticalLineId,
                                XDiameterInPixels = dropPhoto.XDiameterInPixels,
                                YDiameterInPixels = dropPhoto.YDiameterInPixels,
                                ZDiameterInPixels = dropPhoto.ZDiameterInPixels,
                                CreationDateTime = dropPhoto.CreationDateTime,
                                PhotoOrderInSeries = dropPhoto.PhotoOrderInSeries,
                                Contour = dropPhoto.Contour
                            });
                        }

                        dbSeries.Add(new DbSeries
                        {
                            Title = series.Title,
                            AddedDate = series.AddedDate,
                            UseCreationDateTime = series.UseCreationDateTime,
                            CurrentUser = series.CurrentUser,
                            CurrentUserId = series.CurrentUserId,
                            IntervalBetweenPhotos = series.IntervalBetweenPhotos,
                            SeriesId = series.SeriesId,
                            ReferencePhotoForSeries = referencePhotoForSeries != null
                                ? new DbReferencePhoto
                                {
                                    Name = referencePhotoForSeries.Name,
                                    PixelsInMillimeter = referencePhotoForSeries.PixelsInMillimeter,
                                    ReferencePhotoId = referencePhotoForSeries.ReferencePhotoId,
                                    SimpleReferencePhotoLine = referencePhotoForSeries.SimpleReferencePhotoLine,
                                    SimpleReferencePhotoLineId = referencePhotoForSeries.SimpleReferencePhotoLineId,
                                    Series = referencePhotoForSeries.Series
                                }
                                : null,
                            DropPhotosSeries = dbDropPhotoForAdd.OrderBy(x => x.PhotoOrderInSeries).ToList()
                        });
                    }

                    return dbSeries;
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task<DbSeries> GetFullDbSeriesForExportById(Guid seriesId)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    var series = await context.Series.Where(x => x.SeriesId == seriesId)
                        .Select(x => new
                        {
                            x.Title,
                            x.IntervalBetweenPhotos,
                            x.UseCreationDateTime
                        }).FirstOrDefaultAsync();

                    var referencePhotoForSeries = context.ReferencePhotos.Where(x => x.Series.SeriesId == seriesId)
                        .Select(x => new
                        {
                            x.Name,
                            x.PixelsInMillimeter,
                            x.SimpleReferencePhotoLine,
                            x.Content
                        }).FirstOrDefault();

                    var dropPhotoForSeries = context.DropPhotos.Where(x => x.CurrentSeriesId == seriesId)
                        .Select(x => new
                        {
                            x.Name,
                            x.Content,
                            x.SimpleHorizontalLine,
                            x.SimpleVerticalLine,
                            x.XDiameterInPixels,
                            x.YDiameterInPixels,
                            x.ZDiameterInPixels,
                            x.CreationDateTime,
                            x.PhotoOrderInSeries,
                            x.DropPhotoId
                        }).ToList();

                    var dbDropPhotoForAdd = new List<DbDropPhoto>();

                    foreach (var dropPhoto in dropPhotoForSeries)
                    {
                        var drop = await context.Drops.Where(x => x.DropPhoto.DropPhotoId == dropPhoto.DropPhotoId)
                            .Select(x => new
                            {
                                x.RadiusInMeters,
                                x.VolumeInCubicalMeters,
                                x.XDiameterInMeters,
                                x.YDiameterInMeters,
                                x.ZDiameterInMeters
                            }).FirstOrDefaultAsync();

                        var contour = await context.Contours.Where(x => x.ContourId == dropPhoto.DropPhotoId)
                            .Select(x => new
                            {
                                x.ContourId,
                                x.CalculationParameters,
                                x.CalculationProvider,
                                x.SimpleLines
                            }).FirstOrDefaultAsync();

                        DbContour contourForAdd = null;

                        if (contour != null)
                        {
                            contourForAdd = new DbContour
                            {
                                CalculationParameters = contour.CalculationParameters,
                                CalculationProvider = contour.CalculationProvider,
                                SimpleLines = new List<DbSimpleLine>()
                            };

                            foreach (var dbSimpleLine in contour.SimpleLines)
                            {
                                contourForAdd.SimpleLines.Add(new DbSimpleLine
                                {
                                    X1 = dbSimpleLine.X1,
                                    X2 = dbSimpleLine.X2,
                                    Y1 = dbSimpleLine.Y1,
                                    Y2 = dbSimpleLine.Y2
                                });
                            }
                        }

                        dbDropPhotoForAdd.Add(new DbDropPhoto
                        {
                            Drop = new DbDrop
                            {
                                RadiusInMeters = drop?.RadiusInMeters,
                                VolumeInCubicalMeters = drop.VolumeInCubicalMeters,
                                XDiameterInMeters = drop.XDiameterInMeters,
                                YDiameterInMeters = drop.YDiameterInMeters,
                                ZDiameterInMeters = drop.ZDiameterInMeters
                            },
                            Content = dropPhoto.Content,
                            Name = dropPhoto.Name,
                            SimpleHorizontalLine = dropPhoto.SimpleHorizontalLine != null
                                ? new DbSimpleLine
                                {
                                    X1 = dropPhoto.SimpleHorizontalLine.X1,
                                    Y1 = dropPhoto.SimpleHorizontalLine.Y1,
                                    Y2 = dropPhoto.SimpleHorizontalLine.Y2,
                                    X2 = dropPhoto.SimpleHorizontalLine.X2
                                }
                                : null,
                            SimpleVerticalLine = dropPhoto.SimpleVerticalLine != null
                                ? new DbSimpleLine
                                {
                                    X1 = dropPhoto.SimpleVerticalLine.X1,
                                    Y1 = dropPhoto.SimpleVerticalLine.Y1,
                                    Y2 = dropPhoto.SimpleVerticalLine.Y2,
                                    X2 = dropPhoto.SimpleVerticalLine.X2
                                }
                                : null,
                            XDiameterInPixels = dropPhoto.XDiameterInPixels,
                            YDiameterInPixels = dropPhoto.YDiameterInPixels,
                            ZDiameterInPixels = dropPhoto.ZDiameterInPixels,
                            CreationDateTime = dropPhoto.CreationDateTime,
                            PhotoOrderInSeries = dropPhoto.PhotoOrderInSeries,
                            Contour = contourForAdd
                        });
                    }

                    DbReferencePhoto referencePhotoForAdd = null;

                    if (referencePhotoForSeries != null)
                    {
                        referencePhotoForAdd = new DbReferencePhoto
                        {
                            Name = referencePhotoForSeries.Name,
                            PixelsInMillimeter = referencePhotoForSeries.PixelsInMillimeter,
                            Content = referencePhotoForSeries.Content
                        };

                        if (referencePhotoForSeries.SimpleReferencePhotoLine != null)
                        {
                            var simpleReferencePhotoLineForAdd = new DbSimpleLine
                            {
                                X1 = referencePhotoForSeries.SimpleReferencePhotoLine.X1,
                                Y1 = referencePhotoForSeries.SimpleReferencePhotoLine.Y1,
                                X2 = referencePhotoForSeries.SimpleReferencePhotoLine.X2,
                                Y2 = referencePhotoForSeries.SimpleReferencePhotoLine.Y2
                            };

                            referencePhotoForAdd.SimpleReferencePhotoLine = simpleReferencePhotoLineForAdd;
                        }
                    }

                    return new DbSeries
                    {
                        Title = series?.Title,
                        UseCreationDateTime = series.UseCreationDateTime,
                        IntervalBetweenPhotos = series.IntervalBetweenPhotos,
                        ReferencePhotoForSeries = referencePhotoForAdd,
                        DropPhotosSeries = dbDropPhotoForAdd.OrderBy(x => x.PhotoOrderInSeries).ToList()
                    };
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task CreateFullSeries(DbSeries series)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    context.Users.Attach(series.CurrentUser);
                    context.Series.Add(series);

                    await context.SaveChangesAsync();
                }
                catch (DbUpdateException e)
                {
                    throw new DbUpdateException(e.Message);
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidOperationException(e.Message);
                }
            }
        }

        public async Task DeleteSingleSeries(DbSeries series)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    context.Series.Attach(series);

                    foreach (var dropPhoto in series.DropPhotosSeries)
                    {
                        if (dropPhoto.SimpleVerticalLine != null)
                            context.SimpleLines.Remove(dropPhoto.SimpleVerticalLine);

                        if (dropPhoto.SimpleHorizontalLine != null)
                            context.SimpleLines.Remove(dropPhoto.SimpleHorizontalLine);

                        if (dropPhoto.Contour != null)
                        {
                            context.SimpleLines.RemoveRange(dropPhoto.Contour.SimpleLines);
                            context.Contours.Remove(dropPhoto.Contour);
                        }
                    }

                    if (series.ReferencePhotoForSeries != null &&
                        series.ReferencePhotoForSeries.SimpleReferencePhotoLine != null)
                        context.SimpleLines.Remove(series.ReferencePhotoForSeries.SimpleReferencePhotoLine);

                    context.Series.Remove(series);

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task UpdateSeriesName(string seriesName, Guid seriesId)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    var series = await context.Series.FirstOrDefaultAsync(x => x.SeriesId == seriesId);

                    if (series != null) series.Title = seriesName;

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task UseCreationDateTime(bool useCreationDateTime, Guid seriesId)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    var series = await context.Series.FirstOrDefaultAsync(x => x.SeriesId == seriesId);

                    if (series != null) series.UseCreationDateTime = useCreationDateTime;

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task UpdateSeriesIntervalBetweenPhotos(int interval, Guid seriesId)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    var series = await context.Series.FirstOrDefaultAsync(x => x.SeriesId == seriesId);

                    if (series != null) series.IntervalBetweenPhotos = interval;

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        #endregion

        #region Drop Photo

        public async Task CreateDropPhoto(DbDropPhoto dropPhoto, Guid seriesId)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    var dbDropPhoto = new DbDropPhoto
                    {
                        AddedDate = dropPhoto.AddedDate,
                        Content = dropPhoto.Content,
                        CurrentSeriesId = seriesId,
                        Drop = new DbDrop
                        {
                            DropId = dropPhoto.DropPhotoId,
                            RadiusInMeters = dropPhoto.Drop.RadiusInMeters,
                            VolumeInCubicalMeters = dropPhoto.Drop.VolumeInCubicalMeters,
                            XDiameterInMeters = dropPhoto.Drop.XDiameterInMeters,
                            YDiameterInMeters = dropPhoto.Drop.YDiameterInMeters,
                            ZDiameterInMeters = dropPhoto.Drop.ZDiameterInMeters
                        },
                        DropPhotoId = dropPhoto.DropPhotoId,
                        Name = dropPhoto.Name,
                        SimpleHorizontalLine = dropPhoto.SimpleHorizontalLine,
                        SimpleHorizontalLineId = dropPhoto.SimpleHorizontalLineId,
                        SimpleVerticalLine = dropPhoto.SimpleVerticalLine,
                        SimpleVerticalLineId = dropPhoto.SimpleVerticalLineId,
                        XDiameterInPixels = dropPhoto.XDiameterInPixels,
                        YDiameterInPixels = dropPhoto.YDiameterInPixels,
                        ZDiameterInPixels = dropPhoto.ZDiameterInPixels,
                        CreationDateTime = dropPhoto.CreationDateTime,
                        PhotoOrderInSeries = dropPhoto.PhotoOrderInSeries
                    };

                    context.DropPhotos.Add(dbDropPhoto);

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task UpdateDropPhoto(DbDropPhoto dropPhoto)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    var contour = await context.Contours.Include(p => p.SimpleLines)
                        .FirstOrDefaultAsync(x => x.ContourId == dropPhoto.DropPhotoId);

                    if (contour != null && dropPhoto.Contour != null)
                    {
                        contour.SimpleLines = dropPhoto.Contour.SimpleLines;
                        contour.CalculationParameters = dropPhoto.Contour.CalculationParameters;
                        contour.CalculationProvider = dropPhoto.Contour.CalculationProvider;

                        context.Set<DbContour>().AddOrUpdate(contour);
                        context.Set<DbSimpleLine>().AddRange(contour.SimpleLines);
                    }
                    else if (dropPhoto.Contour != null)
                    {
                        context.Set<DbContour>().AddOrUpdate(dropPhoto.Contour);
                        context.Set<DbSimpleLine>().AddRange(dropPhoto.Contour.SimpleLines);
                    }
                    else if (contour != null && dropPhoto.Contour == null)
                    {
                        context.Contours.Remove(contour);
                        context.SimpleLines.RemoveRange(contour.SimpleLines);
                    }

                    if (dropPhoto.SimpleHorizontalLine != null)
                        context.Set<DbSimpleLine>().AddOrUpdate(dropPhoto.SimpleHorizontalLine);

                    if (dropPhoto.SimpleVerticalLine != null)
                        context.Set<DbSimpleLine>().AddOrUpdate(dropPhoto.SimpleVerticalLine);
                    context.Set<DbDropPhoto>().AddOrUpdate(dropPhoto);
                    context.Set<DbDrop>().AddOrUpdate(dropPhoto.Drop);

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task UpdateDrop(DbDrop drop)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    context.Set<DbDrop>().AddOrUpdate(drop);

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task UpdateDropPhotoName(string newName, Guid dropPhotoId)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    var dropPhoto = await context.DropPhotos.FirstOrDefaultAsync(x => x.DropPhotoId == dropPhotoId);

                    if (dropPhoto != null) dropPhoto.Name = newName;

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task DeleteDropPhoto(Guid dropPhotoId)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    var dropPhoto = await context.DropPhotos.FirstOrDefaultAsync(x => x.DropPhotoId == dropPhotoId);
                    var contour = await context.Contours.Include(p => p.SimpleLines)
                        .FirstOrDefaultAsync(x => x.ContourId == dropPhoto.DropPhotoId);

                    context.DropPhotos.Attach(dropPhoto ?? throw new InvalidOperationException());
                    if (dropPhoto.SimpleVerticalLine != null)
                        context.SimpleLines.Remove(dropPhoto.SimpleVerticalLine);
                    if (dropPhoto.SimpleHorizontalLine != null)
                        context.SimpleLines.Remove(dropPhoto.SimpleHorizontalLine);
                    if (contour != null)
                    {
                        context.SimpleLines.RemoveRange(contour.SimpleLines);
                        context.Contours.Remove(contour);
                    }

                    context.Drops.Remove(dropPhoto.Drop);
                    context.DropPhotos.Remove(dropPhoto);

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task<byte[]> GetDropPhotoContent(Guid dropPhotoId, CancellationToken cancellationToken)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    var content = await context.DropPhotos.Where(x => x.DropPhotoId == dropPhotoId)
                        .Select(z => z.Content).FirstOrDefaultAsync().ConfigureAwait(true);

                    cancellationToken.ThrowIfCancellationRequested();

                    return content;
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task UpdatePhotosOrderInSeries(List<DbDropPhoto> newDbDropPhotos)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    foreach (var dropPhoto in newDbDropPhotos)
                    {
                        context.DropPhotos.Attach(dropPhoto);
                        context.Entry(dropPhoto).Property(x => x.PhotoOrderInSeries).IsModified = true;
                    }

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        #endregion

        #region Reference Photo

        public async Task UpdateReferencePhoto(DbReferencePhoto referencePhoto)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    if (referencePhoto.SimpleReferencePhotoLine != null)
                    {
                        context.Entry(referencePhoto.SimpleReferencePhotoLine).State = EntityState.Modified;
                        context.Set<DbSimpleLine>().AddOrUpdate(referencePhoto.SimpleReferencePhotoLine);
                    }

                    context.Set<DbReferencePhoto>().AddOrUpdate(referencePhoto);

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task DeleteReferencePhoto(Guid dbReferencePhotoId)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    var referencePhoto =
                        await context.ReferencePhotos.FirstOrDefaultAsync(x =>
                            x.ReferencePhotoId == dbReferencePhotoId);

                    context.ReferencePhotos.Attach(referencePhoto ?? throw new InvalidOperationException());
                    if (referencePhoto.SimpleReferencePhotoLine != null)
                        context.SimpleLines.Remove(referencePhoto.SimpleReferencePhotoLine);
                    context.ReferencePhotos.Remove(referencePhoto);

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task<byte[]> GetReferencePhotoContent(Guid referencePhotoId)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    return await context.ReferencePhotos.Where(x => x.ReferencePhotoId == referencePhotoId)
                        .Select(z => z.Content).FirstOrDefaultAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        #endregion
    }
}