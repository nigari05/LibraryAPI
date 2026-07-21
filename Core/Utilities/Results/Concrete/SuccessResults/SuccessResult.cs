using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Core.Utilities.Results.Concrete.SuccessResults
{
    internal class SuccessResult : Result
    {
        public SuccessResult(HttpStatusCode statusCode) : base(statusCode, true)
        {
            
        }

        public SuccessResult(HttpStatusCode statusCode, string message) : base(statusCode,true, message)
        {
            
        }
    }
}
