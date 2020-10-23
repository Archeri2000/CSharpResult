using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSharp_Result
{
    public static class AsyncResultCollectionExtensions
    {
        public static async Task<bool> AllSucceed<T>(this Task<IEnumerable<Result<T>>> list) 
            where T : notnull
        {
            return (await list).AllSucceed();
        }

        public static async Task<bool> AnySucceed<T>(this Task<IEnumerable<Result<T>>> list) 
            where T : notnull
        {
            return (await list).AnySucceed();
        }

        public static async Task<bool> AllFail<T>(this Task<IEnumerable<Result<T>>> list) 
            where T : notnull
        {
            return (await list).AllFail();
        }

        public static async Task<bool> AnyFail<T>(this Task<IEnumerable<Result<T>>> list) 
            where T : notnull
        {
            return (await list).AnyFail();
        }
        public static async Task<IEnumerable<Exception>> GetFailures<T>(this Task<IEnumerable<Result<T>>> list) 
            where T : notnull
        {
            return (await list).GetFailures();
        }

        public static async Task<IEnumerable<T>> GetSuccesses<T>(this Task<IEnumerable<Result<T>>> list) 
            where T : notnull
        {
            return (await list).GetSuccesses();
        }

        public static async Task<Result<IEnumerable<T>>> ToResultOfSeq<T>(this Task<IEnumerable<Result<T>>> list) 
            where T : notnull
        {
            return (await list).ToResultOfSeq();
        }

        public static async Task<IEnumerable<Result<T>>> ToSeqOfResults<T>(this Task<Result<IEnumerable<T>>> list) 
            where T : notnull
        {
            return (await list).ToSeqOfResults();
        }
    }
}