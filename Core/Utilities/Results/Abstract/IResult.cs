using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Core.Utilities.Results.Abstract
{
    public interface IResult
    {
        HttpStatusCode StatusCode { get; }
        bool Success { get; }
        string? Message { get; }
    }
}
