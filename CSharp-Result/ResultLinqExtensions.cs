using System;

namespace CSharp_Result
{
    public static class ResultLinqExtensions
    {
        
        /// <summary>
        /// LINQ Overload to use Select statement.
        /// Takes a Result and executes a function if the Result is Valid
        /// </summary>
        /// <param name="result">Input Result</param>
        /// <param name="function">Function to execute</param>
        /// <typeparam name="TSucc">Type of input</typeparam>
        /// <typeparam name="TReturn">Return type of function</typeparam>
        /// <returns>The Result of the computation</returns>
        public static Result<TReturn> Select<TSucc, TReturn>(this Result<TSucc> result,
            Func<TSucc, Result<TReturn>> function)
            where TSucc : notnull
            where TReturn : notnull
        {
            return result.Then(function);
        }
        
        /// <summary>
        /// LINQ Overload for Select statement.
        /// Takes a Result and executes a function if the Result is Valid
        /// </summary>
        /// <param name="result">Input Result</param>
        /// <param name="function">Function to execute</param>
        /// <typeparam name="TSucc">Type of input</typeparam>
        /// <typeparam name="TReturn">Return type of function</typeparam>
        /// <returns>The Result of the computation</returns>
        public static Result<TReturn> Select<TSucc, TReturn>(this Result<TSucc> result,
            Func<TSucc, TReturn> function)
            where TSucc : notnull
            where TReturn : notnull
        {
            return result.Then(function, Errors.MapNone);
        }
        
        /// <summary>
        /// LINQ Overload for SelectMany statement
        /// </summary>
        /// <param name="result">Input Result</param>
        /// <param name="function">Function to execute</param>
        /// <typeparam name="TSucc">Type of input</typeparam>
        /// <typeparam name="TReturn">Return type of function</typeparam>
        /// <returns>The Result of the computation</returns>
        public static Result<TReturn> SelectMany<TSucc, TReturn>(this Result<TSucc> result,
            Func<TSucc, Result<TReturn>> function)
            where TSucc : notnull
            where TReturn : notnull
        {
            return result.Then(function);
        }
        
        /// <summary>
        /// LINQ Overload for SelectMany statement
        /// </summary>
        /// <param name="result">Input Result</param>
        /// <param name="function">Function to execute</param>
        /// <param name="returner">Function to convert intermediate result into final result</param>
        /// <typeparam name="TSucc">Type of input</typeparam>
        /// <typeparam name="TMed">Intermediate type</typeparam>
        /// <typeparam name="TReturn">Return type of function</typeparam>
        /// <returns>The Result of the computation</returns>
        public static Result<TReturn> SelectMany<TSucc, TMed, TReturn>(this Result<TSucc> result,
            Func<TSucc, Result<TMed>> function, Func<TSucc, TMed, TReturn> returner)
            where TSucc : notnull
            where TReturn : notnull
            where TMed : notnull
        {
            return result.Then(x => function(x).Then(y => returner(x, y), Errors.MapNone));
        }
    }
}