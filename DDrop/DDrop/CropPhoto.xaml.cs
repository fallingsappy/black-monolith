using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DDrop
{
    /// <summary>
    /// Логика взаимодействия для CropPhoto.xaml
    /// </summary>
    public partial class CropPhoto : Window
    {
        public static readonly DependencyProperty UserPhotoForCroppProperty = DependencyProperty.Register("UserPhotoForCropp", typeof(byte[]), typeof(CropPhoto));

        public byte[] UserPhotoForCropp
        {
            get { return (byte[])GetValue(UserPhotoForCroppProperty); }
            set
            {
                SetValue(UserPhotoForCroppProperty, value);
            }
        }

        public CropPhoto()
        {
            InitializeComponent();
        }

        private void CropPhotoButton_OnClick(object sender, RoutedEventArgs e)
        {
            var croppedBitmapFrame = CroppingControl.CroppingAdorner.GetCroppedBitmapFrame();
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(croppedBitmapFrame));
            using (MemoryStream stream = new MemoryStream())
            {
                encoder.Save(stream);
                byte[] bit = stream.ToArray();
                UserPhotoForCropp = bit;
                stream.Close();
            }
            DialogResult = true;
            Close();
        }
    }
}
