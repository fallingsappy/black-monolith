using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace DDrop.BL.ImageProcessing.CSharp
{
    public class DropletImageProcessor : IDropletImageProcessor
    {
        public Point[] GetDiameters(byte[] image, int ksize = 9, int treshold1 = 50, int treshold2 = 100,
            int size1 = 100, int size2 = 250)
        {
            var inputMat = new Mat();

            CvInvoke.Imdecode(image, ImreadModes.Unchanged, inputMat);

            var imageInput = inputMat.ToImage<Bgr, byte>();

            var grayImage = imageInput.Convert<Gray, byte>();

            var bluredImage = grayImage;
            CvInvoke.MedianBlur(grayImage, bluredImage, ksize);

            var edgedImage = bluredImage;
            CvInvoke.Canny(bluredImage, edgedImage, treshold1, treshold2);

            var closedImage = edgedImage;
            var kernel = CvInvoke.GetStructuringElement(ElementShape.Ellipse, new Size {Height = size1, Width = size2},
                new Point(-1, -1));
            CvInvoke.MorphologyEx(edgedImage, closedImage, MorphOp.Close, kernel, new Point(-1, -1), 0,
                BorderType.Default, new MCvScalar());

            var imageOut = imageInput;
            var resultingContour = new VectorOfVectorOfPoint();
            using (var contours = new VectorOfVectorOfPoint())
            {
                var dilate = new Mat();

                CvInvoke.Dilate(edgedImage, dilate, null, new Point(-1, -1), 1, BorderType.Default,
                    new MCvScalar(0, 0, 0));

                CvInvoke.FindContours(dilate, contours, null, RetrType.External,
                    ChainApproxMethod.ChainApproxSimple);
                var color = new MCvScalar(0, 0, 255);

                var biggest = 0;
                var index = 0;

                var count = contours.Size;

                if (count == 0)
                    throw new InvalidOperationException("Не удалось построить контур.");

                for (var i = 0; i < count; i++)
                    using (var contour = contours[i])
                    {
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