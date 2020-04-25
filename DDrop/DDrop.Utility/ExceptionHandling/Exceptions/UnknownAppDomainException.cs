using System;

namespace DDrop.Utility.ExceptionHandling.Exceptions
{
    public class UnknownAppDomainException : Exception
    {
        public UnknownAppDomainException(string msg) : base(msg)
        {
        }
    }
}