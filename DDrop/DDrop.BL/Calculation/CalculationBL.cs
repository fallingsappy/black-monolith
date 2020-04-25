using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using DDrop.BL.DropPhoto;
using DDrop.DAL;
using DDrop.Utility.Calculation;
using DDrop.Utility.Mappers;

namespace DDrop.BL.Calculation
{
    public class CalculationBL : ICalculationBL
    {
        private readonly IDropPhotoBL _dropPhotoBL;

        public CalculationBL(IDropPhotoBL dropPhotoBL)
        {
            _dropPhotoBL = dropPhotoBL;
        }

        public async Task CalculateDropParameters(BE.Models.DropPhoto dropPhoto, string pixelsInMillimeter,
            Guid currentDropPhotoId)
        {
            DropletSizeCalculator.DropletSizeCalculator.PerformCalculation(
                Convert.ToInt32(pixelsInMillimeter), dropPhoto.XDiameterInPixels,
                dropPhoto.YDiameterInPixels, dropPhoto);

            await _dropPhotoBL.UpdateDropPhoto(dropPhoto);

            if (dropPhoto.DropPhotoId != currentDropPhotoId) dropPhoto.Content = null;
        }

        public void ReCalculateAllParametersFromLines(BE.Models.DropPhoto dropPhoto, string pixelsInMillimeterTextBox)
        {
            if (dropPhoto.SimpleHorizontalLine != null)
            {
                var horizontalLineFirstPoint = new Point(Convert.ToInt32(dropPhoto.SimpleHorizontalLine.X1),
                    Convert.ToInt32(dropPhoto.SimpleHorizontalLine.Y1));
                var horizontalLineSecondPoint = new Point(Convert.ToInt32(dropPhoto.SimpleHorizontalLine.X2),
                    Convert.ToInt32(dropPhoto.SimpleHorizontalLine.Y2));
                dropPhoto.XDiameterInPixels =
                    LineLengthHelper.GetPointsOnLine(horizontalLineFirstPoint, horizontalLineSecondPoint).Count;
            }
            else
            {
                dropPhoto.XDiameterInPixels = 0;
            }

            if (dropPhoto.SimpleVerticalLine != null)
            {
                var verticalLineFirstPoint = new Point(Convert.ToInt32(dropPhoto.SimpleVerticalLine.X1),
                    Convert.ToInt32(dropPhoto.SimpleVerticalLine.Y1));
                var verticalLineSecondPoint = new Point(Convert.ToInt32(dropPhoto.SimpleVerticalLine.X2),
                    Convert.ToInt32(dropPhoto.SimpleVerticalLine.Y2));
                dropPhoto.YDiameterInPixels =
                    LineLengthHelper.GetPointsOnLine(verticalLineFirstPoint, verticalLineSecondPoint).Count;
            }
            else
            {
                dropPhoto.YDiameterInPixels = 0;
            }

            DropletSizeCalculator.DropletSizeCalculator.PerformCalculation(Convert.ToInt32(pixelsInMillimeterTextBox),
                dropPhoto.XDiameterInPixels, dropPhoto.YDiameterInPixels, dropPhoto);
        }
    }
}