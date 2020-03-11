using System;
using System.Data.Entity;
using DDrop.Db.DbEntities;
using System.Threading.Tasks;
using DDrop.Db;

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

        #endregion

        public async Task CreateSeries(DbSeries series)
        {
            using (var context = new DDropContext())
            {
                var createdSeries = context.Series.Add(series);

                await context.SaveChangesAsync();
            }
        }

        public async Task CreateDropPhoto(DbDropPhoto dropPhoto)
        {
            using (var context = new DDropContext())
            {
                var createdDropPhoto = context.DropPhotos.Add(dropPhoto);

                await context.SaveChangesAsync();
            }
        }

        public async Task CreateReferencePhoto(DbReferencePhoto referencePhoto)
        {
            using (var context = new DDropContext())
            {
                var createdReferencePhoto = context.ReferencePhotos.Add(referencePhoto);

                await context.SaveChangesAsync();
            }
        }
    }
}