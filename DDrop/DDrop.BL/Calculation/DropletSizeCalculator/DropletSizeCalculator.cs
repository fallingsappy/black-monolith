using DDrop.BE.Models;
using System;

namespace DDrop.BL.Calculation.DropletSizeCalculator
{
    public static class DropletSizeCalculator
    {
        public static Drop PerformCalculation(int pixelsInMillimeter, int xDiameterInPixels, int yDiameterInPixels, Series CurrentSerries, DropPhoto dropPhoto, int zDiameterInPixels = 0, bool exactCalculationModel = false)
        {
            if (exactCalculationModel)
            {
                return PerformExactCalculation(pixelsInMillimeter, xDiameterInPixels, yDiameterInPixels, zDiameterInPixels, CurrentSerries, dropPhoto);
            }
            else
            {
                return PerformCalculation(pixelsInMillimeter, xDiameterInPixels, yDiameterInPixels, CurrentSerries, dropPhoto);
            }
        }

        private static Drop PerformCalculation(int pixelsInMillimeter, int xDiameterInPixels, int yDiameterInPixels, Series CurrentSerries, DropPhoto dropPhoto)
        {
            double xDiameterInMeters = xDiameterInPixels / (double)pixelsInMillimeter / 1000;
            double yDiameterInMeters = yDiameterInPixels / (double)pixelsInMillimeter / 1000;
            double volume = Convert.ToDouble(4f / 3f * Math.PI * (Math.Pow(xDiameterInMeters, 2) * yDiameterInMeters) / 8);
            double radius = Math.Pow(3 * volume / (4 * Math.PI), 1f / 3f);

            dropPhoto.Drop.XDiameterInMeters = xDiameterInMeters;
            dropPhoto.Drop.YDiameterInMeters = yDiameterInMeters;
            dropPhoto.Drop.VolumeInCubicalMeters = volume;
            dropPhoto.Drop.RadiusInMeters = radius;

            return dropPhoto.Drop;
        }

        private static Drop PerformExactCalculation(int pixelsInMillimeter, int xDiameterInPixels, int yDiameterInPixels, int zDiameterInPixels, Series CurrentSerries, DropPhoto dropPhoto)
        {
            double xDiameterInMeters = xDiameterInPixels / (double)pixelsInMillimeter / 1000;
            double yDiameterInMeters = yDiameterInPixels / (double)pixelsInMillimeter / 1000;
            double zDiameterInMeters = zDiameterInPixels / (double)pixelsInMillimeter / 1000;
            double volume = Convert.ToDouble(4f / 3f * Math.PI * (xDiameterInMeters * yDiameterInMeters * zDiameterInMeters) / 8);
            double radius = Math.Pow(3 * volume / (4 * Math.PI), 1f / 3f);

            dropPhoto.Drop.XDiameterInMeters = xDiameterInMeters;
            dropPhoto.Drop.YDiameterInMeters = yDiameterInMeters;
            dropPhoto.Drop.VolumeInCubicalMeters = volume;
            dropPhoto.Drop.RadiusInMeters = radius;

            return dropPhoto.Drop;
        }
    }
}
