using DDrop.BE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDrop.BL.DropPhoto
{
    public interface IDropPhotoBL
    {
        BE.Models.Entities.DropPhoto DropPhotoViewModelToDropPhoto(DropPhotoViewModel dropPhotoViewModel);
    }
}
