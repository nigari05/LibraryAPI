using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Core.Utilities.Results.Concrete.ErrorResults
{
    public class ErrorDataResult<T> : DataResult<T>
    {
        public ErrorDataResult(HttpStatusCode statusCode, T data) : base(statusCode, false, data)
        {

        }
        public ErrorDataResult(HttpStatusCode statusCode, string message, T data) : base(statusCode, false, message, data)
        {

        }
        public ErrorDataResult(HttpStatusCode statusCode) : base(statusCode, false, default)
        {

        }
        public ErrorDataResult(HttpStatusCode statusCode, string message) : base(statusCode, false, message, default)
        {

        }
    }
}
