using System;
using System.Data.Entity;
using DDrop.Db.DbEntities;
using System.Threading.Tasks;
using DDrop.Db;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Data.Entity.Infrastructure;

namespace DDrop.DAL
{
    public class DDropRepository : IDDropRepository
    {
        #region User

        public async Task CreateUserAsync(DbUser user)
        {
            using (var context = new DDropContext())
            {               
                try
                {
                    var createdUser = context.Users.Add(user);

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

        public async Task<DbUser> GetUserByLogin(string email)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    return await context.Users.FirstOrDefaultAsync(x => x.Email == email);
                }
                catch (ArgumentNullException e)
                {
                    throw new ArgumentNullException(e.Message);
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidOperationException(e.Message);
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
                    var createdSeries = context.Series.Add(series);

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

        public List<DbSeries> GetSeriesByUserId(Guid dbUserId)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    var loadedSeries = context.Series.Where(x => x.CurrentUserId == dbUserId)
                    .Select(x => new {
                        x.Title,
                        x.SeriesId,
                        x.IntervalBetweenPhotos,
                        x.AddedDate,
                        x.CurrentUserId,
                        x.CurrentUser,
                    }).ToList();

                    List<DbSeries> dbSeries = new List<DbSeries>();

                    foreach (var series in loadedSeries)
                    {
                        var referencePhotoForSeries = context.ReferencePhotos.Where(x => x.Series.SeriesId == series.SeriesId)
                            .Select(x => new
                            {
                                x.Name,
                                x.PixelsInMillimeter,
                                x.SimpleReferencePhotoLine,
                                x.SimpleReferencePhotoLineId,
                                x.ReferencePhotoId,
                                x.Series,
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
                                x.AddedDate
                            }).ToList();

                        List<DbDropPhoto> dbDropPhotoForAdd = new List<DbDropPhoto>();

                        if (dropPhotoForSeries != null)
                        {
                            foreach (var dropPhoto in dropPhotoForSeries)
                            {
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
                                    ZDiameterInPixels = dropPhoto.ZDiameterInPixels
                                });
                            }
                        }

                        dbSeries.Add(new DbSeries
                        {
                            Title = series.Title,
                            AddedDate = series.AddedDate,
                            CurrentUser = series.CurrentUser,
                            CurrentUserId = series.CurrentUserId,
                            IntervalBetweenPhotos = series.IntervalBetweenPhotos,
                            SeriesId = series.SeriesId,
                            ReferencePhotoForSeries = referencePhotoForSeries != null ? new DbReferencePhoto
                            {
                                Name = referencePhotoForSeries.Name,
                                PixelsInMillimeter = referencePhotoForSeries.PixelsInMillimeter,
                                ReferencePhotoId = referencePhotoForSeries.ReferencePhotoId,
                                SimpleReferencePhotoLine = referencePhotoForSeries.SimpleReferencePhotoLine,
                                SimpleReferencePhotoLineId = referencePhotoForSeries.SimpleReferencePhotoLineId,
                                Series = referencePhotoForSeries.Series
                            } : null,
                            DropPhotosSeries = dbDropPhotoForAdd != null ? dbDropPhotoForAdd : null
                        });
                    }

                    return dbSeries;
                }
                catch (ArgumentNullException e)
                {
                    throw new ArgumentNullException(e.Message);
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
                    }

                    if (series.ReferencePhotoForSeries != null && series.ReferencePhotoForSeries.SimpleReferencePhotoLine != null)
                        context.SimpleLines.Remove(series.ReferencePhotoForSeries.SimpleReferencePhotoLine);

                    var createdSeries = context.Series.Remove(series);

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

        public Task<DbSeries> GetSingleSeriesById(Guid dbSeriesId)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    return context.Series
                        .Include(x => x.CurrentUser)
                        .Include(x => x.DropPhotosSeries)
                        .Include(x => x.ReferencePhotoForSeries)
                        .FirstOrDefaultAsync();
                }
                catch (ArgumentNullException e)
                {
                    throw new ArgumentNullException(e.Message);
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidOperationException(e.Message);
                }
            }
        }

        #endregion

        #region Drop Photo

        public async Task CreateDropPhoto(DbDropPhoto dropPhoto)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    context.Series.Attach(dropPhoto.CurrentSeries);
                    var createdDropPhoto = context.DropPhotos.Add(dropPhoto);

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

        public async Task UpdatDropPhoto(DbDropPhoto dropPhoto)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    context.Set<DbSimpleLine>().AddOrUpdate(dropPhoto.SimpleHorizontalLine);
                    context.Set<DbSimpleLine>().AddOrUpdate(dropPhoto.SimpleVerticalLine);
                    context.Set<DbDropPhoto>().AddOrUpdate(dropPhoto);
                    context.Set<DbDrop>().AddOrUpdate(dropPhoto.Drop);
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

        public async Task DeleteDropPhoto(Guid dropPhotoId)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    var dropPhoto = await context.DropPhotos.FirstOrDefaultAsync(x => x.DropPhotoId == dropPhotoId);

                    context.DropPhotos.Attach(dropPhoto);
                    if (dropPhoto.SimpleVerticalLine != null)
                        context.SimpleLines.Remove(dropPhoto.SimpleVerticalLine);
                    if (dropPhoto.SimpleHorizontalLine != null)
                        context.SimpleLines.Remove(dropPhoto.SimpleHorizontalLine);
                    context.Drops.Remove(dropPhoto.Drop);
                    context.DropPhotos.Remove(dropPhoto);

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

        public async Task<byte[]> GetDropPhotoContent(Guid dropPhotoId)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    return await context.DropPhotos.Where(x => x.DropPhotoId == dropPhotoId).Select(z => z.Content).FirstOrDefaultAsync();
                }
                catch (ArgumentNullException e)
                {
                    throw new ArgumentNullException(e.Message);
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidOperationException(e.Message);
                }
            }
        }

        #endregion

        #region Reference Photo

        public async Task CreateReferencePhoto(DbReferencePhoto referencePhoto)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    context.Series.Attach(referencePhoto.Series);
                    var createdReferencePhoto = context.ReferencePhotos.Add(referencePhoto);

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
        
        public async Task UpdateReferencePhoto(DbReferencePhoto referencePhoto)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    context.Entry(referencePhoto.SimpleReferencePhotoLine).State = EntityState.Modified;
                    context.Set<DbSimpleLine>().AddOrUpdate(referencePhoto.SimpleReferencePhotoLine);
                    context.Set<DbReferencePhoto>().AddOrUpdate(referencePhoto);
                    
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

        public async Task DeleteReferencePhoto(Guid dbReferencePhotoId)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    var referencePhoto = await context.ReferencePhotos.FirstOrDefaultAsync(x => x.ReferencePhotoId == dbReferencePhotoId);

                    context.ReferencePhotos.Attach(referencePhoto);
                    context.SimpleLines.Remove(referencePhoto.SimpleReferencePhotoLine);
                    context.ReferencePhotos.Remove(referencePhoto);

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

        public async Task<byte[]> GetReferencePhotoContent(Guid referencePhotoId)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    return await context.ReferencePhotos.Where(x => x.ReferencePhotoId == referencePhotoId).Select(z => z.Content).FirstOrDefaultAsync();
                }
                catch (ArgumentNullException e)
                {
                    throw new ArgumentNullException(e.Message);
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidOperationException(e.Message);
                }
            }
        }

        #endregion
    }
}