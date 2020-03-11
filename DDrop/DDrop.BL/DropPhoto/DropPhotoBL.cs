using DDrop.BE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDrop.BE.Models.Serializable;

namespace DDrop.BL.DropPhoto
{
    public class DropPhotoBL : IDropPhotoBL
    {
        public SerializableDropPhoto DropPhotoViewModelToDropPhoto(BE.Models.DropPhoto dropPhotoViewModel)
        {
            return new SerializableDropPhoto
            {
                AddedDate = dropPhotoViewModel.AddedDate,
                Content = dropPhotoViewModel.Content,
                Drop = new SerializableDrop
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
