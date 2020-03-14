using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDrop.Db.DbEntities;

namespace DDrop.DAL
{
    public interface IDDropRepository
    {
        Task CreateUserAsync(DbUser user);
        Task UpdateUserAsync(DbUser user);
        Task<DbUser> GetUserByLogin(string email);
        List<DbSeries> GetSeriesByUserId(Guid dbUserId);
        Task CreateSeries(DbSeries series);
    }
}