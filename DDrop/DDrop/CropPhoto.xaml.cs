using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace DDrop
{
    /// <summary>
    ///     Логика взаимодействия для CropPhoto.xaml
    /// </summary>
    public partial class CropPhoto : Window
    {
        public static readonly DependencyProperty UserPhotoForCroppProperty =
            DependencyProperty.Register("UserPhotoForCropp", typeof(byte[]), typeof(CropPhoto));

        public CropPhoto()
        {
            InitializeComponent();
        }

        public byte[] UserPhotoForCropp
        {
            get => (byte[]) GetValue(UserPhotoForCroppProperty);
            set => SetValue(UserPhotoForCroppProperty, value);
        }

        private void CropPhotoButton_OnClick(object sender, RoutedEventArgs e)
        {
            var croppedBitmapFrame = CroppingControl.CroppingAdorner.GetCroppedBitmapFrame();
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(croppedBitmapFrame));
            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);
                var bit = stream.ToArray();
                UserPhotoForCropp = bit;
                stream.Close();
            }

            DialogResult = true;
            Close();
        }
    }
}