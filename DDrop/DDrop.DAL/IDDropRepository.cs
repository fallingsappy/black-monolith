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
        Task<DbSeries> GetSingleSeriesById(Guid dbSeriesId);
        Task CreateSeries(DbSeries series);
        Task DeleteSingleSeries(DbSeries series);
        Task CreateDropPhoto(DbDropPhoto dropPhoto);
        Task UpdatDropPhoto(DbDropPhoto dropPhoto);
        Task CreateReferencePhoto(DbReferencePhoto referencePhoto);
        Task CreateOrUpdateSimpleLine(List<DbSimpleLine> dbSimpleLines);
    }
}