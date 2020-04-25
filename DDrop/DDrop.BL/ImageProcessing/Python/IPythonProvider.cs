using System.Drawing;

namespace DDrop.BL.ImageProcessing.Python
{
    public interface IPythonProvider
    {
        Point[] GetDiameters(byte[] inputPhoto, string tempFileName, string scriptToRun, string interpreter,
            int ksize = 9, int treshold1 = 50, int treshold2 = 5, int size1 = 100, int size2 = 250);
    }
}