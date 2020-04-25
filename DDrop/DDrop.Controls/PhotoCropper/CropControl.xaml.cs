using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace DDrop.Controls.PhotoCropper
{
    /// <summary>
    ///     Логика взаимодействия для CropControl.xaml
    /// </summary>
    public partial class CropControl : UserControl
    {
        public static readonly DependencyProperty SquareModeProperty =
            DependencyProperty.Register("SquareMode", typeof(bool), typeof(CropControl));

        public CroppingAdorner CroppingAdorner;

        public CropControl()
        {
            InitializeComponent();
        }

        public bool SquareMode
        {
            get => (bool) GetValue(SquareModeProperty);
            set => SetValue(SquareModeProperty, value);
        }

        private void RootGrid_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CroppingAdorner.CaptureMouse();
            CroppingAdorner.MouseLeftButtonDownEventHandler(sender, e);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(CanvasPanel);
            CroppingAdorner = new CroppingAdorner(CanvasPanel, SquareMode);
            adornerLayer.Add(CroppingAdorner);
        }
    }
}