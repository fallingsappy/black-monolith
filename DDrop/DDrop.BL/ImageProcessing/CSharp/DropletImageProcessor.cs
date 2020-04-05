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
    public class DropletImageProcessor : IDropletImageProcessor
    {
        public System.Drawing.Point[] GetDiameters(byte[] image)
        {          
            Mat inputMat = new Mat();

            CvInvoke.Imdecode(image, Emgu.CV.CvEnum.ImreadModes.Unchanged, inputMat);

            Image<Bgr, byte> imageInput = inputMat.ToImage<Bgr, byte>();

            Image<Gray, byte> grayImage = imageInput.Convert<Gray, byte>();

            Image<Gray, byte> bluredImage = grayImage;
            CvInvoke.MedianBlur(grayImage, bluredImage, 9);

            Image<Gray, byte> edgedImage = bluredImage;
            CvInvoke.Canny(bluredImage, edgedImage, 50, 100);

            Image<Gray, byte> closedImage = edgedImage;           
            Mat kernel = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Ellipse, new System.Drawing.Size { Height = 100, Width = 250}, new System.Drawing.Point(-1, -1)); 
            CvInvoke.MorphologyEx(edgedImage, closedImage, Emgu.CV.CvEnum.MorphOp.Close, kernel, new System.Drawing.Point(-1, -1), 0, Emgu.CV.CvEnum.BorderType.Default, new MCvScalar());

            Image<Bgr, byte> imageOut = imageInput;
            VectorOfVectorOfPoint rescontours1 = new VectorOfVectorOfPoint();
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                Mat dilate = new Mat();

                CvInvoke.Dilate(edgedImage, dilate, null, new System.Drawing.Point(-1, -1), 1, Emgu.CV.CvEnum.BorderType.Default, new MCvScalar(0, 0, 0));

                CvInvoke.FindContours(dilate, contours, null, Emgu.CV.CvEnum.RetrType.External,
                    Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
                MCvScalar color = new MCvScalar(0, 0, 255);

                int biggest = 0;
                int index = 0;

                int count = contours.Size;

                for (int i = 0; i < count; i++)
                {
                    using (VectorOfPoint contour = contours[i])
                    if (contour.Size > biggest)
                    {
                        biggest = contour.Size;
                        index = i;
                    }
                }

                rescontours1.Push(contours[index]);

                CvInvoke.DrawContours(imageOut, rescontours1, -1, color, 2);

            }

            imageOut.Save(@"E:\fallingsappy\Data\Programming\portfolio\DDrop\DDrop\Resources\PythonScripts\6.jpg");

            var arr = rescontours1.ToArrayOfArray();

            var s = arr[0];

            return arr[0];
        }
    }
}
