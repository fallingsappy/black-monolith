namespace DDrop.BL.ImageProcessing.CSharp
{
    public interface IDropletImageProcessor
    {
        System.Drawing.Point[] GetDiameters(byte[] image, int ksize = 9, int treshold1 = 50, int treshold2 = 5, int size1 = 100, int size2 = 250);
    }
}