using System;
using System.Diagnostics.CodeAnalysis;
using static CSharp_Result.Errors;

namespace CSharp_Result
{

    /// <summary>
    /// The variant of Do function to use (Either ignore returned errors or return an error if it fails)
    /// </summary>
    public enum DoType
    {
        /// <summary>
        /// Any errors that are returned will be mapped to a Failure which is returned from the Do. Exceptions will still fire.
        /// </summary>
        MapErrors, 
        /// <summary>
        /// Any errors that are returned will be ignored (fire and forget). Exceptions will still fire.
        /// </summary>
        Ignore
    }
    
    /// <summary>
    /// Struct representing the Result Type. 
    /// </summary>
    /// <typeparam name="TSucc"></typeparam>
    public readonly struct Result<TSucc>
    {
        private readonly TSucc? _value;
        private readonly Exception? _exception;
        private readonly bool _isSuccess;
        
        /// <summary>
        /// Wraps the value in a result.
        /// </summary>
        /// <param name="value">Value to wrap in result</param>
        public Result(TSucc? value)
        {
            _value = value;
            _isSuccess = true;
            _exception = default;
        }
        
        /// <summary>
        /// Wraps the exception in a result
        /// </summary>
        /// <param name="exception">Exception to wrap in result</param>
        public Result(Exception? exception)
        {
            _exception = exception;
            _isSuccess = false;
            _value = default;
        }
        
        /// <summary>
        /// Implicitly unwraps the Exception in the result if it is a failure, otherwise returns the default value.
        /// </summary>
        /// <param name="result">The result to convert</param>
        /// <returns>The unwrapped Exception</returns>
        public static implicit operator Exception?(Result<TSucc> result) => result._isSuccess ? default : result._exception;
        
        /// <summary>
        /// Implicitly unwraps the value in the result if it is a success, otherwise returns the default value.
        /// </summary>
        /// <param name="result">The result to convert</param>
        /// <returns>The unwrapped value</returns>
        public static implicit operator TSucc?(Result<TSucc> result) => result._isSuccess ? result._value : default;
        
        /// <summary>
        /// Implicitly converts a value to a successful result
        /// </summary>
        /// <param name="val">The value to convert</param>
        /// <returns>A result holding the value</returns>
        public static implicit operator Result<TSucc>(TSucc? val) => new(val);
        
        /// <summary>
        /// Implicitly converts an exception to a failure result
        /// </summary>
        /// <param name="err">The exception value to convert</param>
        /// <returns>A result holding the Exception</returns>
        public static implicit operator Result<TSucc>(Exception? err) => new(err);

        /// <summary>
        /// If result contains an exception, calls ToString on the exception, else calls ToString on the value.
        /// </summary>
        /// <returns>String representation of the underlying object</returns>
        public override string ToString()
        {
            if (_isSuccess)
            {
                return _value?.ToString()??"null";
            }
            return _exception?.ToString()??"null";
        }
        
        /// <summary>
        /// When comparing two Results, unwrap and compare them by the inner wrapped values.
        /// </summary>
        /// <param name="obj">Other object to compare to</param>
        /// <returns>true if the inner wrapped values are equal</returns>
        public override bool Equals(object? obj)
        {
            if (obj is not Result<TSucc> other) return false;

            return Match(
                value => value is not null && other.Match(succ => value.Equals(succ), _ => false),
                exn => exn is not null && other.Match(_ => false, exception => exn.Equals(exception)));
        }

        /// <summary>
        /// Overriding get hash code to determine the hash code based on the contents of the Result.
        /// </summary>
        /// <returns>Hash code of the contents of the Result</returns>
        public override int GetHashCode()
        {
            return Match(
                value =>
                {
                    return value is null ? 0 : value.GetHashCode();
                }, 
                exn => exn is null ? 0 : exn.GetHashCode());
        }

        /// <summary>
        /// Checks if Result is a Failure
        /// </summary>
        /// <returns>True if Result is a Failure</returns>
        public bool IsFailure()
        {
            return !_isSuccess;
        }
        
        /// <summary>
        /// Checks if Result contains a Success
        /// </summary>
        /// <returns>True if Result is contains a Success</returns>
        public bool IsSuccess()
        {
            return _isSuccess;
        }
        
        /// <summary>
        /// Checks if Result is a Failure, returning the failure in the out parameter
        /// </summary>
        /// <param name="error">out parameter to hold the error value</param>
        /// <returns>True if Result is a Failure</returns>
        public bool IsFailure(out Exception? error)
        {
            error = !_isSuccess ? _exception : default;
            return !_isSuccess;
        }
        
        /// <summary>
        /// Checks if Result contains a Success, returning the success in the out parameter
        /// </summary>
        /// <param name="success">out parameter to hold the success value</param>
        /// <returns>True if Result is contains a Success</returns>
        public bool IsSuccess(out TSucc? success)
        {
            success = _isSuccess ? _value : default;
            return _isSuccess;
        }

        /// <summary>
        /// Returns the contents of the Result if successful, or throws the exception if it failed.
        /// </summary>
        /// <returns></returns>
        public TSucc? Get()
        {
            return Match(
                x => x,
                err => throw (err??new NullReferenceException("Expected Exception to throw but received null."))
            );
        }

        /// <summary>
        /// Unwraps and returns the Success if it exists, otherwise returns a default value
        /// </summary>
        /// <param name="defaultImpl">Optional default value to return</param>
        /// <returns>Success or default value</returns>
        public TSucc? SuccessOrDefault(TSucc? defaultImpl = default)
        {
            return Match(
                Success: s => s,
                Failure: _ => defaultImpl
            );
        }

        /// <summary>
        /// Unwraps and returns the Failure if it exists, otherwise returns a default value
        /// </summary>
        /// <param name="defaultImpl">Optional default value to return</param>
        /// <returns>Failure or default value</returns>
        public Exception? FailureOrDefault(Exception? defaultImpl = default)
        {
            return Match(
                Success: _ => defaultImpl,
                Failure: e => e
            );
        }

        /// <summary>
        /// If holding a Success, Executes the function with the result as input.
        /// </summary>
        /// <param name="type">The type of Do function to use</param>
        /// <param name="function">The function to execute</param>
        /// <typeparam name="TResult">The type of the result of the computation (unused)</typeparam>
        /// <returns>Either the Success, or a Failure</returns>
        public Result<TSucc> Do<TResult>(DoType type, Func<TSucc?,Result<TResult>> function)
        {
            var curr = this;
            return type switch
            {
                DoType.MapErrors => Then(function).Then(_ => curr),
                DoType.Ignore => Then(s =>
                {
                    function(s);
                    return curr;
                }),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Invalid DoType!")
            };
        }

        /// <summary>
        /// If holding a Success, Executes the function with the result as input.
        /// If any exception is thrown, it is mapped by the mapper function.
        /// </summary>
        /// <param name="type">Either Ignore Errors or Map Encountered Errors</param>
        /// <param name="function">The function to execute</param>
        /// <param name="mapException">The mapping function for the error</param>
        /// <typeparam name="TResult">The type of the result of the computation (unused)</typeparam>
        /// <returns>Either the Success, or a Failure</returns>
        public Result<TSucc> Do<TResult>(DoType type, Func<TSucc?,TResult> function, ExceptionFilter mapException)
        {
            return Do(type, function.ToResultFunc(mapException));
        }
        
        /// <summary>
        /// If holding a Success, Executes the function with the result as input.
        /// If any exception is thrown, it is mapped by the mapper function.
        /// </summary>
        /// <param name="type">Either Ignore Errors or Map Encountered Errors</param>
        /// <param name="function">The function to execute</param>
        /// <param name="mapException">The mapping function for the error</param>
        /// <returns>Either the Success, or a Failure</returns>
        public Result<TSucc> Do(DoType type, Action<TSucc?> function, ExceptionFilter mapException)
        {
            return Do(type, function.ToResultFunc(mapException));
        }
        
        /// <summary>
        /// If holding a Success, Executes the function with the result as input.
        /// Returns the result of the computation.
        /// </summary>
        /// <param name="function">The function to execute</param>
        /// <typeparam name="TResult">The type of the result of the computation</typeparam>
        /// <returns>Either a Success from the computation, or a Failure</returns>
        public Result<TResult> Then<TResult>(Func<TSucc?, Result<TResult>> function)
        {
            return Match(
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
        public Result<TResult> Then<TResult>(Func<TSucc?, TResult> function, ExceptionFilter mapException)
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
        public Result<Unit> Then(Action<TSucc?> function, ExceptionFilter mapException)
        {
            return Then(function.ToResultFunc(mapException));
        }

        /// <summary>
        /// If holding a Success, checks if the result fulfils an assertion. If not, returns an AssertionException as error
        /// </summary>
        /// <param name="assertion">The function to execute</param>
        /// <param name="assertionMessage">The message in the AssertionException when the assertion fails</param>
        /// <returns>Either the Success, or a Failure</returns>
        public Result<TSucc> Assert(Func<TSucc?,Result<bool>> assertion, string? assertionMessage = null)
        {
            var curr = this;
            return Then(assertion)
                .Then(assertionResult => assertionResult ? 
                    curr : 
                    (Result<TSucc>)new AssertionException(assertionMessage??"Assertion returned false!"));
        }
        
        /// <summary>
        /// If holding a Success, checks if the result fulfils an assertion. If not, returns an AssertionException as error
        /// If any exception is thrown, it is mapped by the mapper function.
        /// </summary>
        /// <param name="assertion">The assertion to execute</param>
        /// <param name="mapException">The mapping function for the error</param>
        /// <param name="assertionMessage">The message in the AssertionException when the assertion fails</param>
        /// <returns>Either the Success, or a Failure</returns>
        public Result<TSucc> Assert(Func<TSucc?,bool> assertion, ExceptionFilter mapException, string? assertionMessage = null)
        {
            return Assert(assertion.ToResultFunc(mapException), assertionMessage);
        }

        /// <summary>
        /// If holding a Success, checks if the result fulfils a predicate. If yes execute Then, otherwise execute Else
        /// Both Then and Else should return the same type.
        /// </summary>
        /// <param name="predicate">The predicate to check</param>
        /// <param name="Then">The function to execute if predicate returns True</param>
        /// <param name="Else">The function to execute if predicate returns False</param>
        /// <returns>Either the Success, or a Failure</returns>
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public Result<TResult> If<TResult>(Func<TSucc?,Result<bool>> predicate, Func<TSucc?, Result<TResult>> Then, Func<TSucc?, Result<TResult>> Else)
        {
            return this.Then(s => predicate(s).IsSuccess() ? Then(s) : Else(s));
        }

        /// <summary>
        /// If Result is a Failure, passes the Failure through a mapper function.
        /// </summary>
        /// <param name="mapper">Maps Failures to other Failures</param>
        /// <returns>Mapped Failure if Result is Failure</returns>
        public Result<TSucc> MapFailure(Func<Exception?, Exception?> mapper) 
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
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public T Match<T>(Func<TSucc?, T> Success, Func<Exception?, T> Failure)
        {
            return _isSuccess ? Success(_value) : Failure(_exception);
        }

        /// <summary>
        /// Define continuations on the success and failure cases.
        /// If Result is Success, executes Success
        /// Else, executes Failure
        /// </summary>
        /// <param name="Success">Function to execute on Success</param>
        /// <param name="Failure">Function to execute on Failure</param>
        /// <returns>Returns Unit, denoting null return (void function)</returns>
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public void Match(Action<TSucc?> Success, Action<Exception?> Failure)
        {
            if (_isSuccess)
            {
                Success(_value);
            }
            else
            {
                Failure(_exception);
            }
        }
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
            return obj;
        }

        /// <summary>
        /// Converts a non result single argument function to a result function by wrapping it in a try catch,
        /// and mapping the exceptions according to the ExceptionFilter, rethrowing unmapped exceptions
        /// </summary>
        /// <param name="func">Function to convert</param>
        /// <param name="mapException">Predicate determining which exceptions to map</param>
        /// <typeparam name="TInput">The input type to the function</typeparam>
        /// <typeparam name="TSucc">The output type of the function</typeparam>
        /// <returns></returns>
        public static Func<TInput?, Result<TSucc>> ToResultFunc<TInput, TSucc>(this Func<TInput?, TSucc> func, ExceptionFilter mapException)
        {
            return (x =>
            {
                try
                {
                    return func(x);
                }
                catch(Exception e)
                {
                    if (!mapException(e)) throw;
                    return e;
                }
            });
        }

        /// <summary>
        /// Converts a non result single argument void function into a new function that outputs a Result.
        /// </summary>
        /// <param name="func">Function to execute</param>
        /// <param name="mapException">Exceptions to catch and map to Failure</param>
        /// <typeparam name="TInput">Input type</typeparam>
        /// <returns>Function that outputs a Success of Unit if successful, converting relevant Exceptions into Failure</returns>
        public static Func<TInput?, Result<Unit>> ToResultFunc<TInput>(this Action<TInput?> func, ExceptionFilter mapException)
        {
            return ToResultFunc(func.Unit(), mapException);
        }

        /// <summary>
        /// Checks if value contained in Result is not null
        /// </summary>
        /// <param name="result">Result to check</param>
        /// <typeparam name="TSucc">Type of the value contained</typeparam>
        /// <returns>If value in Result is null, returns a NulLReferenceException as Failure</returns>
        public static Result<TSucc> AssertNotNull<TSucc>(this Result<TSucc> result)
        {
            return result.Assert(x => x is not null, "Expected Result to not be null.");
        }
    }
}