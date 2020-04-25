using System;

namespace DDrop.Utility.ExceptionHandling.Exceptions
{
    internal class SampleException : Exception
    {
        public SampleException(string message) : base(message)
        {
        }
    }
}