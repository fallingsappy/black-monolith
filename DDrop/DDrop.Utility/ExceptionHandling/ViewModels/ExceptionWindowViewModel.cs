using System;

namespace DDrop.Utility.ExceptionHandling.ViewModels
{
    internal class ExceptionWindowVM
    {
        public ExceptionWindowVM(Exception exc)
        {
            Exception = exc;
            ExceptionType = exc.GetType().FullName;
        }

        public Exception Exception { get; }

        public string ExceptionType { get; }
    }
}