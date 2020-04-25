using System;
using System.Windows;
using DDrop.Utility.ExceptionHandling.ViewModels;
using DDrop.Utility.ExceptionHandling.Windows;

namespace DDrop.Utility.ExceptionHandling.ExceptionHandling
{
    /// <summary>
    ///     This ExceptionHandler implementation opens a new
    ///     error window for every unhandled exception that occurs.
    /// </summary>
    public class WindowExceptionHandler : GlobalExceptionHandlerBase, IGlobalExceptionHandler
    {
        /// <summary>
        ///     This method opens a new ExceptionWindow with the
        ///     passed exception object as datacontext.
        /// </summary>
        public override void OnUnhandledException(Exception e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                var exceptionWindow = new ExceptionWindow();
                exceptionWindow.DataContext = new ExceptionWindowVM(e);
                exceptionWindow.ShowDialog();
            }));
        }
    }
}