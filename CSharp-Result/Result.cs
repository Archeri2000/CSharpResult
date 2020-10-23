using System;

namespace CSharp_Result
{
 
    /// <summary>
    /// A C# Result type for implementing Railway-Oriented Programming style code.
    /// </summary>
    /// <typeparam name="TSucc">The return type on Success</typeparam>
    public abstract class Result<TSucc> 
        where TSucc : notnull
    {
        public static implicit operator Result<TSucc>(TSucc val) => new Success<TSucc>(val);
        public static implicit operator Result<TSucc>(Exception err) => new Failure<TSucc>(err);
        
        /// <summary>
        /// Checks if Result is an Error
        /// </summary>
        /// <returns>True if Result is an Error</returns>
        public bool IsError()
        {
            return this.Match(Success:s => false, Failure: e => true);
        }
        
        /// <summary>
        /// Checks if Result contains a Valid Result
        /// </summary>
        /// <returns>True if Result is contains a Valid Result</returns>
        public bool IsResult()
        {
            return !IsError();
        }

        /// <summary>
        /// Unwraps and returns the Valid Result if it exists, otherwise returns a default value
        /// </summary>
        /// <param name="defaultImpl">Optional default value to return</param>
        /// <returns>Valid Result or default value</returns>
        public TSucc ValueOrDefault(TSucc defaultImpl = default)
        {
            return this.Match(
                Success: s => s,
                Failure: e => defaultImpl
            );
        }

        /// <summary>
        /// Unwraps and returns the Error if it exists, otherwise returns a default value
        /// </summary>
        /// <param name="defaultImpl">Optional default value to return</param>
        /// <returns>Error or default value</returns>
        public Exception ErrorOrDefault(Exception defaultImpl = default)
        {
            return this.Match(
                Success: s => defaultImpl,
                Failure: e => e
            );
        }
        
        /// <summary>
        /// If holding a Valid Result, Executes the function with the result as input.
        /// </summary>
        /// <param name="function">The function to execute</param>
        /// <typeparam name="TResult">The type of the result of the computation (unused)</typeparam>
        /// <returns>Either the Valid Result, or an Error</returns>
        public Result<TSucc> Do<TResult>(Func<TSucc,Result<TResult>> function)
        {
            return this.Match(
                Success: s =>
                {
                    return Result.ToResult(s)
                        .Then(function)
                        .Then(x => (Result<TSucc>)s);
                },
                Failure: e => e
            );
        }
        
        /// <summary>
        /// If holding a Valid Result, Executes the function with the result as input.
        /// If any exception is thrown, it is mapped by the mapper function.
        /// </summary>
        /// <param name="function">The function to execute</param>
        /// <param name="mapError">The mapping function for the error</param>
        /// <typeparam name="TResult">The type of the result of the computation (unused)</typeparam>
        /// <returns>Either the Valid Result, or an Error</returns>
        public Result<TSucc> Do<TResult>(Func<TSucc,TResult> function, Func<Exception, Exception> mapError)
        {
            return Do(function.ToResultFunc(mapError));
        }
        
        /// <summary>
        /// If holding a Valid Result, Executes the function with the result as input.
        /// If any exception is thrown, it is mapped by the mapper function.
        /// </summary>
        /// <param name="function">The function to execute</param>
        /// <param name="mapError">The mapping function for the error</param>
        /// <returns>Either the Valid Result, or an Error</returns>
        public Result<TSucc> Do(Action<TSucc> function, Func<Exception, Exception> mapError)
        {
            return Do(function.Unit(), mapError);
        }
        
        /// <summary>
        /// If holding a Valid Result, Executes the function with the result as input.
        /// Returns the result of the computation.
        /// </summary>
        /// <param name="function">The function to execute</param>
        /// <typeparam name="TResult">The type of the result of the computation (unused)</typeparam>
        /// <returns>Either a Valid Result from the computation, or an Error</returns>
        public Result<TResult> Then<TResult>(Func<TSucc, Result<TResult>> function) where TResult : notnull
        {
            return this.Match(
                Success: function,
                Failure: e => e
            );
        }
        
        /// <summary>
        /// If holding a Valid Result, Executes the function with the result as input.
        /// If any exception is thrown, it is mapped by the mapper function, otherwise returns the result of the computation.
        /// </summary>
        /// <param name="function">The function to execute</param>
        /// <param name="mapError">The mapping function for the error</param>
        /// <typeparam name="TResult">The type of the result of the computation (unused)</typeparam>
        /// <returns>Either a Valid Result from the computation, or an Error</returns>
        public Result<TResult> Then<TResult>(Func<TSucc, TResult> function, Func<Exception, Exception> mapError) where TResult : notnull
        {
            return Then(function.ToResultFunc(mapError));
        }
        
        /// <summary>
        /// If holding a Valid Result, Executes the function with the result as input.
        /// If any exception is thrown, it is mapped by the mapper function, otherwise returns Unit.
        /// </summary>
        /// <param name="function">The function to execute</param>
        /// <param name="mapError">The mapping function for the error</param>
        /// <returns>Either a Valid Result containing Unit, or an Error</returns>
        public Result<Unit> Then(Action<TSucc> function, Func<Exception, Exception> mapError)
        {
            return Then(function.ToResultFunc(mapError));
        }
        
        /// <summary>
        /// If Result is an Error, passes the Error through a mapper function.
        /// </summary>
        /// <param name="mapper">Maps Errors to other Errors</param>
        /// <returns>Mapped Error if Result is Error</returns>
        public Result<TSucc> MapError(Func<Exception, Exception> mapper) 
        {
            return Match<Result<TSucc>>(
                Success: s => s,
                Failure: e => mapper(e)
            );
        }
        
        /// <summary>
        /// Define continuations on the success and failure cases.
        /// If Result is Valid Result, executes Success
        /// Else, executes Failure
        /// </summary>
        /// <param name="Success">Function to execute on Success</param>
        /// <param name="Failure">Function to execute on Failure</param>
        /// <typeparam name="T">Return type of the function</typeparam>
        /// <returns>Object of type T</returns>
        public T Match<T>(Func<TSucc, T> Success, Func<Exception, T> Failure)
        {
            return this switch
            {
                Success<TSucc> s => Success(s),
                Failure<TSucc> e => Failure(e),
                _ => throw new NotSupportedException("Result is of Invalid State!")
            };
        }
        
        /// <summary>
        /// Define continuations on the success and failure cases.
        /// If Result is Valid Result, executes Success
        /// Else, executes Failure
        /// </summary>
        /// <param name="Success">Function to execute on Success</param>
        /// <param name="Failure">Function to execute on Failure</param>
        /// <returns>Returns Unit, denoting null return (void function)</returns>
        public Unit Match(Action<TSucc> Success, Action<Exception> Failure)
        {
            return Match(Success.Unit(), Failure.Unit());
        }
    }
    
    /// <summary>
    /// The Success case, containing a Valid Result
    /// </summary>
    /// <typeparam name="TSucc">The type of the Valid Result</typeparam>
    public sealed class Success<TSucc>:Result<TSucc> where TSucc : notnull
    {
        private readonly TSucc _val;
        public Success(TSucc val)
        {
            _val = val;
        }
        
        public TSucc Get()
        {
            return _val;
        }

        public override string ToString()
        {
            return _val?.ToString()??"";
        }

        public static implicit operator TSucc(Success<TSucc> s) => s.Get();
    }

    /// <summary>
    /// The Failure case, containing an Error (Expected Exception)
    /// </summary>
    /// <typeparam name="TSucc">The type of the Valid Result (for chaining purposes)</typeparam>
    public sealed class Failure<TSucc> : Result<TSucc> where TSucc : notnull
    {
        private readonly Exception _err;

        public Failure(Exception err)
        {
            _err = err;
        }

        public Exception Get()
        {
            return _err;
        }

        public override string ToString()
        {
            return _err?.ToString()??"";
        }

        public static implicit operator Exception(Failure<TSucc> e) => e.Get();
    }
    
    /// <summary>
    /// Extension Methods on the Result Type
    /// </summary>
    public static class Result
    {
        /// <summary>
        /// Converts an object into a Valid Result
        /// </summary>
        /// <param name="obj">Object to convert</param>
        /// <typeparam name="TSucc">Type of the object</typeparam>
        /// <returns>A Valid Result containing the object</returns>
        public static Result<TSucc> ToResult<TSucc>(this TSucc obj)
        {
            return new Success<TSucc>(obj);
        }

        public static Func<TInput, Result<TSucc>> ToResultFunc<TInput, TSucc>(this Func<TInput, TSucc> func, Func<Exception, Exception> mapError)
        {
            return (x =>
            {
                try
                {
                    var res = func(x);
                    return new Success<TSucc>(res);
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
        /// <returns>Function that outputs a Valid Result of Unit if successful, converting relevant Exceptions into Errors</returns>
        public static Func<TInput, Result<Unit>> ToResultFunc<TInput>(this Action<TInput> func, Func<Exception, Exception> mapError)
        {
            return ToResultFunc(func.Unit(), mapError);
        }
    }
}