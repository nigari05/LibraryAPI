using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Core.Utilities.Results.Concrete.SuccessResults
{
    public class SuccessDataResult<T> : DataResult<T>
    {
        public SuccessDataResult(HttpStatusCode statusCode, T data) : base(statusCode, true, data)
        {

        }
        public SuccessDataResult(HttpStatusCode statusCode, string message, T data) : base(statusCode, true, message, data)
        {

        }
        public SuccessDataResult(HttpStatusCode statusCode) : base(statusCode, true, default)
        {

        }
        public SuccessDataResult(HttpStatusCode statusCode, string message) : base(statusCode, true, message, default)
        {

        }
    }
}
