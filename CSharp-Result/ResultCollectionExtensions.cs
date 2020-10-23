using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharp_Result
{
    /// <summary>
    /// A set of extension methods to convert between Results of Collections and Collections of Results
    /// </summary>
    public static class ResultCollectionExtensions
    {
        /// <summary>
        /// Checks if all the results in the Collection are not Errors.
        /// </summary>
        /// <param name="collection">Collection of results to check</param>
        /// <typeparam name="T">Type contained in Valid Result</typeparam>
        /// <returns>True if all results are not Errors</returns>
        public static bool AllSucceed<T>(this IEnumerable<Result<T>> collection) 
            where T : notnull
        {
            return collection.All(x => x.IsResult());
        }
        
        /// <summary>
        /// Checks if at least one result in the Collection is not an Error
        /// </summary>
        /// <param name="collection">Collection of results to check</param>
        /// <typeparam name="T">Type contained in Valid Result</typeparam>
        /// <returns>True if at least one result is not an Error</returns>
        public static bool AnySucceed<T>(this IEnumerable<Result<T>> collection) 
            where T : notnull
        {
            return collection.Any(x => x.IsResult());
        }
        
        /// <summary>
        /// Checks if all the results in the Collection are Errors.
        /// </summary>
        /// <param name="collection">Collection of results to check</param>
        /// <typeparam name="T">Type contained in Valid Result</typeparam>
        /// <returns>True if all results are Errors</returns>
        public static bool AllFail<T>(this IEnumerable<Result<T>> collection) 
            where T : notnull
        {
            return collection.All(x => x.IsError());
        }
        
        /// <summary>
        /// Checks if at least one result in the Collection is an Error
        /// </summary>
        /// <param name="collection">Collection of results to check</param>
        /// <typeparam name="T">Type contained in Valid Result</typeparam>
        /// <returns>True if at least one result is an Error</returns>
        public static bool AnyFail<T>(this IEnumerable<Result<T>> collection) 
            where T : notnull
        {
            return collection.Any(x => x.IsError());
        }

        /// <summary>
        /// Gets all the Errors 
        /// </summary>
        /// <param name="collection">Collection of results to filter</param>
        /// <typeparam name="T">Type contained in Valid Result</typeparam>
        /// <returns>IEnumerable of all the Errors in the Collection</returns>
        public static IEnumerable<Exception> GetFailures<T>(this IEnumerable<Result<T>> collection) 
            where T : notnull
        {
            return collection.Where(x => x.IsError()).Select(x => x.ErrorOrDefault());
        }
        
        /// <summary>
        /// Gets all the Valid Results 
        /// </summary>
        /// <param name="collection">Collection of results to filter</param>
        /// <typeparam name="T">Type contained in Valid Result</typeparam>
        /// <returns>IEnumerable of all the Valid Results in the Collection</returns>
        public static IEnumerable<T> GetSuccesses<T>(this IEnumerable<Result<T>> collection) 
            where T : notnull
        {
            return collection.Where(x => x.IsResult()).Select(x => x.ValueOrDefault());
        }

        /// <summary>
        /// Converts a Sequence of Results into a Result of Sequence.
        /// </summary>
        /// <param name="collection">Collection of results to filter</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>A Result of a sequence</returns>
        public static Result<IEnumerable<T>> ToResultOfSeq<T>(this IEnumerable<Result<T>> collection) 
            where T : notnull
        {
            var enumerable = collection.ToList();
            var errors = enumerable.GetFailures();
            var innerExceptions = errors.ToList();
            if (innerExceptions.Any())
            {
                return new AggregateException(innerExceptions);
            }
            return new Success<IEnumerable<T>>(enumerable.GetSuccesses());
        }
        
        /// <summary>
        /// Converts a Result of a Sequence into a Sequence of Results
        /// </summary>
        /// <param name="collection">Result of collection to process</param>
        /// <typeparam name="T">Type contained in sequence</typeparam>
        /// <returns>A sequence of results</returns>
        public static IEnumerable<Result<T>> ToSeqOfResults<T>(this Result<IEnumerable<T>> collection) 
            where T : notnull
        {
            if (collection.IsError())
            {
                return new List<Result<T>>{collection.ErrorOrDefault()};
            }
            else
            {
                return collection.ValueOrDefault().Select(r => new Success<T>(r));
            }
        }
        
        //TODO: Add Linq???
        
        public static IEnumerable<Result<TSucc>> DoEach<TSucc, TResult>(this IEnumerable<Result<TSucc>> result,
            Func<TSucc, Result<TResult>> function)
            where TSucc: notnull
            where TResult: notnull
        {
            return result.Select(x => x.Do(function));
        }
        
        public static IEnumerable<Result<TSucc>> DoEach<TSucc, TResult>(this IEnumerable<Result<TSucc>> result,
            Func<TSucc, TResult> function, Func<Exception, Exception> mapError)
            where TSucc: notnull
        {
            return result.Select(x => x.Do(function, mapError));
        }
        
        public static IEnumerable<Result<TSucc>> DoEach<TSucc>(this IEnumerable<Result<TSucc>> result,
            Action<TSucc> function, Func<Exception, Exception> mapError)
            where TSucc: notnull
        {
            return result.Select(x => x.Do(function, mapError));
        }

        public static IEnumerable<Result<TResult>> ThenEach<TSucc, TResult>(this IEnumerable<Result<TSucc>> result,
            Func<TSucc, Result<TResult>> function)
            where TSucc: notnull
            where TResult: notnull
        {
            return result.Select(x => x.Then(function));
        }
        
        public static IEnumerable<Result<TResult>> ThenEach<TSucc, TResult>(this IEnumerable<Result<TSucc>> result,
            Func<TSucc, TResult> function, Func<Exception, Exception> mapError)
            where TSucc: notnull
            where TResult: notnull
        {
            return result.Select(x => x.Then(function, mapError));
        }
        
        public static IEnumerable<Result<Unit>> ThenEach<TSucc>(this IEnumerable<Result<TSucc>> result,
            Action<TSucc> function, Func<Exception, Exception> mapError)
            where TSucc: notnull
        {
            return result.Select(x => x.Then(function, mapError));
        }
    }
}