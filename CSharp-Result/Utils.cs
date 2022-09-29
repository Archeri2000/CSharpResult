using System;

namespace CSharp_Result
{
    /// <summary>
    /// Miscellaneous common utility methods that can be used in Result piping.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Function that returns its input as its output. This can be useful in If expressions for one of the branches.
        /// </summary>
        /// <param name="t">Input object</param>
        /// <typeparam name="T">Type of the input object</typeparam>
        /// <returns>Input object</returns>
        public static T Identity<T>(this T t)
        {
            return t;
        }

        /// <summary>
        /// Predicate Constructor that checks if object is a certain type.
        /// </summary>
        /// <typeparam name="T">Type to match</typeparam>
        /// <returns>Predicate that returns true if object matches T</returns>
        public static Func<object, bool> Is<T>()
        {
            return o => o is T;
        }

        /// <summary>
        /// Extension to Is predicate constructor, that also checks if object is a certain type.
        /// </summary>
        /// <param name="pred">Existing Predicate</param>
        /// <typeparam name="T">Type to match</typeparam>
        /// <returns>Predicate that returns true if object matches any of the types</returns>
        public static Func<object, bool> Or<T>(this Func<object, bool> pred)
        {
            return o => pred(o) || o is T;
        }

        /// <summary>
        /// Tries to cast input object to type T, returning an InvalidCastException result if it fails.  
        /// </summary>
        /// <param name="o">Input object</param>
        /// <typeparam name="T">Type to cast to</typeparam>
        /// <returns>Result holding either input object cast to T or InvalidCastException.</returns>
        public static Result<T> As<T>(this object o)
        {
            if (o is T t) return t;
            return new InvalidCastException($"Unable to cast {o} as type {typeof(T)}!");
        }
        
        
    }
}