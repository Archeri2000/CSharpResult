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

    public class AssertionException : Exception
    {
        public AssertionException():base(){}

        public AssertionException(string message) : base(message) {}

        public AssertionException(string message, Exception inner) : base(message, inner){}
    }
}