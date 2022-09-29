using System;

namespace CSharp_Result
{
    /// <summary>
    /// Empty Type to return for void functions to improve compatibility and code reuse for Results.
    /// </summary>
    public struct Unit
    {
        /// <summary>
        /// All Unit types are equal and have no value.
        /// </summary>
        /// <param name="obj">object to check equality</param>
        /// <returns>True if and only if the object is of type Unit</returns>
        public override bool Equals(object? obj) => obj is Unit;

        /// <summary>
        /// Unit has no string value, so it will return an empty string on ToString()
        /// </summary>
        /// <returns>Empty String</returns>
        public override string ToString() => string.Empty;

        /// <summary>
        /// Unit has no value and consequently has a hashcode of 0
        /// </summary>
        /// <returns>0</returns>
        public override int GetHashCode() => 0;

        /// <summary>
        /// All Unit are equal to each other, so this is always true.
        /// </summary>
        /// <param name="left">First Unit</param>
        /// <param name="right">Second Unit</param>
        /// <returns></returns>
        public static bool operator ==(Unit left, Unit right) => true;

        /// <summary>
        /// All Unit are equal to each other, so this is always false.
        /// </summary>
        /// <param name="left">First Unit</param>
        /// <param name="right">Second Unit</param>
        /// <returns></returns>
        public static bool operator !=(Unit left, Unit right) => false;
        
    }

    /// <summary>
    /// Extension method converting void methods with up to 15 arguments to methods returning Unit.
    /// </summary>
    public static class UnitExtensions
    {
        /// <summary>
        /// Extension method that converts a void method to one that returns Unit.
        /// </summary>
        /// <param name="func">Function to convert</param>
        /// <typeparam name="T1"></typeparam>
        /// <returns></returns>
        public static Func<T1, Unit> Unit<T1>(this Action<T1> func)
        {
            return t1 =>
            {
                func(t1);
                return new Unit();
            };
        }
        
        /// <summary>
        /// Extension method that converts a void method to one that returns Unit.
        /// </summary>
        /// <param name="func">Function to convert</param>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <returns></returns>
        public static Func<T1, T2, Unit> Unit<T1, T2>(Action<T1, T2> func)
        {
            return (t1, t2) =>
            {
                func(t1, t2);
                return new Unit();
            };
        }
        
        /// <summary>
        /// Extension method that converts a void method to one that returns Unit.
        /// </summary>
        /// <param name="func">Function to convert</param>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <returns></returns>
        public static Func<T1, T2, T3, Unit> Unit<T1, T2, T3>(Action<T1, T2, T3> func)
        {
            return (t1, t2, t3) =>
            {
                func(t1, t2, t3);
                return new Unit();
            };
        }
        
        /// <summary>
        /// Extension method that converts a void method to one that returns Unit.
        /// </summary>
        /// <param name="func">Function to convert</param>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <returns></returns>
        public static Func<T1, T2, T3, T4, Unit> Unit<T1, T2, T3, T4>(Action<T1, T2, T3, T4> func)
        {
            return (t1,t2,t3,t4) =>
            {
                func(t1,t2,t3,t4);
                return new Unit();
            };
        }

        /// <summary>
        /// Extension method that converts a void method to one that returns Unit.
        /// </summary>
        /// <param name="func">Function to convert</param>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <returns></returns>
        public static Func<T1, T2, T3, T4, T5, Unit> Unit<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> func)
        {
            return (t1, t2, t3, t4, t5) =>
            {
                func(t1, t2, t3, t4, t5);
                return new Unit();
            };
        }
        
        /// <summary>
        /// Extension method that converts a void method to one that returns Unit.
        /// </summary>
        /// <param name="func">Function to convert</param>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <returns></returns>
        public static Func<T1, T2, T3, T4, T5, T6, Unit> Unit<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> func)
        {
            return (t1, t2, t3, t4, t5, t6) =>
            {
                func(t1, t2, t3, t4, t5, t6);
                return new Unit();
            };
        }
        
        /// <summary>
        /// Extension method that converts a void method to one that returns Unit.
        /// </summary>
        /// <param name="func">Function to convert</param>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <returns></returns>
        public static Func<T1, T2, T3, T4, T5, T6, T7, Unit> Unit<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> func)
        {
            return (t1, t2, t3, t4, t5, t6, t7) =>
            {
                func(t1, t2, t3, t4, t5, t6, t7);
                return new Unit();
            };
        }
        
        /// <summary>
        /// Extension method that converts a void method to one that returns Unit.
        /// </summary>
        /// <param name="func">Function to convert</param>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <returns></returns>
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, Unit> Unit<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> func)
        {
            return (t1, t2, t3, t4, t5, t6, t7, t8) =>
            {
                func(t1, t2, t3, t4, t5, t6, t7, t8);
                return new Unit();
            };
        }
        
        /// <summary>
        /// Extension method that converts a void method to one that returns Unit.
        /// </summary>
        /// <param name="func">Function to convert</param>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <returns></returns>
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Unit> Unit<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> func)
        {
            return (t1, t2, t3, t4, t5, t6, t7, t8, t9) =>
            {
                func(t1, t2, t3, t4, t5, t6, t7, t8, t9);
                return new Unit();
            };
        }
        
        /// <summary>
        /// Extension method that converts a void method to one that returns Unit.
        /// </summary>
        /// <param name="func">Function to convert</param>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <returns></returns>
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Unit> Unit<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> func)
        {
            return (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10) =>
            {
                func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10);
                return new Unit();
            };
        }   
        
        /// <summary>
        /// Extension method that converts a void method to one that returns Unit.
        /// </summary>
        /// <param name="func">Function to convert</param>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <typeparam name="T11"></typeparam>
        /// <returns></returns>
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Unit> Unit<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> func)
        {
            return (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11) =>
            {
                func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11);
                return new Unit();
            };
        }
            
        /// <summary>
        /// Extension method that converts a void method to one that returns Unit.
        /// </summary>
        /// <param name="func">Function to convert</param>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <typeparam name="T11"></typeparam>
        /// <typeparam name="T12"></typeparam>
        /// <returns></returns>
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, Unit> Unit<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> func)
        {
            return (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12) =>
            {
                func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12);
                return new Unit();
            };
        }
        
        /// <summary>
        /// Extension method that converts a void method to one that returns Unit.
        /// </summary>
        /// <param name="func">Function to convert</param>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <typeparam name="T11"></typeparam>
        /// <typeparam name="T12"></typeparam>
        /// <typeparam name="T13"></typeparam>
        /// <returns></returns>
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, Unit> Unit<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> func)
        {
            return (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13) =>
            {
                func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13);
                return new Unit();
            };
        }
            
        /// <summary>
        /// Extension method that converts a void method to one that returns Unit.
        /// </summary>
        /// <param name="func">Function to convert</param>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <typeparam name="T11"></typeparam>
        /// <typeparam name="T12"></typeparam>
        /// <typeparam name="T13"></typeparam>
        /// <typeparam name="T14"></typeparam>
        /// <returns></returns>
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, Unit> Unit<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> func)
        {
            return (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14) =>
            {
                func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14);
                return new Unit();
            };
        }
        
        /// <summary>
        /// Extension method that converts a void method to one that returns Unit.
        /// </summary>
        /// <param name="func">Function to convert</param>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <typeparam name="T11"></typeparam>
        /// <typeparam name="T12"></typeparam>
        /// <typeparam name="T13"></typeparam>
        /// <typeparam name="T14"></typeparam>
        /// <typeparam name="T15"></typeparam>
        /// <returns></returns>
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, Unit> Unit<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> func)
        {
            return (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) =>
            {
                func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15);
                return new Unit();
            };
        }
    }
}