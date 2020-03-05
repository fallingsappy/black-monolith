using DDrop.BE.Models;
using System;

namespace DDrop.BL.Calculation.DropletSizeCalculator
{
    public static class DropletSizeCalculator
    {
        public static void PerformCalculation(int pixelsInMillimeter, int xDiameterInPixels, int yDiameterInPixels, SeriesViewModel CurrentSerries, DropPhotoViewModel dropPhoto, int zDiameterInPixels = 0, bool exactCalculationModel = false)
        {
            if (exactCalculationModel)
            {
                PerformExactCalculation(pixelsInMillimeter, xDiameterInPixels, yDiameterInPixels, zDiameterInPixels, CurrentSerries, dropPhoto);
            }
            else
            {
                PerformCalculation(pixelsInMillimeter, xDiameterInPixels, yDiameterInPixels, CurrentSerries, dropPhoto);
            }
        }

        private static void PerformCalculation(int pixelsInMillimeter, int xDiameterInPixels, int yDiameterInPixels, SeriesViewModel CurrentSerries, DropPhotoViewModel dropPhoto)
        {
            dropPhoto.Drop.XDiameterInMeters = xDiameterInPixels / (double)pixelsInMillimeter / 1000;
            dropPhoto.Drop.YDiameterInMeters = yDiameterInPixels / (double)pixelsInMillimeter / 1000;
            dropPhoto.Drop.VolumeInCubicalMeters = Convert.ToDouble(4f / 3f * Math.PI * (Math.Pow(dropPhoto.Drop.XDiameterInMeters, 2) * dropPhoto.Drop.YDiameterInMeters) / 8);
            dropPhoto.Drop.RadiusInMeters = Math.Pow(3 * dropPhoto.Drop.VolumeInCubicalMeters / (4 * Math.PI), 1f / 3f);
        }

        private static void PerformExactCalculation(int pixelsInMillimeter, int xDiameterInPixels, int yDiameterInPixels, int zDiameterInPixels, SeriesViewModel CurrentSerries, DropPhotoViewModel dropPhoto)
        {
            dropPhoto.Drop.XDiameterInMeters = xDiameterInPixels / (double)pixelsInMillimeter / 1000;
            dropPhoto.Drop.YDiameterInMeters = yDiameterInPixels / (double)pixelsInMillimeter / 1000;
            dropPhoto.Drop.ZDiameterInMeters = zDiameterInPixels / (double)pixelsInMillimeter / 1000;
            dropPhoto.Drop.VolumeInCubicalMeters = Convert.ToDouble(4f / 3f * Math.PI * (dropPhoto.Drop.XDiameterInMeters * dropPhoto.Drop.YDiameterInMeters * dropPhoto.Drop.ZDiameterInMeters) / 8);
            dropPhoto.Drop.RadiusInMeters = Math.Pow(3 * dropPhoto.Drop.VolumeInCubicalMeters / (4 * Math.PI), 1f / 3f);
        }
    }
}
