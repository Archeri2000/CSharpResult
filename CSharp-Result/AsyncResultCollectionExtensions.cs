using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CSharp_Result
{
    public static class AsyncResultCollectionExtensions()
    {
       /// <summary>
       /// 
       /// </summary>
       /// <param name="results"></param>
       /// <typeparam name="TSucc"></typeparam>
       /// <returns></returns>
        public static async Task<IEnumerable<Result<TSucc>>> AwaitAll<TSucc>(this IEnumerable<Task<Result<TSucc>>> results)
        {
           var awaited = await Task.WhenAll(results.ToList());
           return awaited.AsEnumerable();
        }
        
        public static async Task<Result<TSucc>> AwaitAny<TSucc>(this IEnumerable<Task<Result<TSucc>>> results)
        {
           return (await Task.WhenAny(results)).Result;
        }

        public static async IAsyncEnumerable<Result<TSucc>> ToIAsyncEnumerable<TSucc>(
           this IEnumerable<Task<Result<TSucc>>> results)
        {
           foreach (var result in results)
           {
              yield return await result;
           }
        }
        
      /// <summary>
      /// Executes Async Do on each element of the collection
      /// </summary>
      /// <param name="results">Input Async Result Collection</param>
      /// <param name="function">The function to execute</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <typeparam name="TResult">The type of the result of the computation (unused)</typeparam>
      /// <returns>Collection after executing function on each element</returns>
      public static IEnumerable<Task<Result<TSucc>>> DoAwaitEach<TSucc, TResult>(this IEnumerable<Task<Result<TSucc>>> results, Func<TSucc, Task<Result<TResult>>> function) 
         where TSucc : notnull
         where TResult : notnull
        {
           return results.Select(x => x.DoAwait(function));
        }

      /// <summary>
      /// Executes Async Do on each element of the collection
      /// </summary>
      /// <param name="results">Input Async Result Collection</param>
      /// <param name="function">The function to execute</param>
      /// <param name="mapException">The mapping function for the error</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <typeparam name="TResult">The type of the result of the computation (unused)</typeparam>
      /// <returns>Collection after executing function on each element</returns>
      public static IEnumerable<Task<Result<TSucc>>> DoAwaitEach<TSucc, TResult>(this IEnumerable<Task<Result<TSucc>>> results, Func<TSucc, Task<TResult>> function, Func<Exception, Exception> mapException) 
         where TSucc : notnull
         where TResult : notnull
      {
         return results.DoAwaitEach(function.ToAsyncResultFunc(mapException));
      }
      
      /// <summary>
      /// Executes Async Do on each element of the collection
      /// </summary>
      /// <param name="results">Input Async Result Collection</param>
      /// <param name="function">The function to execute</param>
      /// <param name="mapException">The mapping function for the error</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <returns>Collection after executing function on each element</returns>
      public static IEnumerable<Task<Result<TSucc>>> DoAwaitEach<TSucc>(this IEnumerable<Task<Result<TSucc>>> results, Func<TSucc, Task> function, Func<Exception, Exception> mapException) 
         where TSucc : notnull
      {
         return results.DoAwaitEach(function.ToAsyncResultFunc(mapException));
      }
      
      /// <summary>
      /// Executes Do on each element of the collection
      /// </summary>
      /// <param name="results">Input Async Result Collection</param>
      /// <param name="function">The function to execute</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <typeparam name="TResult">The type of the result of the computation (unused)</typeparam>
      /// <returns>Collection after executing function on each element</returns>
      public static IEnumerable<Task<Result<TSucc>>> DoEach<TSucc, TResult>(this IEnumerable<Task<Result<TSucc>>> results, Func<TSucc, Result<TResult>> function) 
         where TSucc : notnull
      {
         return results.Select(x => x.Do(function));
      }

      /// <summary>
      /// Executes Do on each element of the collection
      /// </summary>
      /// <param name="results">Input Async Result Collection</param>
      /// <param name="function">The function to execute</param>
      /// <param name="mapException">The mapping function for the error</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <typeparam name="TResult">The type of the result of the computation (unused)</typeparam>
      /// <returns>Collection after executing function on each element</returns>
      public static IEnumerable<Task<Result<TSucc>>> DoEach<TSucc, TResult>(this IEnumerable<Task<Result<TSucc>>> results, Func<TSucc, TResult> function, Func<Exception, Exception> mapException) 
         where TSucc : notnull
      {
         return results.DoEach(function.ToResultFunc(mapException));
      }
      
      /// <summary>
      /// Executes Do on each element of the collection
      /// </summary>
      /// <param name="results">Input Async Result Collection</param>
      /// <param name="function">The function to execute</param>
      /// <param name="mapException">The mapping function for the error</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <returns>Collection after executing function on each element</returns>
      public static IEnumerable<Task<Result<TSucc>>> DoEach<TSucc>(this IEnumerable<Task<Result<TSucc>>> results, Action<TSucc> function, Func<Exception, Exception> mapException) 
         where TSucc : notnull
      {
         return results.DoEach(function.ToResultFunc(mapException));
      }

      /// <summary>
      /// Executes Async Then on each element of the collection
      /// </summary>
      /// <param name="results">Input Async Result Collection</param>
      /// <param name="function">The function to execute</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <typeparam name="TResult">The type of the result of the computation (unused)</typeparam>
      /// <returns>Collection after executing function on each element</returns>
      public static IEnumerable<Task<Result<TResult>>> ThenAwaitEach<TSucc, TResult>(
         this IEnumerable<Task<Result<TSucc>>> results, Func<TSucc, Task<Result<TResult>>> function)
      {
         return results.Select(x => x.ThenAwait(function));
      }
      
      /// <summary>
      /// Executes Async Then on each element of the collection
      /// </summary>
      /// <param name="results">Input Async Result Collection</param>
      /// <param name="function">The function to execute</param>
      /// <param name="mapException">The mapping function for the error</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <typeparam name="TResult">The type of the result of the computation (unused)</typeparam>
      /// <returns>Collection after executing function on each element</returns>
      public static IEnumerable<Task<Result<TResult>>> ThenAwaitEach<TSucc, TResult>(this IEnumerable<Task<Result<TSucc>>> results, Func<TSucc, Task<TResult>> function, Func<Exception, Exception> mapException) 
         where TSucc : notnull
         where TResult : notnull
      {
         return results.ThenAwaitEach(function.ToAsyncResultFunc(mapException));
      }
      
      /// <summary>
      /// Executes Async Then on each element of the collection
      /// </summary>
      /// <param name="results">Input Async Result Collection</param>
      /// <param name="function">The function to execute</param>
      /// <param name="mapException">The mapping function for the error</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <returns>Collection after executing function on each element</returns>
      public static IEnumerable<Task<Result<Unit>>> ThenAwaitEach<TSucc>(this IEnumerable<Task<Result<TSucc>>> results, Func<TSucc, Task> function, Func<Exception, Exception> mapException) 
         where TSucc : notnull
      {
         return results.ThenAwaitEach(function.ToAsyncResultFunc(mapException));
      }
      
      /// <summary>
      /// Executes Then on each element of the collection
      /// </summary>
      /// <param name="results">Input Async Result Collection</param>
      /// <param name="function">The function to execute</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <typeparam name="TResult">The type of the result of the computation (unused)</typeparam>
      /// <returns>Collection after executing function on each element</returns>
      public static IEnumerable<Task<Result<TResult>>> ThenEach<TSucc, TResult>(this IEnumerable<Task<Result<TSucc>>> results, Func<TSucc, Result<TResult>> function) 
         where TSucc : notnull
         where TResult : notnull
      {
         return results.Select(x => x.Then(function));
      }
      
      /// <summary>
      /// Executes Then on each element of the collection
      /// </summary>
      /// <param name="results">Input Async Result Collection</param>
      /// <param name="function">The function to execute</param>
      /// <param name="mapException">The mapping function for the error</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <typeparam name="TResult">The type of the result of the computation (unused)</typeparam>
      /// <returns>Collection after executing function on each element</returns>
      public static IEnumerable<Task<Result<TResult>>> ThenEach<TSucc, TResult>(this IEnumerable<Task<Result<TSucc>>> results, Func<TSucc, TResult> function, Func<Exception, Exception> mapException) 
         where TSucc : notnull
         where TResult : notnull
      {
         return results.ThenEach(function.ToResultFunc(mapException));
      }
      
      /// <summary>
      /// Executes Then on each element of the collection
      /// </summary>
      /// <param name="results">Input Async Result Collection</param>
      /// <param name="function">The function to execute</param>
      /// <param name="mapException">The mapping function for the error</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <returns>Collection after executing function on each element</returns>
      public static IEnumerable<Task<Result<Unit>>> ThenEach<TSucc>(this IEnumerable<Task<Result<TSucc>>> results, Action<TSucc> function, Func<Exception, Exception> mapException) 
         where TSucc : notnull
      {
         return results.ThenEach(function.ToResultFunc(mapException));

      }
      
      /// <summary>
      /// Executes Async If on each element of the collection
      /// </summary>
      /// <param name="results">Input Async Result Collection</param>
      /// <param name="function">The function to execute</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <returns>Collection after executing function on each element</returns>
      public static IEnumerable<Task<Result<TSucc>>> IfAwaitEach<TSucc>(this IEnumerable<Task<Result<TSucc>>> results, Func<TSucc, Task<Result<bool>>> function) 
         where TSucc : notnull
      {
         return results.Select(x => x.IfAwait(function));
      }

      /// <summary>
      /// Executes Async If on each element of the collection
      /// </summary>
      /// <param name="results">Input Async Result Collection</param>
      /// <param name="function">The function to execute</param>
      /// <param name="mapException">The mapping function for the error</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <returns>Collection after executing function on each element</returns>
      public static IEnumerable<Task<Result<TSucc>>> IfAwaitEach<TSucc>(this IEnumerable<Task<Result<TSucc>>> results, Func<TSucc, Task<bool>> function, Func<Exception, Exception> mapException) 
         where TSucc : notnull
      {
         return results.IfAwaitEach(function.ToAsyncResultFunc(mapException));
      }

      /// <summary>
      /// Executes If on each element of the collection
      /// </summary>
      /// <param name="results">Input Async Result Collection</param>
      /// <param name="function">The function to execute</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <returns>Collection after executing function on each element</returns>
      public static IEnumerable<Task<Result<TSucc>>> If<TSucc>(this IEnumerable<Task<Result<TSucc>>> results, Func<TSucc, Result<bool>> function) 
         where TSucc : notnull
      {
         return results.Select(x => x.If(function));
      }

      /// <summary>
      /// Executes If on each element of the collection
      /// </summary>
      /// <param name="results">Input Async Result Collection</param>
      /// <param name="function">The function to execute</param>
      /// <param name="mapException">The mapping function for the error</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <returns>Collection after executing function on each element</returns>
      public static IEnumerable<Task<Result<TSucc>>> If<TSucc>(this IEnumerable<Task<Result<TSucc>>> results, Func<TSucc, bool> function, Func<Exception, Exception> mapException) 
         where TSucc : notnull
      {
         return results.If(function.ToResultFunc(mapException));
      }

      /// <summary>
      /// Executes Async Match on each element of the collection
      /// </summary>
      /// <param name="results">Input Async Result Collection</param>
      /// <param name="Success">The function to execute if Result is Success</param>
      /// <param name="Failure">The function to execute if Result is Error</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <typeparam name="TResult">Return type</typeparam>
      /// <returns>Collection after executing function on each element</returns>
      public static IEnumerable<Task<TResult>> MatchAwaitEach<TSucc, TResult>(this IEnumerable<Task<Result<TSucc>>> results,
         Func<TSucc, Task<TResult>> Success, Func<Exception, Task<TResult>> Failure)
      {
         return results.Select(x => x.MatchAwait(Success, Failure));
      }
        
      /// <summary>
      /// Executes Async Match on each element of the collection
      /// </summary>
      /// <param name="results">Input Async Result Collection</param>
      /// <param name="Success">The function to execute if Result is Success</param>
      /// <param name="Failure">The function to execute if Result is Error</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <returns>Collection after executing function on each element</returns>
      public static IEnumerable<Task> MatchAwaitEach<TSucc>(this IEnumerable<Task<Result<TSucc>>> results,
         Func<TSucc, Task> Success, Func<Exception, Task> Failure)
      {
         return results.Select(x => x.MatchAwait(Success, Failure));
      }
      
      /// <summary>
      /// Executes Match on each element of the collection
      /// </summary>
      /// <param name="results">Input Async Result Collection</param>
      /// <param name="Success">The function to execute if Result is Success</param>
      /// <param name="Failure">The function to execute if Result is Error</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <returns>Collection after executing function on each element</returns>
      public static IEnumerable<Task<TResult>> MatchEach<TSucc, TResult>(this IEnumerable<Task<Result<TSucc>>> results,
         Func<TSucc, TResult> Success, Func<Exception, TResult> Failure)
      {
         return results.Select(x => x.Match(Success, Failure));
      }
        
      /// <summary>
      /// Executes Match on each element of the collection
      /// </summary>
      /// <param name="results">Input Async Result Collection</param>
      /// <param name="Success">The function to execute if Result is Success</param>
      /// <param name="Failure">The function to execute if Result is Error</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <returns>Collection after executing function on each element</returns>
      public static IEnumerable<Task> MatchEach<TSucc>(this IEnumerable<Task<Result<TSucc>>> results,
         Action<TSucc> Success, Action<Exception> Failure)
      {
         return results.Select(x => x.Match(Success, Failure));
      }
      
      /// <summary>
      /// Converts a Result Collection into an Async Result Collection
      /// </summary>
      /// <param name="results">Result Collection to convert</param>
      /// <typeparam name="TSucc">Type contained in Result</typeparam>
      /// <returns>Collection of Async Results</returns>
      public static IEnumerable<Task<Result<TSucc>>> ToAsyncResultCollection<TSucc>(this IEnumerable<Result<TSucc>> results)
      {
         return results.Select(x => x.ToAsyncResult());
      }
      
      /// <summary>
      /// Converts a Collection into an Async Result Collection
      /// </summary>
      /// <param name="results">Result Collection to convert</param>
      /// <typeparam name="TSucc">Type contained in Result</typeparam>
      /// <returns>Collection of Async Results</returns>
      public static IEnumerable<Task<Result<TSucc>>> ToAsyncResultCollection<TSucc>(this IEnumerable<TSucc> results)
      {
         return results.Select(x => x.ToAsyncResult());
      }
    }
}