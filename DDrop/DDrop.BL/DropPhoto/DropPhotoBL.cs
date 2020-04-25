using System.Threading;
using System.Threading.Tasks;
using DDrop.DAL;
using DDrop.Utility.Mappers;

namespace DDrop.BL.DropPhoto
{
    public class DropPhotoBL : IDropPhotoBL
    {
        private readonly IDDropRepository _dDropRepository;

        public DropPhotoBL(IDDropRepository dDropRepository)
        {
            _dDropRepository = dDropRepository;
        }

        public async Task UpdateDropPhoto(BE.Models.DropPhoto dropPhoto)
        {
            if (dropPhoto.Content == null)
                dropPhoto.Content =
                    await _dDropRepository.GetDropPhotoContent(dropPhoto.DropPhotoId, CancellationToken.None);

            var dbPhoto = DDropDbEntitiesMapper.DropPhotoToDbDropPhoto(dropPhoto, dropPhoto.CurrentSeriesId);

            await Task.Run(() => _dDropRepository.UpdateDropPhoto(dbPhoto));
        }
    }
}