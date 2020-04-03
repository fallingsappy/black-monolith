using System;
using System.Collections.Generic;
using System.Threading;
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
        Task<DbSeries> GetFullDbSeriesForExportById(Guid seriesId);
        Task CreateSeries(DbSeries series);
        Task DeleteSingleSeries(DbSeries series);
        Task CreateFullSeries(DbSeries series);
        Task UseCreationDateTime(bool useCreationDateTime, Guid seriesId);
        Task UpdateSeriesName(string seriesName, Guid seriesId);
        Task UpdateSeriesIntervalBetweenPhotos(int interval, Guid seriesId);
        Task CreateDropPhoto(DbDropPhoto dropPhoto, Guid seriesId);
        Task UpdateDropPhoto(DbDropPhoto dropPhoto);
        Task DeleteDropPhoto(Guid dropPhotoId);
        Task UpdateDropPhotoName(string newName, Guid dropPhotoId);
        Task<byte[]> GetDropPhotoContent(Guid dropPhotoId, CancellationToken cancellationToken);
        Task UpdatePhotosOrderInSeries(List<DbDropPhoto> newDbDropPhotos);
        Task DeleteReferencePhoto(Guid dbReferencePhotoId);
        Task UpdateReferencePhoto(DbReferencePhoto referencePhoto);
        Task<byte[]> GetReferencePhotoContent(Guid referencePhotoId);
    }
}