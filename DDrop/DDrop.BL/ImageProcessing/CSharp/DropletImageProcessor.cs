﻿using System;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace DDrop.BL.ImageProcessing.CSharp
{
    public class DropletImageProcessor : IDropletImageProcessor
    {
        public System.Drawing.Point[] GetDiameters(byte[] image, int ksize = 9, int treshold1 = 50, int treshold2 = 100, int size1 = 100, int size2 = 250)
        {          
            Mat inputMat = new Mat();

            CvInvoke.Imdecode(image, Emgu.CV.CvEnum.ImreadModes.Unchanged, inputMat);

            Image<Bgr, byte> imageInput = inputMat.ToImage<Bgr, byte>();

            Image<Gray, byte> grayImage = imageInput.Convert<Gray, byte>();

            Image<Gray, byte> bluredImage = grayImage;
            CvInvoke.MedianBlur(grayImage, bluredImage, ksize);

            Image<Gray, byte> edgedImage = bluredImage;
            CvInvoke.Canny(bluredImage, edgedImage, treshold1, treshold2);

            Image<Gray, byte> closedImage = edgedImage;           
            Mat kernel = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Ellipse, new System.Drawing.Size { Height = size1, Width = size2 }, new System.Drawing.Point(-1, -1)); 
            CvInvoke.MorphologyEx(edgedImage, closedImage, Emgu.CV.CvEnum.MorphOp.Close, kernel, new System.Drawing.Point(-1, -1), 0, Emgu.CV.CvEnum.BorderType.Default, new MCvScalar());

            Image<Bgr, byte> imageOut = imageInput;
            VectorOfVectorOfPoint resultingContour = new VectorOfVectorOfPoint();
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

                if (count == 0)
                    throw new InvalidOperationException("Не удалось построить контур.");

                for (int i = 0; i < count; i++)
                {
                    using (VectorOfPoint contour = contours[i])
                        if (contour.Size > biggest)
                        {
                            biggest = contour.Size;
                            index = i;
                        }
                }

                resultingContour.Push(contours[index]);

                CvInvoke.DrawContours(imageOut, resultingContour, -1, color, 2);
            }
            
            var arr = resultingContour.ToArrayOfArray();

            return arr[0];
        }
    }
}
