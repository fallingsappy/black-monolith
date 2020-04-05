namespace DDrop.BL.ImageProcessing.CSharp
{
    public interface IDropletImageProcessor
    {
        System.Drawing.Point[] GetDiameters(byte[] image);
    }
}