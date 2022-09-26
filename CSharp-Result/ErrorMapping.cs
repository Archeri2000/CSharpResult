using System;

namespace CSharp_Result
{
    public static class Errors
    {
        public delegate bool ExceptionFilter(Exception e);
        public static bool MapNone(Exception e)
        {
            return false;
        }

        public static bool MapAll(Exception e)
        {
            return true;
        }

        public static ExceptionFilter MapIfExceptionIs<T>()
        {
            return e => e is T;
        }

        public static ExceptionFilter Or<T>(this ExceptionFilter filter)
        {
            return e => filter(e) || e is T;
        }
    }

    /// <summary>
    /// Exception that is returned when an Assert function fails.
    /// </summary>
    public class AssertionException : Exception
    {
        /// <summary>
        /// Creates a new AssertionException
        /// </summary>
        public AssertionException():base(){}

        /// <summary>
        /// Creates a new AssertionException that captures the exception message
        /// </summary>
        /// <param name="message">Exception message to capture</param>
        public AssertionException(string? message) : base(message) {}

        /// <summary>
        /// Creates a new AssertionException that captures the exception message and an inner Exception
        /// </summary>
        /// <param name="message">Exception message to capture</param>
        /// <param name="inner">Inner Exception</param>
        public AssertionException(string? message, Exception? inner) : base(message, inner){}

    }
    
}