using System.Threading.Tasks;

namespace DDrop.BL.DropPhoto
{
    public interface IDropPhotoBL
    {
        Task UpdateDropPhoto(BE.Models.DropPhoto dropPhoto);
    }
}