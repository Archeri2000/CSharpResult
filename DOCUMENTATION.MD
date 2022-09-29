# Documentation

### NOTE: The documentation is slightly outdated as new utility methods have been added. New documentation will be coming soon!
## Overview
This library uses 5 method keywords to describe possible computation types. 
Each of these methods can be further modified by adding **Await** to add Async Effects, and **Each** to operate over a collection

| Keyword  | Description  |
|---|---|
| Then | Executes a function and returns the output.|
| Do | Executes a function and if output is Success, ignore it and return the input. Do takes in an Enum which specifies whether to track if the function successfully runs or to ignore it. | 
| Assert | Executes an assertion and either returns the original result or an assertion exception. |
| If | Executes a predicate and if output is True, run and return the 'then' expression otherwise run and return the 'else' expression|
| Match | If value is Success, execute first function, else execute second function |

| Modifiers | Description |
|---|---|
| Await | Executes an async function and await the result. |
| Each | Executes a function on every member of a collection. |

For example:
```c#
// Normal computation
result.Then(...)
    .Assert(...)
    .Then(...)
    .Do(...);

// Async computations mixed with normal computations
result.ThenAwait(...)
    .Assert(...)
    .ThenAwait(...)
    .IfAwait(...)
    .DoAwait(...);

// Collection computations
results.ThenEach(...)
    .AssertEach(...)
    .ThenEach(...)
    .DoEach(...);

// Async Result Collections mixed with normal computations
results.ThenAwaitEach(...)
    .AssertEach(...)
    .ThenAwaitEach(...)
    .AssertAwaitEach(...)
    .DoAwaiteach(...);
```
## Result Type
The result type allows for functions to return either a successful result or a failure exception. 
This allows developers to provide context documenting intended errors that could occur, instead of returning null or throwing an exception.

The result type can either hold a value or an Exception representing an intended Failure state. 
Using implicit operators, it is possible to simply return the intended Exception or value.
```c#
// Without Result using null return
public int GetValue(string key){
    if(!map.ContainsKey(key)){
        return null;
    }
    return map[key];
}

// Without Result using exceptions
public int GetValue(string key){
    if(!map.ContainsKey(key)){
        throw new KeyNotFoundException($"Key {key} was not found in map!");
    }
    return map[key];
}

// With Result
public Result<int> GetValue(string key){
    if(!map.ContainsKey(key)){
        return new KeyNotFoundException($"Key {key} was not found in map!");
    }
    return map[key];
}
```
By returning an exception, additional context is given on the type of failure state, thus allowing downstream operations to respond accordingly.

Existing functions and libraries can be wrapped to introduce compatibility with the Result type manually as follows:
```c#
// Without Result
public int GetValue(string key){
    if(!map.ContainsKey(key)){
        throw new KeyNotFoundException($"Key {key} was not found in map!");
    }
    return map[key];
}

// Wrap Exception using Try Catch
public Result<int> GetValueWrapped(string key){
    try{
        return GetValue(key);
    }catch(Exception e){
        return e;
    }
}
```

Two static functions are included to cover the most common cases of exception mapping:
```c#
        public static class Errors{
            // Map no exceptions, thus throwing any caught exception
            public static bool MapNone(Exception e)
            {
                return false;
            }
    
            // Map all exceptions, thus returning any caught exception
            public static Exception MapAll(Exception e)
            {
                return true;
            }
        }
```
Fine grained control can be achieved by implementing your own exception mapping function:
```c#
    public static bool CatchNullRefAndNotFound(Exception e){
        if(e is KeyNotFoundException || e is NullReferenceException){
            return true;
        }
        return false;
    }
```

Alternatively you can describe the exception mapper in fluent syntax:
```c#
    var mapper = MapIfExceptionIs<KeyNotFoundException>().Or<NullReferenceException>();
```

### Converting to result type
Any type can be converted to a Result by using the extension method:
```c#
//int
int x = 3;
//Result<int>
var resultX = x.ToResult();
```
Any single argument function returning a value can be converted to return a Result of that value by using the extension method and specifying how to handle thrown exceptions:
```c#
//Function returning an int
Func<int,int> add1 = (x => x+1);
//Function returning a Result<int>
var resultFunc = add1.ToResultFunc(Errors.MapNone);
```
Any void function can be converted to return a result of that value by using the extension method and specifying how to handle thrown exceptions:
 ```c#
Action<int> printInt = (x => System.WriteLine(x));
var resultFunc = printInt.ToResultFunc(Errors.MapNone);
```

### Usage
The result type is best used to construct pipelines of single variable functions to describe workflows.

Three different methods are provided for chaining the flow of functions.
```c#
// Then is used to perform computation and return the result
var add1 = ((Func<int,int>)(x => x+1)).ToResultFunc(Errors.MapNone);
var thenRes = 1.ToResult()
            .Then(add1)
            .Then(add1);
Console.WriteLine(thenRes.SuccessOrDefault()); // Should print out 3 by unwrapping the value in the Result

// Do is used to perform computation then discard the result, returning the input
var printInt = ((Action<int>)(x => System.WriteLine(x))).ToResultFunc(Errors.MapNone);
var doRes = 1.ToResult()
            .Do(DoType.Ignore, printInt); // Should print out 1
Console.WriteLine(doRes.SuccessOrDefault()); // Should print out 1 by unwrapping the value

// Assert is used to check a boolean predicate. If the result returned is false, it returns a Failure.
var isEven = ((Func<int,bool>)(x => x%2==0)).ToResultFunc(Errors.MapNone);
var result = 1.ToResult()
                .Assert(isEven) //Will return a failure here due to predicate check
                .Then(add1);



// Combining them to construct workflows
var result = 1.ToResult()
                .Do(DoType.Ignore, printInt) // Prints 1
                .Then(add1) // Adds 1 to 1
                .Do(DoType.Ignore, printInt) // Prints 2
                .Then(add1); //Adds 1 to 2
Console.WriteLine(result.SuccessOrDefault()); //Prints 3
```

If at any step the result returned is a Failure, the remaining steps will not be run. This allows for the construction of simple to understand workflows that can describe the flow of data through a program clearly.
```c#
// Example
var result = resourceModel.ToResult()
                    .Then(ConvertToDomain)
                    .Assert(model => Validate(model, arg2, arg3))
                    .Do(model => db.Save(model))
                    .Then(ConvertToResponse);
```

Processing the final result can be done in several ways. It is possible to check the state of the result and unwrap it:
```c#
var result = resource.ToResult()...;

//Check success
if(result.IsSuccess()){
    //Unwrap
    var value = result.SuccessOrDefault();
    ...
}

//Check success with out variable
if(result.IsSuccess(out var succ)){
    //use succ here
}

//Check failure
if(result.IsFailure()){
    var error = result.FailureOrDefault();
    ...
}

//Check failure with out variable
if(result.IsFailure(out var err)){
    //use err here and handle it
}
```

Alternatively, a more idiomatic way to handle the result is to use continuations in both the success and failure case.
Both paths must return the same type of value.
```c#
var result = resource.ToResult()...;

return result.Match(
    Success: value => value.ToString(),
    Failure: err => ""
);
```

### Query Expression Syntax
The Result type supports Query Expressions due to implementing the Select and SelectMany extension methods.

This enables writing the pipeline in expression form:
```c#
var result =
        from identifier in GetId(id)
        from key in GetKey(identifier)
        select key,identifier
```
## Results and Collections
When dealing with results, it may be desirable to process a collection of values at once. 
Support for collections processing is included in the form of extension methods. 
Methods are identical to normal result processing, with an added **Each** to denote operating on a collection.
```c#
var ls = new List<int>();
...
var results = ls.ToResultCollection()
                .DoEach(...)
                .AssertEach(...)
                .ThenEach(...)
                .MatchEach(...);
```

Methods are also included for querying the state of an IEnumerable of results:
```c#
var results = ls.ToResultCollection();

//Test success
results.AllSucceed();
results.AnySucceed();

//Test failure
results.AllFail();
results.AnyFail();

//Get Successes
results.GetSuccesses();

//Get Failures
results.GetFailures();
```

### Query Expressions
Query Expression support is not extended to Result Collections as it would conflict with existing LINQ methods and cause confusion.

## Async Results
This library also provides support for Async operations on results in the form of extension methods. 
Async Results are Results wrapped in a Task to denote an asynchronous computation.

Constructing Async Results and functions follows a similar convention to the Result type.
```c#
// Creating Async Results
var x = 1;
var resultX = 1.ToAsyncResult();

// Wrapping Async Functions
Func<int,Task<int>> add1 = (async x => x + 1);
var resultAdd1 = add1.ToAsyncResultFunc(Errors.MapNone);

// Wrapping Async Void Function
Func<int, Task> printX = (async x => Console.WriteLine(x));
var resultPrintX = printX.ToAsyncResultFunc(Errors.MapNone);
```

Methods are identical to normal result processing, with an added **Await** to denote executing async operations. 
This can be used in combination with Then, Do and If to incorporate non async operations.
```c#
var result = await model.ToAsyncResult()
                        .ThenAwait(...)
                        .Assert(...)
                        .DoAwait(...)
                        .Then(...)
                        .ThenAwait(...);
```

### Query Expressions
Query Expression support is included for async computations. 
Similar to normal Result functions, the **from** and **select** keywords can be used.
```c#
var result = 
            from val in db.LoadAsync(key)
            from user in users.GetAsync(key)
            select new UserObj(user, val)
```

## Async Results and Collections
Support also exists for managing a collection of Async Results.

A collection of async results can be created as follows:
```c#
// Create from collection
var ints = new List<int>();
var results = ints.ToAsyncResultCollection();

// Create from Result collection
var res = new List<Result<int>>();
var results = res.ToAsyncResultCollection();
```
The workflow when managing a collection of async results is to use the chaining functions **Then**, **Do** and **If** to describe the async workflow, 
then call one of the functions (e.g. **AwaitAll**, **AwaitAny**) to begin the async execution.

Similar to managing collections of results, the **Each** keyword is used to signify operating on a collection.
When an async operation needs to be executed, the **Await** keyword is used.
```c#
var results = models.ToAsyncResultCollection()
                    .ThenEach(...)
                    .AssertAwaitEach(...)
                    .ThenAwaitEach(...)
                    .DoEach(...);
```

### Querying collection state and IAsyncEnumerable support
Similar to the Result collection, it is possible to query the state of the collection.
```c#
var results = models.ToAsyncResultCollection()...;

results.AnySucceed();
results.AnyFail();
results.AllSucceed();
results.AllFail();
```

Support also exists for processing these Async Results.
```c#
var results = models.ToAsyncResultCollection()...;

// await all the Async Results to return
await results.AwaitAll();
// await any Async Result to return
await results.AwaitAny();
```

It is also possible to turn the collection into an IAsyncEnumerable for processing.
```c#
var results = models.ToAsyncResultCollection()...;

// get only successes as IAsyncEnumerable
var successes = results.GetSuccesses();
// get only failures as IAsyncEnumerable
var failures = results.GetFailures();
// get all results as IAsyncEnumerable
var res = results.ToIAsyncEnumerable();
```

### Query Expressions
For the same reasons as Result Collections, Query Expression support is not extended to Async Result Collections.