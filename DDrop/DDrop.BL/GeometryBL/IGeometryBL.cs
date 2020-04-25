using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Shapes;
using DDrop.BE.Enums.Options;
using DDrop.Controls.PixelDrawer;

namespace DDrop.BL.GeometryBL
{
    public interface IGeometryBL
    {
        void PrepareLines(BE.Models.DropPhoto selectedPhoto, out Line horizontalLine, out Line verticalLine,
            bool showLinesOnPreview);

        void CreateDiameters(BE.Models.DropPhoto dropPhoto, Point[] points);
        void RestoreOriginalLines(BE.Models.DropPhoto dropPhoto, BE.Models.DropPhoto storedPhoto, Canvas canvas);

        void PrepareContour(BE.Models.DropPhoto selectedPhoto, out ObservableCollection<Line> contour,
            bool showContourOnPreview);

        void CreateContour(BE.Models.DropPhoto dropPhoto, Point[] points,
            CalculationVariants calculationVariant, string cShrpKsize, string cShrpSize1, string cShrpSize2,
            string cShrpThreshold1, string cShrpThreshold2, string pythonKSise, string pythonSize1, string pythonSize2,
            string pythonThreshold1,
            string pythonThreshold2, BE.Models.DropPhoto currentDropPhoto, PixelDrawer imgCurrent);

        void StoreContour(BE.Models.DropPhoto dropPhoto, BE.Models.DropPhoto storeTo);

        void RestoreOriginalContour(BE.Models.DropPhoto dropPhoto, BE.Models.DropPhoto storedPhoto, Canvas canvas,
            Guid currentDropPhotoId);
    }
}