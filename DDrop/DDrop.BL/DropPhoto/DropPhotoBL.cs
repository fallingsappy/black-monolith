using DDrop.BE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDrop.BL.DropPhoto
{
    public class DropPhotoBL : IDropPhotoBL
    {
        public BE.Models.Entities.DropPhoto DropPhotoViewModelToDropPhoto(DropPhotoViewModel dropPhotoViewModel)
        {
            return new BE.Models.Entities.DropPhoto
            {
                AddedDate = dropPhotoViewModel.AddedDate,
                Content = dropPhotoViewModel.Content,
                Drop = new BE.Models.Entities.Drop
                {
                    DropId = dropPhotoViewModel.Drop.DropId,
                    RadiusInMeters = dropPhotoViewModel.Drop.RadiusInMeters,
                    VolumeInCubicalMeters = dropPhotoViewModel.Drop.VolumeInCubicalMeters,
                    XDiameterInMeters = dropPhotoViewModel.Drop.XDiameterInMeters,
                    YDiameterInMeters = dropPhotoViewModel.Drop.YDiameterInMeters,
                    ZDiameterInMeters = dropPhotoViewModel.Drop.ZDiameterInMeters
                },
                DropPhotoId = dropPhotoViewModel.DropPhotoId,
                Name = dropPhotoViewModel.Name,
                SimpleHorizontalLine = dropPhotoViewModel.SimpleHorizontalLine,
                SimpleVerticalLine = dropPhotoViewModel.SimpleVerticalLine,
                XDiameterInPixels = dropPhotoViewModel.XDiameterInPixels,
                YDiameterInPixels = dropPhotoViewModel.YDiameterInPixels,
                ZDiameterInPixels = dropPhotoViewModel.ZDiameterInPixels
            };
        }
    }
}
