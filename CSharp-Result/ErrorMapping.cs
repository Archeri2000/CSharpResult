using System;

namespace CSharp_Result
{
    public static class Errors
    {
        public static Exception MapNone(Exception e)
        {
            throw e;
        }

        public static Exception MapAll(Exception e)
        {
            return e;
        }
    }
}