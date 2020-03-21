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
        Task UpdateSeriesName(string seriesName, Guid seriesId);
        Task UpdateSeriesIntervalBetweenPhotos(int interval, Guid seriesId);
        Task DeleteListOfDropPhoto(List<Guid> dropPhotoIds);
        Task CreateDropPhoto(DbDropPhoto dropPhoto, Guid seriesId);
        Task UpdatDropPhoto(DbDropPhoto dropPhoto);
        Task DeleteDropPhoto(Guid dropPhotoId);
        Task UpdateDropPhotoName(string newName, Guid dropPhotoId);
        Task<byte[]> GetDropPhotoContent(Guid dropPhotoId);
        Task CreateReferencePhoto(DbReferencePhoto referencePhoto);
        Task DeleteReferencePhoto(Guid dbReferencePhotoId);
        Task UpdateReferencePhoto(DbReferencePhoto referencePhoto);
        Task<byte[]> GetReferencePhotoContent(Guid referencePhotoId);
    }
}