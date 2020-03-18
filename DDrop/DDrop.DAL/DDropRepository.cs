using System;
using System.Data.Entity;
using DDrop.Db.DbEntities;
using System.Threading.Tasks;
using DDrop.Db;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;

namespace DDrop.DAL
{
    public class DDropRepository : IDDropRepository
    {
        #region User

        public async Task CreateUserAsync(DbUser user)
        {
            using (var context = new DDropContext())
            {
                var createdUser = context.Users.Add(user);

                try
                {
                    await context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
        }

        public async Task UpdateUserAsync(DbUser user)
        {
            using (var context = new DDropContext())
            {
                var userToUpdate = await context.Users.FirstOrDefaultAsync(x => x.UserId == user.UserId);

                try
                {
                    context.Entry(userToUpdate).CurrentValues.SetValues(user);

                    await context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
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
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
        }

        #endregion

        #region Series
        public async Task CreateSeries(DbSeries series)
        {
            using (var context = new DDropContext())
            {
                context.Users.Attach(series.CurrentUser);
                var createdSeries = context.Series.Add(series);

                await context.SaveChangesAsync();
            }
        }

        public List<DbSeries> GetSeriesByUserId(Guid dbUserId)
        {
            using (var context = new DDropContext())
            {
                return context.Series.Where(x => x.CurrentUserId == dbUserId)
                    .Include(c => c.ReferencePhotoForSeries)
                    .Include(c => c.ReferencePhotoForSeries.SimpleReferencePhotoLine)
                    .Include(c => c.DropPhotosSeries.Select(x => x.SimpleVerticalLine))
                    .Include(c => c.DropPhotosSeries.Select(x => x.SimpleHorizontalLine))
                    .Include(c => c.DropPhotosSeries.Select(x => x.Drop))
                    .Include(c => c.DropPhotosSeries.Select(x => x.CurrentSeries))
                    .Include(c => c.CurrentUser)
                    .ToList();
            }
        }

        public async Task DeleteSingleSeries(DbSeries series)
        {
            using (var context = new DDropContext())
            {
                context.Series.Attach(series);

                foreach (var dropPhoto in series.DropPhotosSeries)
                {
                    if (dropPhoto.SimpleVerticalLine != null)
                        context.SimpleLines.Remove(dropPhoto.SimpleVerticalLine);

                    if (dropPhoto.SimpleHorizontalLine != null)
                        context.SimpleLines.Remove(dropPhoto.SimpleHorizontalLine);
                }

                context.SimpleLines.Remove(series.ReferencePhotoForSeries.SimpleReferencePhotoLine);

                var createdSeries = context.Series.Remove(series);

                await context.SaveChangesAsync();
            }
        }

        public Task<DbSeries> GetSingleSeriesById(Guid dbSeriesId)
        {
            using (var context = new DDropContext())
            {
                return context.Series
                    .Include(x => x.CurrentUser)
                    .Include(x => x.DropPhotosSeries)
                    .Include(x => x.ReferencePhotoForSeries)
                    .FirstOrDefaultAsync();
            }
        }

        #endregion

        #region Drop Photo

        public async Task CreateDropPhoto(DbDropPhoto dropPhoto)
        {
            using (var context = new DDropContext())
            {
                context.Series.Attach(dropPhoto.CurrentSeries);
                var createdDropPhoto = context.DropPhotos.Add(dropPhoto);

                await context.SaveChangesAsync();
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

                catch (Exception ex)
                {

                }
            }
        }

        public async Task DeleteDropPhoto(Guid dropPhotoId)
        {
            using (var context = new DDropContext())
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
        }

        #endregion

        #region Reference Photo

        public async Task CreateReferencePhoto(DbReferencePhoto referencePhoto)
        {
            using (var context = new DDropContext())
            {
                context.Series.Attach(referencePhoto.Series);
                var createdReferencePhoto = context.ReferencePhotos.Add(referencePhoto);

                await context.SaveChangesAsync();
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

                catch (Exception ex)
                {

                }
            }
        }

        public async Task DeleteReferencePhoto(Guid dbReferencePhotoId)
        {
            using (var context = new DDropContext())
            {
                var referencePhoto = await context.ReferencePhotos.FirstOrDefaultAsync(x => x.ReferencePhotoId == dbReferencePhotoId);

                context.ReferencePhotos.Attach(referencePhoto);
                context.SimpleLines.Remove(referencePhoto.SimpleReferencePhotoLine);
                context.ReferencePhotos.Remove(referencePhoto);

                await context.SaveChangesAsync();
            }
        }

        #endregion
    }
}