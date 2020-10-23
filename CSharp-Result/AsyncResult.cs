using System;
using System.Threading.Tasks;

namespace CSharp_Result
{
   public static class AsyncResult 
   {
      /// <summary>
      /// Converts an object into an Async Valid Result
      /// </summary>
      /// <param name="obj">Object to convert</param>
      /// <typeparam name="TSucc">Type of the object</typeparam>
      /// <returns>An Async Valid Result containing the object</returns>
      public static async Task<Result<TSucc>> ToAsyncResult<TSucc>(this TSucc obj) 
         where TSucc : notnull
      {
         return obj;
      }
      /// <summary>
      /// Converts a Result into an Async Result
      /// </summary>
      /// <param name="obj">Result to convert</param>
      /// <typeparam name="TSucc">Type of the Valid Result</typeparam>
      /// <returns>An Async Result containing the object</returns>
      public static async Task<Result<TSucc>> ToAsyncResult<TSucc>(this Result<TSucc> obj) 
         where TSucc : notnull
      {
         return obj;
      }
      
      /// <summary>
      /// Converts a normal single argument function into a new function that outputs a Result.
      /// </summary>
      /// <param name="func">Function to execute</param>
      /// <param name="mapError">Exceptions to catch and map to Errors</param>
      /// <typeparam name="TInput">Input type</typeparam>
      /// <typeparam name="TSucc">Output type</typeparam>
      /// <returns>Async Function that outputs a Valid Result if successful, converting relevant Exceptions into Errors</returns>
      public static Func<TInput, Task<Result<TSucc>>> ToAsyncResultFunc<TInput, TSucc>(this Func<TInput, Task<TSucc>> func, Func<Exception, Exception> mapError) 
         where TSucc : notnull
      {
         return (async x =>
         {
            try
            {
               return await func(x);
            }
            catch(Exception e)
            {
               return mapError(e);
            }
         });
      }
      
      /// <summary>
      /// Converts a normal single argument void function into a new function that outputs a Result.
      /// </summary>
      /// <param name="func">Function to execute</param>
      /// <param name="mapError">Exceptions to catch and map to Errors</param>
      /// <typeparam name="TInput">Input type</typeparam>
      /// <returns>Async Function that outputs a Valid Result of Unit if successful, converting relevant Exceptions into Errors</returns>
      public static Func<TInput, Task<Result<Unit>>> ToAsyncResultFunc<TInput>(this Func<TInput, Task> func, Func<Exception, Exception> mapError) 
      {
         return (async x =>
         {
            try
            {
               await func(x);
               return new Unit();
            }
            catch(Exception e)
            {
               return mapError(e);
            }
         });
      }

      /// <summary>
      /// Converts a normal single argument function into a new function that outputs a Result.
      /// </summary>
      /// <param name="func">Function to execute</param>
      /// <param name="mapError">Exceptions to catch and map to Errors</param>
      /// <typeparam name="TInput">Input type</typeparam>
      /// <typeparam name="TSucc">Output type</typeparam>
      /// <returns>Async Function that outputs a Valid Result if successful, converting relevant Exceptions into Errors</returns>
      public static Func<TInput, Task<Result<TSucc>>> ToAsyncResultFunc<TInput, TSucc>(this Func<TInput, TSucc> func, Func<Exception, Exception> mapError) 
         where TSucc : notnull
      {
         return (async x =>
         {
            try
            {
               return func(x);
            }
            catch(Exception e)
            {
               return mapError(e);
            }
         });
      }
   }
   public static class AsyncResultExtensions
   {
      /// <summary>
      /// If Async Result is an Error, passes the Error through a mapper function.
      /// </summary>
      /// <param name="res">Input Async Result</param>
      /// <param name="mapper">Maps Errors to other Errors</param>
      /// <returns>Mapped Error if Result is Error</returns>
      public static async Task<Result<TSucc>> MapError<TSucc>(this Task<Result<TSucc>> res, Func<Exception, Exception> mapper) 
         where TSucc : notnull
      {
         var result = await res;
         return result.MapError(mapper);
      }

      /// <summary>
      /// Define continuations on the success and failure cases.
      /// If Result is Valid Result, executes Success
      /// Else, executes Failure
      /// </summary>
      /// <param name="res">Input Result</param>
      /// <param name="Success">Function to execute on Success</param>
      /// <param name="Failure">Function to execute on Failure</param>
      /// <typeparam name="TInput">Type of the input Result</typeparam>
      /// <typeparam name="T">Return type of the function</typeparam>
      /// <returns>Object of type T</returns>
      public async static Task<T> Match<TInput, T>(this Task<Result<TInput>> res, Func<TInput, Task<T>> Success, Func<Exception, Task<T>> Failure)
         where TInput : notnull
      {
         var result = await res switch
         {
            Success<TInput> s => Success(s),
            Failure<TInput> e => Failure(e),
            _ => throw new NotSupportedException("Unable to match Result with non Success or Failure!")
         };
         return await result;
      }

      /// <summary>
      /// If holding a Valid Result, Executes the async function with the result as input.
      /// </summary>
      /// <param name="res">Input Async Result</param>
      /// <param name="function">The function to execute</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <typeparam name="TResult">The type of the result of the computation (unused)</typeparam>
      /// <returns>Async of either the Valid Result, or an Error</returns>
      public static Task<Result<TSucc>> DoAwait<TSucc, TResult>(this Task<Result<TSucc>> res, Func<TSucc, Task<Result<TResult>>> function) 
         where TSucc : notnull
         where TResult : notnull
      {
         return res.Match(
            Success: s =>
            {
               return function(s).Then(x => s, Errors.MapNone);
            },
            Failure: e => Task.FromResult((Result<TSucc>)e)
         );
      }

      /// <summary>
      /// If holding a Valid Result, Executes the async function with the result as input.
      /// If any exception is thrown, it is mapped by the mapper function.
      /// </summary>
      /// <param name="res">Input Async Result</param>
      /// <param name="function">The function to execute</param>
      /// <param name="mapError">The mapping function for the error</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <typeparam name="TResult">The type of the result of the computation (unused)</typeparam>
      /// <returns>Async of either the Valid Result, or an Error</returns>
      public static Task<Result<TSucc>> DoAwait<TSucc, TResult>(this Task<Result<TSucc>> res, Func<TSucc, Task<TResult>> function, Func<Exception, Exception> mapError) 
         where TSucc : notnull
         where TResult : notnull
      {
         return res.DoAwait(function.ToAsyncResultFunc(mapError));
      }
      
      /// <summary>
      /// If holding a Valid Result, Executes the async function with the result as input.
      /// If any exception is thrown, it is mapped by the mapper function.
      /// </summary>
      /// <param name="res">Input Async Result</param>
      /// <param name="function">The function to execute</param>
      /// <param name="mapError">The mapping function for the error</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <returns>Async of either a Valid Result of Unit, or an Error</returns>
      public static Task<Result<TSucc>> DoAwait<TSucc>(this Task<Result<TSucc>> res, Func<TSucc, Task> function, Func<Exception, Exception> mapError) 
         where TSucc : notnull
      {
         return res.DoAwait(function.ToAsyncResultFunc(mapError));
      }
      
      /// <summary>
      /// If holding a Valid Result, Executes the function with the result as input.
      /// </summary>
      /// <param name="res">Input Async Result</param>
      /// <param name="function">The function to execute</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <typeparam name="TResult">The type of the result of the computation (unused)</typeparam>
      /// <returns>Async of either the Valid Result, or an Error</returns>
      public static async Task<Result<TSucc>> Do<TSucc, TResult>(this Task<Result<TSucc>> res, Func<TSucc, Result<TResult>> function) 
         where TSucc : notnull
      {
         var result = await res;
         return result.Do(function);
      }

      /// <summary>
      /// If holding a Valid Result, Executes the function with the result as input.
      /// If any exception is thrown, it is mapped by the mapper function.
      /// </summary>
      /// <param name="res">Input Async Result</param>
      /// <param name="function">The function to execute</param>
      /// <param name="mapError">The mapping function for the error</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <typeparam name="TResult">The type of the result of the computation (unused)</typeparam>
      /// <returns>Async of either the Valid Result, or an Error</returns>
      public static async Task<Result<TSucc>> Do<TSucc, TResult>(this Task<Result<TSucc>> res, Func<TSucc, TResult> function, Func<Exception, Exception> mapError) 
         where TSucc : notnull
      {
         var result = await res;
         return result.Do(function, mapError);
      }
      
      /// <summary>
      /// If holding a Valid Result, Executes the function with the result as input.
      /// If any exception is thrown, it is mapped by the mapper function.
      /// </summary>
      /// <param name="res">Input Async Result</param>
      /// <param name="function">The function to execute</param>
      /// <param name="mapError">The mapping function for the error</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <returns>Async of either a Valid Result of Unit, or an Error</returns>
      public static async Task<Result<TSucc>> Do<TSucc>(this Task<Result<TSucc>> res, Action<TSucc> function, Func<Exception, Exception> mapError) 
         where TSucc : notnull
      {
         var result = await res;
         return result.Do(function, mapError);
      }

      /// <summary>
      /// If holding a Valid Result, Executes the async function with the result as input.
      /// Returns the result of the computation.
      /// </summary>
      /// <param name="res">Input Async Result</param>
      /// <param name="function">The function to execute</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <typeparam name="TResult">The type of the result of the computation (unused)</typeparam>
      /// <returns>Async of either a Valid Result from the computation, or an Error</returns>
      public static Task<Result<TResult>> ThenAwait<TSucc, TResult>(this Task<Result<TSucc>> res, Func<TSucc, Task<Result<TResult>>> function) 
         where TSucc : notnull
         where TResult : notnull
      {
         return res.Match(
            Success: function,
            Failure: e => Task.FromResult((Result<TResult>)e)
            );
      }
      
      /// <summary>
      /// If holding a Valid Result, Executes the async function with the result as input.
      /// If any exception is thrown, it is mapped by the mapper function, otherwise returns the result of the computation.
      /// </summary>
      /// <param name="res">Input Async Result</param>
      /// <param name="function">The function to execute</param>
      /// <param name="mapError">The mapping function for the error</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <typeparam name="TResult">The type of the result of the computation (unused)</typeparam>
      /// <returns>Async of either a Valid Result from the computation, or an Error</returns>
      public static Task<Result<TResult>> ThenAwait<TSucc, TResult>(this Task<Result<TSucc>> res, Func<TSucc, Task<TResult>> function, Func<Exception, Exception> mapError) 
         where TSucc : notnull
         where TResult : notnull
      {
         return res.ThenAwait(function.ToAsyncResultFunc(mapError));
      }
      
      /// <summary>
      /// If holding a Valid Result, Executes the async function with the result as input.
      /// If any exception is thrown, it is mapped by the mapper function, otherwise returns Unit.
      /// </summary>
      /// <param name="res">Input Async Result</param>
      /// <param name="function">The function to execute</param>
      /// <param name="mapError">The mapping function for the error</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <returns>Async of either a Valid Result containing Unit, or an Error</returns>
      public static Task<Result<Unit>> ThenAwait<TSucc>(this Task<Result<TSucc>> res, Func<TSucc, Task> function, Func<Exception, Exception> mapError) 
         where TSucc : notnull
      {
         return res.ThenAwait(function.ToAsyncResultFunc(mapError));
      }
      
      /// <summary>
      /// If holding a Valid Result, Executes the function with the result as input.
      /// Returns the result of the computation.
      /// </summary>
      /// <param name="res">Input Async Result</param>
      /// <param name="function">The function to execute</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <typeparam name="TResult">The type of the result of the computation (unused)</typeparam>
      /// <returns>Async of either a Valid Result from the computation, or an Error</returns>
      public static async Task<Result<TResult>> Then<TSucc, TResult>(this Task<Result<TSucc>> res, Func<TSucc, Result<TResult>> function) 
         where TSucc : notnull
         where TResult : notnull
      {
         var result = await res;
         return result.Then(function);
      }
      
      /// <summary>
      /// If holding a Valid Result, Executes the async function with the result as input.
      /// If any exception is thrown, it is mapped by the mapper function, otherwise returns the result of the computation.
      /// </summary>
      /// <param name="res">Input Async Result</param>
      /// <param name="function">The function to execute</param>
      /// <param name="mapError">The mapping function for the error</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <typeparam name="TResult">The type of the result of the computation (unused)</typeparam>
      /// <returns>Async of either a Valid Result from the computation, or an Error</returns>
      public static async Task<Result<TResult>> Then<TSucc, TResult>(this Task<Result<TSucc>> res, Func<TSucc, TResult> function, Func<Exception, Exception> mapError) 
         where TSucc : notnull
         where TResult : notnull
      {
         var result = await res;
         return result.Then(function, mapError);
      }
      
      /// <summary>
      /// If holding a Valid Result, Executes the async function with the result as input.
      /// If any exception is thrown, it is mapped by the mapper function, otherwise returns Unit.
      /// </summary>
      /// <param name="res">Input Async Result</param>
      /// <param name="function">The function to execute</param>
      /// <param name="mapError">The mapping function for the error</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <returns>Async of either a Valid Result containing Unit, or an Error</returns>
      public static async Task<Result<Unit>> Then<TSucc>(this Task<Result<TSucc>> res, Action<TSucc> function, Func<Exception, Exception> mapError) 
         where TSucc : notnull
      {
         var result = await res;
         return result.Then(function, mapError);
      }
   }
}