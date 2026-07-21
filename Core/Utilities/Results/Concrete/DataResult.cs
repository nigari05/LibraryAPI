using Core.Utilities.Results.Abstract;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Core.Utilities.Results.Concrete
{
    public class DataResult<T> : Result, IDataResult<T>
    {
        public T? Data { get; }

        public DataResult(HttpStatusCode statusCode, bool success, string? message, T? data) : base(statusCode, success, message)
        {
            Data = data;
        }

        public DataResult(HttpStatusCode statusCode, bool success, T? data) : base(statusCode, success)
        {
            Data = data;
        }
    }
}
