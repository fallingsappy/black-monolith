using System;
using System.Windows;

namespace DDrop.Utility.ExceptionHandling.Windows
{
    public partial class ExceptionWindow
    {
        public ExceptionWindow()
        {
            InitializeComponent();
        }

        // In a real world application we would use a command
        // property on the viewmodel and some sort of system
        // service that we iject into the viewmodel to exit the
        // application.
        private void OnExitAppClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OnExceptionWindowClosed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}