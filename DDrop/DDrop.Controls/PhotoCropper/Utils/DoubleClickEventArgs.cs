using System;
using System.Windows.Media.Imaging;

namespace DDrop.Controls.PhotoCropper.Utils
{
    public class DoubleClickEventArgs : EventArgs
    {
        public BitmapFrame BitmapFrame { get; set; }
    }
}