using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSharp_Result
{
    /// <summary>
    /// LINQ extension methods to use with Result
    /// </summary>
    public static class AsyncResultLinqExtensions
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
        public static async Task<Result<TReturn>> Select<TSucc, TReturn>(this Task<Result<TSucc>> result, 
            Func<TSucc, Result<TReturn>> function)
            where TSucc : notnull
            where TReturn : notnull
        {
            return await result.Then(function);
        }

        /// <summary>
        /// LINQ Overload to use Select statement.
        /// Takes a Result and executes an async function if the Result is Valid
        /// </summary>
        /// <param name="result">Input Result</param>
        /// <param name="function">Function to execute</param>
        /// <typeparam name="TSucc">Type of input</typeparam>
        /// <typeparam name="TReturn">Return type of function</typeparam>
        /// <returns>The Result of the computation</returns>
        public static async Task<Result<TReturn>> Select<TSucc, TReturn>(this Task<Result<TSucc>> result,
            Func<TSucc, Task<Result<TReturn>>> function)
            where TSucc : notnull
            where TReturn : notnull
        {
            return await result.ThenAwait(function);
        }

        /// <summary>
        /// LINQ Overload to use Select statement.
        /// Takes a Result and executes an async function if the Result is Valid
        /// </summary>
        /// <param name="result">Input Result</param>
        /// <param name="function">Function to execute</param>
        /// <typeparam name="TSucc">Type of input</typeparam>
        /// <typeparam name="TReturn">Return type of function</typeparam>
        /// <returns>The Result of the computation</returns>
        public static async Task<Result<TReturn>> Select<TSucc, TReturn>(this Task<Result<TSucc>> result,
            Func<TSucc, Task<TReturn>> function)
            where TSucc : notnull
            where TReturn : notnull
        {
            return await result.ThenAwait(function, Errors.MapNone);
        }

        /// <summary>
        /// LINQ Overload to use Select statement.
        /// Takes a Result and executes a function if the Result is Valid
        /// </summary>
        /// <param name="result">Input Result</param>
        /// <param name="function">Function to execute</param>
        /// <typeparam name="TSucc">Type of input</typeparam>
        /// <typeparam name="TReturn">Return type of function</typeparam>
        /// <returns>The Result of the computation</returns>
        public static async Task<Result<TReturn>> Select<TSucc, TReturn>(this Task<Result<TSucc>> result,
            Func<TSucc, TReturn> function)
            where TSucc : notnull
            where TReturn : notnull
        {
            return await result.Then(function, Errors.MapNone);
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
        public static async Task<Result<TReturn>> SelectMany<TSucc, TMed, TReturn>(this Task<Result<TSucc>> result,
            Func<TSucc, Task<Result<TMed>>> function, Func<TSucc, TMed, Task<TReturn>> returner)
            where TSucc : notnull
            where TReturn : notnull
            where TMed : notnull
        {
            return await result.ThenAwait(x => function(x).ThenAwait(y => returner(x, y), Errors.MapNone));
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
        public static async Task<Result<TReturn>> SelectMany<TSucc, TMed, TReturn>(this Task<Result<TSucc>> result,
            Func<TSucc, Task<Result<TMed>>> function, Func<TSucc, TMed, TReturn> returner)
            where TSucc : notnull
            where TReturn : notnull
            where TMed : notnull
        {
            return await result.ThenAwait(x => function(x).Then(y => returner(x, y), Errors.MapNone));
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
        public static async Task<Result<TReturn>> SelectMany<TSucc, TMed, TReturn>(this Task<Result<TSucc>> result,
            Func<TSucc, Result<TMed>> function, Func<TSucc, TMed, Task<TReturn>> returner)
            where TSucc : notnull
            where TReturn : notnull
            where TMed : notnull
        {
            return await result.ThenAwait(x => function(x).ToAsyncResult().ThenAwait(y => returner(x, y), Errors.MapNone));
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
        public static async Task<Result<TReturn>> SelectMany<TSucc, TMed, TReturn>(this Task<Result<TSucc>> result,
            Func<TSucc, Result<TMed>> function, Func<TSucc, TMed, TReturn> returner)
            where TSucc : notnull
            where TReturn : notnull
            where TMed : notnull
        {
            return await result.Then(x => function(x).Then(y => returner(x, y), Errors.MapNone));
        }

    }  
}