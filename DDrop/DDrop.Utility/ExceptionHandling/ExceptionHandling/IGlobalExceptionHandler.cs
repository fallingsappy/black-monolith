using System;

namespace DDrop.Utility.ExceptionHandling.ExceptionHandling
{
    public interface IGlobalExceptionHandler
    {
        /// <summary>
        ///     This methods gets invoked for every unhandled excption
        ///     that is raise on the application Dispatcher, the AppDomain
        ///     or by the GC cleaning up a faulted Task.
        /// </summary>
        /// <param name="e">The unhandled exception</param>
        void OnUnhandledException(Exception e);
    }
}