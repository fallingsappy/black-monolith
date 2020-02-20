using DDrop.BE.Models;
using DDrop.Utility.ImageOperations;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace DDrop
{
    /// <summary>
    /// Логика взаимодействия для ManualEdit.xaml
    /// </summary>
    public partial class ManualEdit : Window
    {
        private int _initialXDiameterInPixels;
        private int _initialYDiameterInPixels;
        private bool _saveRequired = false;

        public static readonly DependencyProperty ImageForEditProperty = DependencyProperty.Register("ImageForEdit", typeof(ImageSource), typeof(ManualEdit));
        public static readonly DependencyProperty CurrentDropPhotoProperty = DependencyProperty.Register("CurrentDropPhoto", typeof(DropPhoto), typeof(ManualEdit));

        public DropPhoto CurrentDropPhoto
        {
            get { return (DropPhoto)GetValue(CurrentDropPhotoProperty); }
            set
            {
                SetValue(CurrentDropPhotoProperty, value);
            }
        }

        public ImageSource ImageForEdit
        {
            get { return (ImageSource)GetValue(ImageForEditProperty); }
            set
            {
                SetValue(ImageForEditProperty, value);
            }
        }

        public ManualEdit(DropPhoto dropPhoto)
        {
            InitializeComponent();
            EditWindowPixelDrawer.TwoLineMode = true;
            PhotoForEdit.ItemsSource = new ObservableCollection<DropPhoto>() { dropPhoto };
            ImageForEdit = ImageInterpreter.LoadImage(dropPhoto.Content);
            _initialXDiameterInPixels = dropPhoto.XDiameterInPixels;
            _initialYDiameterInPixels = dropPhoto.YDiameterInPixels;
            CurrentDropPhoto = dropPhoto;

            if (CurrentDropPhoto.HorizontalLine != null)
                EditWindowPixelDrawer.CanDrawing.Children.Add(CurrentDropPhoto.HorizontalLine);

            if (CurrentDropPhoto.VerticalLine != null)
                EditWindowPixelDrawer.CanDrawing.Children.Add(CurrentDropPhoto.VerticalLine);

            PixelsInMillimeterHorizontalTextBox.Text = _initialXDiameterInPixels.ToString();
            PixelsInMillimeterVerticalTextBox.Text = _initialYDiameterInPixels.ToString();
        }

        private void SaveInputPhotoEditButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsSaveRequired())
            {
                SavePixelDiameters();
            }
        }

        private void SavePixelDiameters()
        {
            int xDiameterInPixelsTextBox = Convert.ToInt32(PixelsInMillimeterHorizontalTextBox.Text);
            int yDiameterInPixelsTextBox = Convert.ToInt32(PixelsInMillimeterVerticalTextBox.Text);

            CurrentDropPhoto.Drop.RadiusInMeters = 0;
            CurrentDropPhoto.Drop.VolumeInCubicalMeters = 0;
            CurrentDropPhoto.Drop.XDiameterInMeters = 0;
            CurrentDropPhoto.Drop.YDiameterInMeters = 0;
            CurrentDropPhoto.Drop.ZDiameterInMeters = 0;

            if (_initialXDiameterInPixels != xDiameterInPixelsTextBox)
            {
                CurrentDropPhoto.XDiameterInPixels = xDiameterInPixelsTextBox;
                _initialXDiameterInPixels = xDiameterInPixelsTextBox;
            }

            if (_initialYDiameterInPixels != yDiameterInPixelsTextBox)
            {
                CurrentDropPhoto.YDiameterInPixels = yDiameterInPixelsTextBox;
                _initialYDiameterInPixels = yDiameterInPixelsTextBox;
            }

            _saveRequired = false;
        }

        private bool IsSaveRequired()
        {
            int xDiameterInPixelsTextBox;
            int yDiameterInPixelsTextBox;

            if (int.TryParse(PixelsInMillimeterHorizontalTextBox.Text, out xDiameterInPixelsTextBox) && int.TryParse(PixelsInMillimeterVerticalTextBox.Text, out yDiameterInPixelsTextBox))
            {
                if (_initialXDiameterInPixels != xDiameterInPixelsTextBox || _initialYDiameterInPixels != yDiameterInPixelsTextBox)
                {
                    return _saveRequired = true;
                }
            }

            return _saveRequired;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (IsSaveRequired())
            {
                if (MessageBox.Show("Сохранить изменения?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    SavePixelDiameters();
                }
            }

            EditWindowPixelDrawer.CanDrawing.Children.Remove(CurrentDropPhoto.HorizontalLine);
            EditWindowPixelDrawer.CanDrawing.Children.Remove(CurrentDropPhoto.VerticalLine);
        }

        private void VerticalRulerToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            HorizontalRulerToggleButton.IsChecked = false;
            EditWindowPixelDrawer._drawingVerticalLine = true;
        }

        private void VerticalRulerToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            EditWindowPixelDrawer._drawingVerticalLine = false;
        }

        private void HorizontalRulerToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            VerticalRulerToggleButton.IsChecked = false;
            EditWindowPixelDrawer._drawingHorizontalLine = true;
        }

        private void HorizontalRulerToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            EditWindowPixelDrawer._drawingHorizontalLine = false;
        }
    }
}
