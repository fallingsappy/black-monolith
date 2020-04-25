using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;

namespace DDrop.Utility.Calculation
{
    public class LineLengthHelper
    {
        [DllImport("gdi32.dll")]
        private static extern bool LineDDA(int nXStart, int nYStart, int nXEnd, int nYEnd, LineDdaProc lpLineFunc,
            IntPtr lpData);

        public static List<Point> GetPointsOnLine(System.Drawing.Point point1, System.Drawing.Point point2)
        {
            var points = new List<Point>();
            var handle = GCHandle.Alloc(points);

            try
            {
                LineDDA(point1.X, point1.Y, point2.X, point2.Y, GetPointsOnLineCallback, GCHandle.ToIntPtr(handle));
            }
            finally
            {
                handle.Free();
            }

            return points;
        }

        private static void GetPointsOnLineCallback(int x, int y, IntPtr lpData)
        {
            var handle = GCHandle.FromIntPtr(lpData);
            var points = (List<Point>) handle.Target;
            points.Add(new Point(x, y));
        }

        private delegate void LineDdaProc(int x, int y, IntPtr lpData);
    }
}