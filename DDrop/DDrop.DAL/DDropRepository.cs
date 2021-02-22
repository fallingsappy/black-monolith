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

                        var measurementForSeries = context.Measurements.Where(x => x.CurrentSeriesId == series.SeriesId)
                            .Select(x => new
                            {
                                x.Name,
                                x.CurrentSeries,
                                x.CurrentSeriesId,
                                x.Drop,
                                x.MeasurementId,
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

                        var dbMeasurementForAdd = new List<DbMeasurement>();

                        foreach (var measurement in measurementForSeries)
                        {
                            if (measurement.Contour != null)
                                measurement.Contour.SimpleLines = context.SimpleLines
                                    .Where(x => x.ContourId == measurement.Contour.ContourId).ToList();

                            dbMeasurementForAdd.Add(new DbMeasurement
                            {
                                AddedDate = measurement.AddedDate,
                                CurrentSeries = measurement.CurrentSeries,
                                CurrentSeriesId = measurement.CurrentSeriesId,
                                Drop = measurement.Drop,
                                MeasurementId = measurement.MeasurementId,
                                Name = measurement.Name,
                                SimpleHorizontalLine = measurement.SimpleHorizontalLine,
                                SimpleHorizontalLineId = measurement.SimpleHorizontalLineId,
                                SimpleVerticalLine = measurement.SimpleVerticalLine,
                                SimpleVerticalLineId = measurement.SimpleVerticalLineId,
                                XDiameterInPixels = measurement.XDiameterInPixels,
                                YDiameterInPixels = measurement.YDiameterInPixels,
                                ZDiameterInPixels = measurement.ZDiameterInPixels,
                                CreationDateTime = measurement.CreationDateTime,
                                PhotoOrderInSeries = measurement.PhotoOrderInSeries,
                                Contour = measurement.Contour
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
                            MeasurementsSeries = dbMeasurementForAdd.OrderBy(x => x.PhotoOrderInSeries).ToList()
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

                    var measurementForSeries = context.Measurements.Where(x => x.CurrentSeriesId == seriesId)
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
                            x.MeasurementId
                        }).ToList();

                    var dbMeasurementForAdd = new List<DbMeasurement>();

                    foreach (var measurement in measurementForSeries)
                    {
                        var drop = await context.Drops.Where(x => x.Measurement.MeasurementId == measurement.MeasurementId)
                            .Select(x => new
                            {
                                x.RadiusInMeters,
                                x.VolumeInCubicalMeters,
                                x.XDiameterInMeters,
                                x.YDiameterInMeters,
                                x.ZDiameterInMeters
                            }).FirstOrDefaultAsync();

                        var contour = await context.Contours.Where(x => x.ContourId == measurement.MeasurementId)
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

                        dbMeasurementForAdd.Add(new DbMeasurement
                        {
                            Drop = new DbDrop
                            {
                                RadiusInMeters = drop?.RadiusInMeters,
                                VolumeInCubicalMeters = drop.VolumeInCubicalMeters,
                                XDiameterInMeters = drop.XDiameterInMeters,
                                YDiameterInMeters = drop.YDiameterInMeters,
                                ZDiameterInMeters = drop.ZDiameterInMeters
                            },
                            Content = measurement.Content,
                            Name = measurement.Name,
                            SimpleHorizontalLine = measurement.SimpleHorizontalLine != null
                                ? new DbSimpleLine
                                {
                                    X1 = measurement.SimpleHorizontalLine.X1,
                                    Y1 = measurement.SimpleHorizontalLine.Y1,
                                    Y2 = measurement.SimpleHorizontalLine.Y2,
                                    X2 = measurement.SimpleHorizontalLine.X2
                                }
                                : null,
                            SimpleVerticalLine = measurement.SimpleVerticalLine != null
                                ? new DbSimpleLine
                                {
                                    X1 = measurement.SimpleVerticalLine.X1,
                                    Y1 = measurement.SimpleVerticalLine.Y1,
                                    Y2 = measurement.SimpleVerticalLine.Y2,
                                    X2 = measurement.SimpleVerticalLine.X2
                                }
                                : null,
                            XDiameterInPixels = measurement.XDiameterInPixels,
                            YDiameterInPixels = measurement.YDiameterInPixels,
                            ZDiameterInPixels = measurement.ZDiameterInPixels,
                            CreationDateTime = measurement.CreationDateTime,
                            PhotoOrderInSeries = measurement.PhotoOrderInSeries,
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
                        MeasurementsSeries = dbMeasurementForAdd.OrderBy(x => x.PhotoOrderInSeries).ToList()
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

                    foreach (var measurement in series.MeasurementsSeries)
                    {
                        if (measurement.SimpleVerticalLine != null)
                            context.SimpleLines.Remove(measurement.SimpleVerticalLine);

                        if (measurement.SimpleHorizontalLine != null)
                            context.SimpleLines.Remove(measurement.SimpleHorizontalLine);

                        if (measurement.Contour != null)
                        {
                            context.SimpleLines.RemoveRange(measurement.Contour.SimpleLines);
                            context.Contours.Remove(measurement.Contour);
                        }
                    }

                    if (series.ReferencePhotoForSeries != null &&
                        series.ReferencePhotoForSeries.SimpleReferencePhotoLine != null)
                        context.SimpleLines.Remove(series.ReferencePhotoForSeries.SimpleReferencePhotoLine);

                    context.Series.Remove(series);

                    bool saveFailed;

                    do
                    {
                        saveFailed = false;

                        try
                        {
                            await context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException ex)
                        {
                            saveFailed = true;

                            // Update the values of the entity that failed to save from the store
                            await ex.Entries.Single().ReloadAsync();
                        }

                    } while (saveFailed);

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

        public async Task CreateMeasurement(DbMeasurement measurement, Guid seriesId)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    var dbMeasurement = new DbMeasurement
                    {
                        AddedDate = measurement.AddedDate,
                        Content = measurement.Content,
                        CurrentSeriesId = seriesId,
                        Drop = new DbDrop
                        {
                            DropId = measurement.MeasurementId,
                            RadiusInMeters = measurement.Drop.RadiusInMeters,
                            VolumeInCubicalMeters = measurement.Drop.VolumeInCubicalMeters,
                            XDiameterInMeters = measurement.Drop.XDiameterInMeters,
                            YDiameterInMeters = measurement.Drop.YDiameterInMeters,
                            ZDiameterInMeters = measurement.Drop.ZDiameterInMeters
                        },
                        MeasurementId = measurement.MeasurementId,
                        Name = measurement.Name,
                        SimpleHorizontalLine = measurement.SimpleHorizontalLine,
                        SimpleHorizontalLineId = measurement.SimpleHorizontalLineId,
                        SimpleVerticalLine = measurement.SimpleVerticalLine,
                        SimpleVerticalLineId = measurement.SimpleVerticalLineId,
                        XDiameterInPixels = measurement.XDiameterInPixels,
                        YDiameterInPixels = measurement.YDiameterInPixels,
                        ZDiameterInPixels = measurement.ZDiameterInPixels,
                        CreationDateTime = measurement.CreationDateTime,
                        PhotoOrderInSeries = measurement.PhotoOrderInSeries
                    };

                    context.Measurements.Add(dbMeasurement);

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task UpdateMeasurement(DbMeasurement measurement)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    var contour = await context.Contours.Include(p => p.SimpleLines)
                        .FirstOrDefaultAsync(x => x.ContourId == measurement.MeasurementId);

                    if (contour != null && measurement.Contour != null)
                    {
                        context.SimpleLines.RemoveRange(contour.SimpleLines);
                        contour.SimpleLines = measurement.Contour.SimpleLines;
                        contour.CalculationParameters = measurement.Contour.CalculationParameters;
                        contour.CalculationProvider = measurement.Contour.CalculationProvider;

                        context.Set<DbContour>().AddOrUpdate(contour);
                        context.Set<DbSimpleLine>().AddRange(contour.SimpleLines);
                    }
                    else if (measurement.Contour != null)
                    {
                        context.Set<DbContour>().AddOrUpdate(measurement.Contour);
                        context.Set<DbSimpleLine>().AddRange(measurement.Contour.SimpleLines);
                    }
                    else if (contour != null && measurement.Contour == null)
                    {
                        context.Contours.Remove(contour);
                        context.SimpleLines.RemoveRange(contour.SimpleLines);
                    }

                    if (measurement.SimpleHorizontalLine != null)
                        context.Set<DbSimpleLine>().AddOrUpdate(measurement.SimpleHorizontalLine);

                    if (measurement.SimpleVerticalLine != null)
                        context.Set<DbSimpleLine>().AddOrUpdate(measurement.SimpleVerticalLine);
                    context.Set<DbMeasurement>().AddOrUpdate(measurement);
                    context.Set<DbDrop>().AddOrUpdate(measurement.Drop);

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

        public async Task UpdateMeasurementName(string newName, Guid measurementId)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    var measurement = await context.Measurements.FirstOrDefaultAsync(x => x.MeasurementId == measurementId);

                    if (measurement != null) measurement.Name = newName;

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task DeleteMeasurement(Guid measurementId)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    var measurement = await context.Measurements.FirstOrDefaultAsync(x => x.MeasurementId == measurementId);
                    var contour = await context.Contours.Include(p => p.SimpleLines)
                        .FirstOrDefaultAsync(x => x.ContourId == measurement.MeasurementId);

                    context.Measurements.Attach(measurement ?? throw new InvalidOperationException());
                    if (measurement.SimpleVerticalLine != null)
                        context.SimpleLines.Remove(measurement.SimpleVerticalLine);
                    if (measurement.SimpleHorizontalLine != null)
                        context.SimpleLines.Remove(measurement.SimpleHorizontalLine);
                    if (contour != null)
                    {
                        context.SimpleLines.RemoveRange(contour.SimpleLines);
                        context.Contours.Remove(contour);
                    }

                    context.Drops.Remove(measurement.Drop);
                    context.Measurements.Remove(measurement);

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task<byte[]> GetMeasurementContent(Guid measurementId, CancellationToken cancellationToken)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    var content = await context.Measurements.Where(x => x.MeasurementId == measurementId)
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

        public async Task UpdatePhotosOrderInSeries(List<DbMeasurement> newDbMeasurements)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    foreach (var measurement in newDbMeasurements)
                    {
                        context.Measurements.Attach(measurement);
                        context.Entry(measurement).Property(x => x.PhotoOrderInSeries).IsModified = true;
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