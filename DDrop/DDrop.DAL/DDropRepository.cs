using System;
using System.Data.Entity;
using DDrop.Db.DbEntities;
using System.Threading.Tasks;
using DDrop.Db;
using System.Collections.Generic;
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
                    .Include(c => c.ReferencePhotoForSeries.SimpleLine)
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
                var dropPhotoToUpdate = await context.DropPhotos.FirstOrDefaultAsync(x => x.DropPhotoId == dropPhoto.DropPhotoId);

                try
                {
                    context.Entry(dropPhotoToUpdate).CurrentValues.SetValues(dropPhoto);

                    await context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
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

        public async Task DeleteReferencePhoto(DbReferencePhoto dbReferencePhoto)
        {
            using (var context = new DDropContext())
            {
                context.Series.Attach(dbReferencePhoto.Series);
                context.ReferencePhotos.Add(dbReferencePhoto);
                var createdSeries = context.ReferencePhotos.Remove(dbReferencePhoto);

                await context.SaveChangesAsync();
            }
        }

        #endregion

        #region Simple Line

        public async Task CreateOrUpdateSimpleLine(List<DbSimpleLine> dbSimpleLines)
        {
            using (var context = new DDropContext())
            {
                foreach (var dbSimpleLine in dbSimpleLines)
                {
                    var dbSimpleLineToUpdate = await context.SimpleLines.FirstOrDefaultAsync(x => x.SimpleLineId == dbSimpleLine.SimpleLineId);

                    if (dbSimpleLineToUpdate != null)
                    {
                        try
                        {
                            context.Entry(dbSimpleLineToUpdate).CurrentValues.SetValues(dbSimpleLine);


                        }
                        catch (Exception e)
                        {
                            throw new Exception(e.Message);
                        }
                    }
                    else
                    {
                        //  context.SimpleLines.Attach(dbSimpleLine);

                        ////if (dbSimpleLine.DropPhotoHorizontalLine != null)
                        ////{
                        //if (dbSimpleLine.DropPhotoHorizontalLine != null)
                        //    dbSimpleLine.DropPhotoHorizontalLine = null;
                        //if (dbSimpleLine.DropPhotoVerticalLine != null)
                        //    dbSimpleLine.DropPhotoVerticalLine = null;
                        //if (dbSimpleLine.ReferencePhoto != null)
                        //    dbSimpleLine.ReferencePhoto = null;

                        context.SimpleLines.Add(dbSimpleLine);
                            //context.DropPhotos.Attach(dbSimpleLine.DropPhotoHorizontalLine);
                        //}
                            

                        //if (dbSimpleLine.DropPhotoVerticalLine != null)
                        //    context.DropPhotos.Attach(dbSimpleLine.DropPhotoVerticalLine);

                        //if (dbSimpleLine.ReferencePhoto != null)
                        //    context.ReferencePhotos.Attach(dbSimpleLine.ReferencePhoto);

                       // var createdSeries = context.SimpleLines.Add(dbSimpleLine);
                    }
                }


                await context.SaveChangesAsync();
            }
        }

        #endregion
    }
}