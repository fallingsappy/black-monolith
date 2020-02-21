using DDrop.Utility.ImageOperations;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;

namespace DDrop.BL.ImageProcessing.CSharp
{
    class DropletImageProcessor
    {
        public List<int> GetDiameters(byte[] inputPhoto, string outputPath)
        {
            List<int> result = new List<int>();

            string path = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo di = Directory.CreateDirectory(path + @"\Temp\");
            File.WriteAllBytes($@"{path}\Temp\1.jpg", inputPhoto);
            string inputFile = $@"{path}\Temp\1.jpg";

            Image<Bgr, byte> imageInput = new Image<Bgr, byte>(inputFile);

            Image<Gray, byte> grayImage = imageInput.Convert<Gray, byte>();
            Image<Gray, byte> bluredImage = grayImage;
            CvInvoke.MedianBlur(grayImage, bluredImage, 9);
            Image<Gray, byte> edgedImage = bluredImage;
            CvInvoke.Canny(bluredImage, edgedImage, 50, 5);
            Image<Gray, byte> closedImage = edgedImage;
            Mat kernel = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Ellipse, System.Drawing.Size.Empty, new System.Drawing.Point(100, 250));
            CvInvoke.MorphologyEx(edgedImage, closedImage, Emgu.CV.CvEnum.MorphOp.Close, kernel, new System.Drawing.Point(100, 250), 10000, Emgu.CV.CvEnum.BorderType.Default, new MCvScalar());
            Image<Gray, byte> contours = closedImage;
            Image<Gray, byte> hierarchy = closedImage;
            CvInvoke.FindContours(closedImage, contours, hierarchy, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);

            //while (contours != null)
            //{
            //    contours = contours.

            //}

            //CvInvoke.ApproxPolyDP()

            return result;
        }
    }
}
