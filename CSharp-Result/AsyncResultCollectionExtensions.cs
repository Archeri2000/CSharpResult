using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CSharp_Result.Errors;

namespace CSharp_Result
{
    public static class AsyncResultCollectionExtensions
    {
        /// <summary>
        /// Checks if all the results in the Collection are not Failures.
        /// </summary>
        /// <param name="collection">Collection of Async Results to check</param>
        /// <typeparam name="T">Type contained in Success</typeparam>
        /// <returns>True if all results are not Failures</returns>
        public static async Task<bool> AllSucceed<T>(this IEnumerable<Task<Result<T>>> collection) 
            where T : notnull
        {
           await foreach (var result in collection.ToIAsyncEnumerable())
           {
              if (result.IsFailure())
              {
                 return false;
              }
           }
           return true;
        }
        
        /// <summary>
        /// Checks if at least one result in the Collection is not a Failure
        /// </summary>
        /// <param name="collection">Collection of Async Results to check</param>
        /// <typeparam name="T">Type contained in Success</typeparam>
        /// <returns>True if at least one result is not a Failure</returns>
        public static async Task<bool> AnySucceed<T>(this IEnumerable<Task<Result<T>>> collection) 
            where T : notnull
        {
           await foreach (var result in collection.ToIAsyncEnumerable())
           {
              if (result.IsSuccess())
              {
                 return true;
              }
           }
           return false;           
        }
        
        /// <summary>
        /// Checks if all the results in the Collection are Failures.
        /// </summary>
        /// <param name="collection">Collection of Async Results to check</param>
        /// <typeparam name="T">Type contained in Success</typeparam>
        /// <returns>True if all results are Failures</returns>
        public static async Task<bool> AllFail<T>(this IEnumerable<Task<Result<T>>> collection) 
            where T : notnull
        {
           await foreach (var result in collection.ToIAsyncEnumerable())
           {
              if (result.IsSuccess())
              {
                 return false;
              }
           }
           return true;        
        }
        
        /// <summary>
        /// Checks if at least one result in the Collection is a Failure
        /// </summary>
        /// <param name="collection">Collection of Async Results to check</param>
        /// <typeparam name="T">Type contained in Success</typeparam>
        /// <returns>True if at least one result is a Failure</returns>
        public static async Task<bool> AnyFail<T>(this IEnumerable<Task<Result<T>>> collection) 
            where T : notnull
        {
           await foreach (var result in collection.ToIAsyncEnumerable())
           {
              if (result.IsFailure())
              {
                 return true;
              }
           }
            return false;
        }
        
        /// <summary>
        /// Returns all the contents of the underlying Results if all elements succeed, otherwise throw an aggregate exception with all failures
        /// </summary>
        /// <param name="collection">The collection to parse</param>
        /// <typeparam name="TSucc">The type of the success wrapped</typeparam>
        /// <returns>The collection of successes if they all succeed</returns>
        /// <exception cref="AggregateException">The collection of exceptions if any fail</exception>
        public static async Task<IEnumerable<TSucc>> Get<TSucc>(this IEnumerable<Task<Result<TSucc>>> collection)
           where TSucc : notnull
        {
           var failures = new List<Exception>();
           var successes = new List<TSucc>();
           await foreach (var result in collection.ToIAsyncEnumerable())
           {
              if (result.IsFailure())
              {
                 failures.Add(result.FailureOrDefault());
              }
              else
              {
                 successes.Add(result.SuccessOrDefault());
              }
           }

           if (failures.Any())
           {
              throw new AggregateException(failures);
           }
           return successes;
        }

        /// <summary>
        /// Gets all the Failures 
        /// </summary>
        /// <param name="collection">Collection of Async Results to filter</param>
        /// <typeparam name="T">Type contained in Success</typeparam>
        /// <returns>IEnumerable of all the Failures in the Collection</returns>
        public static async IAsyncEnumerable<Exception> GetFailures<T>(this IEnumerable<Task<Result<T>>> collection) 
            where T : notnull
        {
           await foreach (var result in collection.ToIAsyncEnumerable())
           {
              if (result.IsFailure())
              {
                 yield return result.FailureOrDefault();
              }
           }
        }
        
        /// <summary>
        /// Gets all the Successes 
        /// </summary>
        /// <param name="collection">Collection of Async Results to filter</param>
        /// <typeparam name="T">Type contained in Success</typeparam>
        /// <returns>IEnumerable of all the Successes in the Collection</returns>
        public static async IAsyncEnumerable<T> GetSuccesses<T>(this IEnumerable<Task<Result<T>>> collection) 
            where T : notnull
        {
           await foreach (var result in collection.ToIAsyncEnumerable())
           {
              if (result.IsSuccess())
              {
                 yield return result.SuccessOrDefault();
              }
           }        
        }

       /// <summary>
       /// Awaits all the Async Results in the collection
       /// </summary>
       /// <param name="results">Collection of Async Results</param>
       /// <typeparam name="TSucc">Type of Result</typeparam>
       /// <returns>A Task returning a Collection of Async Results</returns>
        public static async Task<IEnumerable<Result<TSucc>>> AwaitAll<TSucc>(this IEnumerable<Task<Result<TSucc>>> results)
        {
           var awaited = await Task.WhenAll(results.ToList());
           return awaited.AsEnumerable();
        }
        
       /// <summary>
       /// Awaits any Async Result from a collection
       /// </summary>
       /// <param name="results">Collection of Async Results</param>
       /// <typeparam name="TSucc">Type of Result</typeparam>
       /// <returns>A Task returning a Result</returns>
        public static async Task<Result<TSucc>> AwaitAny<TSucc>(this IEnumerable<Task<Result<TSucc>>> results)
        {
           return (await Task.WhenAny(results)).Result;
        }

       
       /// <summary>
       /// Converts a collection of Async Results to an Async Enumerable of Results
       /// </summary>
       /// <param name="results">Collection of Async Results</param>
       /// <typeparam name="TSucc">Type of Result</typeparam>
       /// <returns>An async enumerable of Results</returns>
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
      /// <param name="type">The type of Do function to use</param>
      /// <param name="function">The function to execute</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <typeparam name="TResult">The type of the result of the computation (unused)</typeparam>
      /// <returns>Collection after executing function on each element</returns>
      public static IEnumerable<Task<Result<TSucc>>> DoAwaitEach<TSucc, TResult>(this IEnumerable<Task<Result<TSucc>>> results, DoType type, Func<TSucc, Task<Result<TResult>>> function) 
         where TSucc : notnull
         where TResult : notnull
        {
           return results.Select(x => x.DoAwait(type, function));
        }

      /// <summary>
      /// Executes Async Do on each element of the collection
      /// </summary>
      /// <param name="results">Input Async Result Collection</param>
      /// <param name="type">The type of Do function to use</param>
      /// <param name="function">The function to execute</param>
      /// <param name="mapException">The mapping function for the error</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <typeparam name="TResult">The type of the result of the computation (unused)</typeparam>
      /// <returns>Collection after executing function on each element</returns>
      public static IEnumerable<Task<Result<TSucc>>> DoAwaitEach<TSucc, TResult>(this IEnumerable<Task<Result<TSucc>>> results, DoType type, Func<TSucc, Task<TResult>> function, ExceptionFilter mapException) 
         where TSucc : notnull
         where TResult : notnull
      {
         return results.Select(x => x.DoAwait(type, function, mapException));
      }
      
      /// <summary>
      /// Executes Async Do on each element of the collection
      /// </summary>
      /// <param name="results">Input Async Result Collection</param>
      /// <param name="type">The type of Do function to use</param>
      /// <param name="function">The function to execute</param>
      /// <param name="mapException">The mapping function for the error</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <returns>Collection after executing function on each element</returns>
      public static IEnumerable<Task<Result<TSucc>>> DoAwaitEach<TSucc>(this IEnumerable<Task<Result<TSucc>>> results, DoType type, Func<TSucc, Task> function, ExceptionFilter mapException) 
         where TSucc : notnull
      {
         return results.Select(x => x.DoAwait(type, function, mapException));
      }
      
      /// <summary>
      /// Executes Do on each element of the collection
      /// </summary>
      /// <param name="results">Input Async Result Collection</param>
      /// <param name="type">The type of Do function to use</param>
      /// <param name="function">The function to execute</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <typeparam name="TResult">The type of the result of the computation (unused)</typeparam>
      /// <returns>Collection after executing function on each element</returns>
      public static IEnumerable<Task<Result<TSucc>>> DoEach<TSucc, TResult>(this IEnumerable<Task<Result<TSucc>>> results, DoType type, Func<TSucc, Result<TResult>> function) 
         where TSucc : notnull
      {
         return results.Select(x => x.Do(type, function));
      }

      /// <summary>
      /// Executes Do on each element of the collection
      /// </summary>
      /// <param name="results">Input Async Result Collection</param>
      /// <param name="type">The type of Do function to use</param>
      /// <param name="function">The function to execute</param>
      /// <param name="mapException">The mapping function for the error</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <typeparam name="TResult">The type of the result of the computation (unused)</typeparam>
      /// <returns>Collection after executing function on each element</returns>
      public static IEnumerable<Task<Result<TSucc>>> DoEach<TSucc, TResult>(this IEnumerable<Task<Result<TSucc>>> results, DoType type, Func<TSucc, TResult> function, ExceptionFilter mapException) 
         where TSucc : notnull
      {
         return results.Select(x => x.Do(type, function, mapException));
      }
      
      /// <summary>
      /// Executes Do on each element of the collection
      /// </summary>
      /// <param name="results">Input Async Result Collection</param>
      /// <param name="type">The type of Do function to use</param>
      /// <param name="function">The function to execute</param>
      /// <param name="mapException">The mapping function for the error</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <returns>Collection after executing function on each element</returns>
      public static IEnumerable<Task<Result<TSucc>>> DoEach<TSucc>(this IEnumerable<Task<Result<TSucc>>> results, DoType type, Action<TSucc> function, ExceptionFilter mapException) 
         where TSucc : notnull
      {
         return results.Select(x => x.Do(type, function, mapException));
      }

      /// <summary>
      /// Executes Async Then on each element of the collection
      /// </summary>
      /// <param name="results">Input Async Result Collection</param>
      /// <param name="function">The function to execute</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <typeparam name="TResult">The type of the result of the computation</typeparam>
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
      /// <typeparam name="TResult">The type of the result of the computation</typeparam>
      /// <returns>Collection after executing function on each element</returns>
      public static IEnumerable<Task<Result<TResult>>> ThenAwaitEach<TSucc, TResult>(this IEnumerable<Task<Result<TSucc>>> results, Func<TSucc, Task<TResult>> function, ExceptionFilter mapException) 
         where TSucc : notnull
         where TResult : notnull
      {
         return results.Select(x => x.ThenAwait(function, mapException));
      }
      
      /// <summary>
      /// Executes Async Then on each element of the collection
      /// </summary>
      /// <param name="results">Input Async Result Collection</param>
      /// <param name="function">The function to execute</param>
      /// <param name="mapException">The mapping function for the error</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <returns>Collection after executing function on each element</returns>
      public static IEnumerable<Task<Result<Unit>>> ThenAwaitEach<TSucc>(this IEnumerable<Task<Result<TSucc>>> results, Func<TSucc, Task> function, ExceptionFilter mapException) 
         where TSucc : notnull
      {
         return results.Select(x => x.ThenAwait(function, mapException));
      }
      
      /// <summary>
      /// Executes Then on each element of the collection
      /// </summary>
      /// <param name="results">Input Async Result Collection</param>
      /// <param name="function">The function to execute</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <typeparam name="TResult">The type of the result of the computation</typeparam>
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
      /// <typeparam name="TResult">The type of the result of the computation</typeparam>
      /// <returns>Collection after executing function on each element</returns>
      public static IEnumerable<Task<Result<TResult>>> ThenEach<TSucc, TResult>(this IEnumerable<Task<Result<TSucc>>> results, Func<TSucc, TResult> function, ExceptionFilter mapException) 
         where TSucc : notnull
         where TResult : notnull
      {
         return results.Select(x => x.Then(function, mapException));
      }
      
      /// <summary>
      /// Executes Then on each element of the collection
      /// </summary>
      /// <param name="results">Input Async Result Collection</param>
      /// <param name="function">The function to execute</param>
      /// <param name="mapException">The mapping function for the error</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <returns>Collection after executing function on each element</returns>
      public static IEnumerable<Task<Result<Unit>>> ThenEach<TSucc>(this IEnumerable<Task<Result<TSucc>>> results, Action<TSucc> function, ExceptionFilter mapException) 
         where TSucc : notnull
      {
         return results.Select(x => x.Then(function, mapException));

      }
      
      /// <summary>
      /// Executes Async Assert on each element of the collection
      /// </summary>
      /// <param name="results">Input Async Result Collection</param>
      /// <param name="assertion">The assertion to execute</param>
      /// <param name="assertionMessage">The message in the AssertionException when the assertion fails</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <returns>Collection after executing function on each element</returns>
      public static IEnumerable<Task<Result<TSucc>>> AssertAwaitEach<TSucc>(this IEnumerable<Task<Result<TSucc>>> results, Func<TSucc?, Task<Result<bool>>> assertion, string? assertionMessage = null) 
         where TSucc : notnull
      {
         return results.Select(x => x.AssertAwait(assertion, assertionMessage));
      }

      /// <summary>
      /// Executes Async Assert on each element of the collection
      /// </summary>
      /// <param name="results">Input Async Result Collection</param>
      /// <param name="assertion">The assertion to execute</param>
      /// <param name="mapException">The mapping function for the error</param>
      /// <param name="assertionMessage">The message in the AssertionException when the assertion fails</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <returns>Collection after executing function on each element</returns>
      public static IEnumerable<Task<Result<TSucc>>> AssertAwaitEach<TSucc>(this IEnumerable<Task<Result<TSucc>>> results, Func<TSucc?, Task<bool>> assertion, ExceptionFilter mapException, string? assertionMessage = null) 
         where TSucc : notnull
      {
         return results.Select(x => x.AssertAwait(assertion, mapException, assertionMessage));
      }

      /// <summary>
      /// Executes Assert on each element of the collection
      /// </summary>
      /// <param name="results">Input Async Result Collection</param>
      /// <param name="assertion">The assertion to execute</param>
      /// <param name="assertionMessage">The message in the AssertionException when the assertion fails</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <returns>Collection after executing function on each element</returns>
      public static IEnumerable<Task<Result<TSucc>>> AssertEach<TSucc>(this IEnumerable<Task<Result<TSucc>>> results, Func<TSucc?, Result<bool>> assertion, string? assertionMessage = null) 
         where TSucc : notnull
      {
         return results.Select(x => x.Assert(assertion, assertionMessage));
      }

      /// <summary>
      /// Executes Assert on each element of the collection
      /// </summary>
      /// <param name="results">Input Async Result Collection</param>
      /// <param name="assertion">The assertion to execute</param>
      /// <param name="mapException">The mapping function for the error</param>
      /// <param name="assertionMessage">The message in the AssertionException when the assertion fails</param>
      /// <typeparam name="TSucc">Input type</typeparam>
      /// <returns>Collection after executing function on each element</returns>
      public static IEnumerable<Task<Result<TSucc>>> AssertEach<TSucc>(this IEnumerable<Task<Result<TSucc>>> results, Func<TSucc?, bool> assertion, ExceptionFilter mapException, string? assertionMessage = null) 
         where TSucc : notnull
      {
         return results.Select(x => x.Assert(assertion, mapException, assertionMessage));
      }
      
      /// <summary>
      /// Executes If on each element of the collection
      /// Both Then and Else should return the same type.
      /// </summary>
      /// <param name="results">Input Async Result Collection</param>
      /// <param name="predicate">The predicate to check</param>
      /// <param name="Then">The function to execute if predicate returns True</param>
      /// <param name="Else">The function to execute if predicate returns False</param>
      /// <returns>Collection after executing function on each element</returns>
      public static IEnumerable<Task<Result<TResult>>> IfEach<TSucc, TResult>(this IEnumerable<Task<Result<TSucc>>> results, Func<TSucc?,Result<bool>> predicate, Func<TSucc?, Result<TResult>> Then, Func<TSucc?, Result<TResult>> Else)
      {
         return results.Select(x => x.If(predicate, Then, Else));
      }
      
      /// <summary>
      /// Executes If on each element of the collection
      /// Both Then and Else should return the same type.
      /// </summary>
      /// <param name="results">Input Async Result Collection</param>
      /// <param name="predicate">The async predicate to check</param>
      /// <param name="Then">The async function to execute if predicate returns True</param>
      /// <param name="Else">The async function to execute if predicate returns False</param>
      /// <returns>Collection after executing function on each element</returns>
      public static IEnumerable<Task<Result<TResult>>> IfAwaitEach<TSucc, TResult>(this IEnumerable<Task<Result<TSucc>>> results, Func<TSucc,Task<Result<bool>>> predicate, Func<TSucc, Task<Result<TResult>>> Then, Func<TSucc, Task<Result<TResult>>> Else)
      {
         return results.Select(x => x.IfAwait(predicate, Then, Else));
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