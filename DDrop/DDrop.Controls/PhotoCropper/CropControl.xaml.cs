using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DDrop.Controls.PhotoCropper
{
    /// <summary>
    /// Логика взаимодействия для CropControl.xaml
    /// </summary>
    public partial class CropControl : UserControl
    {
        public CropControl()
        {
            InitializeComponent();
        }

        public CroppingAdorner CroppingAdorner;
        public static readonly DependencyProperty SquareModeProperty = DependencyProperty.Register("SquareMode", typeof(bool), typeof(CropControl));
        public bool SquareMode
        {
            get { return (bool)GetValue(CropControl.SquareModeProperty); }
            set
            {
                SetValue(CropControl.SquareModeProperty, value);
            }
        }

        private void RootGrid_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CroppingAdorner.CaptureMouse();
            CroppingAdorner.MouseLeftButtonDownEventHandler(sender, e);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(CanvasPanel);
            CroppingAdorner = new CroppingAdorner(CanvasPanel, SquareMode);
            adornerLayer.Add(CroppingAdorner);
        }
    }
}
