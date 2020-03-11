using DDrop.BE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDrop.BE.Models.Serializable;

namespace DDrop.BL.DropPhoto
{
    public interface IDropPhotoBL
    {
        SerializableDropPhoto DropPhotoViewModelToDropPhoto(BE.Models.DropPhoto dropPhotoViewModel);
    }
}
