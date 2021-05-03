using System;
using static CSharp_Result.Errors;

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
        /// When comparing two Results, unwrap and compare them by the inner wrapped values.
        /// </summary>
        /// <param name="obj">Other object to compare to</param>
        /// <returns>true if the inner wrapped values are equal</returns>
        public override bool Equals(object? obj)
        {
            if (obj is not Result<TSucc>) return false;
            
            var other = (Result<TSucc>) obj;
            return Match(
                x => other.Match(y => x.Equals(y), _ => false),
                x => other.Match(_ => false, y => x.Equals(y)));
        }

        /// <summary>
        /// Overriding get hash code to determine the hash code based on the contents of the Result.
        /// </summary>
        /// <returns>Hash code of the contents of the Result</returns>
        public override int GetHashCode()
        {
            return Match(x => x.GetHashCode(), x => x.GetHashCode());
        }

        /// <summary>
        /// Checks if Result is a Failure
        /// </summary>
        /// <returns>True if Result is a Failure</returns>
        public bool IsFailure()
        {
            return this.Match(Success:s => false, Failure: e => true);
        }
        
        /// <summary>
        /// Checks if Result contains a Success
        /// </summary>
        /// <returns>True if Result is contains a Success</returns>
        public bool IsSuccess()
        {
            return !IsFailure();
        }
        
        /// <summary>
        /// Checks if Result is a Failure, returning the failure in the out parameter
        /// </summary>
        /// <param name="error">out parameter to hold the error value</param>
        /// <returns>True if Result is a Failure</returns>
        public bool IsFailure(out Exception error)
        {
            bool ret;
            (error, ret) = this.Match(Success:_ => (null, false), Failure: e => (e, true));
            return ret;
        }
        
        /// <summary>
        /// Checks if Result contains a Success, returning the success in the out parameter
        /// </summary>
        /// <param name="success">out parameter to hold the success value</param>
        /// <returns>True if Result is contains a Success</returns>
        public bool IsSuccess(out TSucc success)
        {
            bool ret;
            (success, ret) = this.Match(Success: s => (s, true), Failure: _ => (default, false));
            return ret;
        }

        /// <summary>
        /// Returns the contents of the Result if successful, or throws the exception if it failed.
        /// </summary>
        /// <returns></returns>
        public TSucc Get()
        {
            return Match(
                x => x,
                err => throw err
            );
        }

        /// <summary>
        /// Unwraps and returns the Success if it exists, otherwise returns a default value
        /// </summary>
        /// <param name="defaultImpl">Optional default value to return</param>
        /// <returns>Success or default value</returns>
        public TSucc SuccessOrDefault(TSucc defaultImpl = default)
        {
            return this.Match(
                Success: s => s,
                Failure: e => defaultImpl
            );
        }

        /// <summary>
        /// Unwraps and returns the Failure if it exists, otherwise returns a default value
        /// </summary>
        /// <param name="defaultImpl">Optional default value to return</param>
        /// <returns>Failure or default value</returns>
        public Exception FailureOrDefault(Exception defaultImpl = default)
        {
            return this.Match(
                Success: s => defaultImpl,
                Failure: e => e
            );
        }
        
        /// <summary>
        /// If holding a Success, Executes the function with the result as input.
        /// </summary>
        /// <param name="function">The function to execute</param>
        /// <typeparam name="TResult">The type of the result of the computation (unused)</typeparam>
        /// <returns>Either the Success, or a Failure</returns>
        public Result<TSucc> Do<TResult>(Func<TSucc,Result<TResult>> function)
        {
            return this.Match(
                Success: s =>
                {
                    return s.ToResult()
                        .Then(function)
                        .Then(x => (Result<TSucc>)s);
                },
                Failure: e => e
            );
        }
        
        /// <summary>
        /// If holding a Success, Executes the function with the result as input.
        /// If any exception is thrown, it is mapped by the mapper function.
        /// </summary>
        /// <param name="function">The function to execute</param>
        /// <param name="mapException">The mapping function for the error</param>
        /// <typeparam name="TResult">The type of the result of the computation (unused)</typeparam>
        /// <returns>Either the Success, or a Failure</returns>
        public Result<TSucc> Do<TResult>(Func<TSucc,TResult> function, ExceptionFilter mapException)
        {
            return Do(function.ToResultFunc(mapException));
        }
        
        /// <summary>
        /// If holding a Success, Executes the function with the result as input.
        /// If any exception is thrown, it is mapped by the mapper function.
        /// </summary>
        /// <param name="function">The function to execute</param>
        /// <param name="mapException">The mapping function for the error</param>
        /// <returns>Either the Success, or a Failure</returns>
        public Result<TSucc> Do(Action<TSucc> function, ExceptionFilter mapException)
        {
            return Do(function.Unit(), mapException);
        }
        
        /// <summary>
        /// If holding a Success, Executes the function with the result as input.
        /// Returns the result of the computation.
        /// </summary>
        /// <param name="function">The function to execute</param>
        /// <typeparam name="TResult">The type of the result of the computation</typeparam>
        /// <returns>Either a Success from the computation, or a Failure</returns>
        public Result<TResult> Then<TResult>(Func<TSucc, Result<TResult>> function) where TResult : notnull
        {
            return this.Match(
                Success: function,
                Failure: e => e
            );
        }
        
        /// <summary>
        /// If holding a Success, Executes the function with the result as input.
        /// If any exception is thrown, it is mapped by the mapper function, otherwise returns the result of the computation.
        /// </summary>
        /// <param name="function">The function to execute</param>
        /// <param name="mapException">The mapping function for the error</param>
        /// <typeparam name="TResult">The type of the result of the computation</typeparam>
        /// <returns>Either a Success from the computation, or a Failure</returns>
        public Result<TResult> Then<TResult>(Func<TSucc, TResult> function, ExceptionFilter mapException) where TResult : notnull
        {
            return Then(function.ToResultFunc(mapException));
        }
        
        /// <summary>
        /// If holding a Success, Executes the function with the result as input.
        /// If any exception is thrown, it is mapped by the mapper function, otherwise returns Unit.
        /// </summary>
        /// <param name="function">The function to execute</param>
        /// <param name="mapException">The mapping function for the error</param>
        /// <returns>Either a Success containing Unit, or a Failure</returns>
        public Result<Unit> Then(Action<TSucc> function, ExceptionFilter mapException)
        {
            return Then(function.ToResultFunc(mapException));
        }
        
        /// <summary>
        /// If holding a Success, Executes the function with the result as input. If the function returns False, returns a failure.
        /// </summary>
        /// <param name="predicate">The function to execute</param>
        /// <returns>Either the Success, or a Failure</returns>
        public Result<TSucc> If(Func<TSucc,Result<bool>> predicate)
        {
            return this.Match(
                Success: s =>
                {
                    return s.ToResult()
                        .Then(predicate)
                        .Then(x => x?(Result<TSucc>)s:(Result<TSucc>)new Exception("Predicate returned false!"));
                },
                Failure: e => e
            );
        }
        
        /// <summary>
        /// If holding a Success, Executes the function with the result as input. If the function returns False, returns a failure.
        /// If any exception is thrown, it is mapped by the mapper function.
        /// </summary>
        /// <param name="function">The function to execute</param>
        /// <param name="mapException">The mapping function for the error</param>
        /// <returns>Either the Success, or a Failure</returns>
        public Result<TSucc> If(Func<TSucc,bool> function, ExceptionFilter mapException)
        {
            return If(function.ToResultFunc(mapException));
        }

        /// <summary>
        /// If Result is a Failure, passes the Failure through a mapper function.
        /// </summary>
        /// <param name="mapper">Maps Failures to other Failures</param>
        /// <returns>Mapped Failure if Result is Failure</returns>
        public Result<TSucc> MapFailure(Func<Exception, Exception> mapper) 
        {
            return Match<Result<TSucc>>(
                Success: s => s,
                Failure: e => mapper(e)
            );
        }
        
        /// <summary>
        /// Define continuations on the success and failure cases.
        /// If Result is Success, executes Success
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
        /// If Result is Success, executes Success
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
    /// The Success case, containing a Success
    /// </summary>
    /// <typeparam name="TSucc">The type of the Success</typeparam>
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
    /// The Failure case, containing a Failure (Expected Exception)
    /// </summary>
    /// <typeparam name="TSucc">The type of the Success (for chaining purposes)</typeparam>
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
        /// Converts an object into a Success
        /// </summary>
        /// <param name="obj">Object to convert</param>
        /// <typeparam name="TSucc">Type of the object</typeparam>
        /// <returns>A Success containing the object</returns>
        public static Result<TSucc> ToResult<TSucc>(this TSucc obj)
        {
            return new Success<TSucc>(obj);
        }

        public static Func<TInput, Result<TSucc>> ToResultFunc<TInput, TSucc>(this Func<TInput, TSucc> func, ExceptionFilter mapException)
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
                    if (!mapException(e)) throw;
                    return e;
                }
            });
        }
        
        /// <summary>
        /// Converts a normal single argument void function into a new function that outputs a Result.
        /// </summary>
        /// <param name="func">Function to execute</param>
        /// <param name="mapException">Exceptions to catch and map to Failure</param>
        /// <typeparam name="TInput">Input type</typeparam>
        /// <returns>Function that outputs a Success of Unit if successful, converting relevant Exceptions into Failure</returns>
        public static Func<TInput, Result<Unit>> ToResultFunc<TInput>(this Action<TInput> func, ExceptionFilter mapException)
        {
            return ToResultFunc(func.Unit(), mapException);
        }

        /// <summary>
        /// Checks if value contained in Result is not null
        /// </summary>
        /// <param name="result">Result to check</param>
        /// <typeparam name="TSucc">Type of the value contained</typeparam>
        /// <returns>If value in Result is null, returns a NulLReferenceException as Failure</returns>
        public static Result<TSucc> IsNotNull<TSucc>(this Result<TSucc> result)
        {
            return result.Match(
                Success: x => (Result<TSucc>)x?? (Result<TSucc>)new NullReferenceException(),
                Failure: x => x
            );
        }
    }
}