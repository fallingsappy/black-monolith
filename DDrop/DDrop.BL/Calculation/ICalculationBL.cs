using System;
using System.Threading.Tasks;

namespace DDrop.BL.Calculation
{
    public interface ICalculationBL
    {
        Task CalculateDropParameters(BE.Models.DropPhoto dropPhoto, string pixelsInMillimeter, Guid currentDropPhotoId);
        void ReCalculateAllParametersFromLines(BE.Models.DropPhoto dropPhoto, string pixelsInMillimeterTextBox);
    }
}