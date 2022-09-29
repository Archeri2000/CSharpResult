using System;

namespace CSharp_Result
{
    /// <summary>
    /// Static class to hold methods related to mapping thrown Exceptions
    /// </summary>
    public static class Errors
    {
        /// <summary>
        /// Predicate taking a thrown Exception and returning true if it should be mapped to a Result.
        /// </summary>
        public delegate bool ExceptionFilter(Exception e);
        
        /// <summary>
        /// Using this ExceptionFilter, any thrown Exceptions will be rethrown
        /// </summary>
        /// <param name="e">Thrown Exception</param>
        /// <returns>False always</returns>
        public static bool MapNone(Exception e)
        {
            return false;
        }

        /// <summary>
        /// Using this ExceptionFilter, any thrown Exceptions will be converted to a Result
        /// </summary>
        /// <param name="e">Thrown Exception</param>
        /// <returns>True always</returns>
        public static bool MapAll(Exception e)
        {
            return true;
        }

        /// <summary>
        /// Using this ExceptionFilter, thrown Exceptions are rethrown unless they match the type T.
        /// </summary>
        /// <typeparam name="T">Exception type to match</typeparam>
        /// <returns>ExceptionFilter which returns true when Exception is T</returns>
        public static ExceptionFilter MapIfExceptionIs<T>()
        {
            return e => e is T;
        }

        /// <summary>
        /// Extension for MapIfExceptionIs to allow it to match more than one type.
        /// </summary>
        /// <param name="filter">Existing ExceptionFilter</param>
        /// <typeparam name="T">Exception type to match</typeparam>
        /// <returns>ExceptionFilter which returns true when it matches any of the types</returns>
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
        public AssertionException()
        {}

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