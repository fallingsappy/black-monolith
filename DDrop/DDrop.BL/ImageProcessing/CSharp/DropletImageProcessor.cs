using DDrop.Utility.ImageOperations;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using Emgu.CV.Util;

namespace DDrop.BL.ImageProcessing.CSharp
{
    public class DropletImageProcessor
    {
        public List<int> GetDiameters()
        {
            List<int> result = new List<int>();

            //string path = AppDomain.CurrentDomain.BaseDirectory;
            //DirectoryInfo di = Directory.CreateDirectory(path + @"\Temp\");
            //File.WriteAllBytes($@"{path}\Temp\1.jpg", inputPhoto);
            string inputFile = @"E:\fallingsappy\Data\Programming\portfolio\DDrop\DDrop\Resources\PythonScripts\IMG_8915.JPG";

            Image<Bgr, byte> imageInput = new Image<Bgr, byte>(inputFile);

            Image<Gray, byte> grayImage = imageInput.Convert<Gray, byte>();
            grayImage.Save(@"E:\fallingsappy\Data\Programming\portfolio\DDrop\DDrop\Resources\PythonScripts\1.jpg");
            Image<Gray, byte> bluredImage = grayImage;
            CvInvoke.MedianBlur(grayImage, bluredImage, 9);
            bluredImage.Save(@"E:\fallingsappy\Data\Programming\portfolio\DDrop\DDrop\Resources\PythonScripts\2.jpg");
            Image<Gray, byte> edgedImage = bluredImage;
            CvInvoke.Canny(bluredImage, edgedImage, 50, 5);
            edgedImage.Save(@"E:\fallingsappy\Data\Programming\portfolio\DDrop\DDrop\Resources\PythonScripts\3.jpg");
            Image<Gray, byte> closedImage = edgedImage;           
            Mat kernel = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Ellipse, new System.Drawing.Size { Height = 100, Width = 250}, new System.Drawing.Point(-1, -1)); 
            CvInvoke.MorphologyEx(edgedImage, closedImage, Emgu.CV.CvEnum.MorphOp.Close, kernel, new System.Drawing.Point(-1, -1), 0, Emgu.CV.CvEnum.BorderType.Replicate, new MCvScalar());
            // edgedImage, closedImage, Emgu.CV.CvEnum.MorphOp.Close, kernel, new System.Drawing.Point(100, 250), 10000, Emgu.CV.CvEnum.BorderType.Default, new MCvScalar()
            closedImage.Save(@"E:\fallingsappy\Data\Programming\portfolio\DDrop\DDrop\Resources\PythonScripts\4.jpg");
            Image<Gray, byte> contoursImage = closedImage;
            Image<Bgr, byte> imageOut = imageInput;
            VectorOfVectorOfPoint rescontours1 = new VectorOfVectorOfPoint();
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                //Mat hierarchy = new Mat();
                CvInvoke.FindContours(contoursImage, contours, null, Emgu.CV.CvEnum.RetrType.External,
                    Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
                MCvScalar color = new MCvScalar(0, 0, 255);

                int count = contours.Size;
                for (int i = 0; i < count; i++)
                {
                    using (VectorOfPoint contour = contours[i])
                        using (VectorOfPoint approxContour = new VectorOfPoint())
                        {
                            CvInvoke.ApproxPolyDP(contour, approxContour,
                                0.01 * CvInvoke.ArcLength(contour, true), true);

                            var area = CvInvoke.ContourArea(contour);

                        if (area > 0)
                        {
                            rescontours1.Push(approxContour);
                        }

                            CvInvoke.DrawContours(imageOut, rescontours1, -1, color, 2);
                        }
                    
                }
            }

            imageOut.Save(@"E:\fallingsappy\Data\Programming\portfolio\DDrop\DDrop\Resources\PythonScripts\6.jpg");

            return result;
        }
    }
}
