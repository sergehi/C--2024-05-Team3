using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Grpc.Core;

namespace Common.Rpc
{
    public class RpcExceptionEx : RpcException
    {
        public RpcExceptionEx(Status status) : base(status) 
        {
        }

        public RpcExceptionEx(Status status, Metadata trailers) : base(status, trailers) 
        { 
        }

        public RpcExceptionEx(Status status, string message) 
            : base(status, new Metadata() { { "message", WebUtility.UrlEncode(message) } })
        {
        }

        public RpcExceptionEx(Status status, string message, string stackTrace)
            : base(status, new Metadata() { { "message", WebUtility.UrlEncode(message) }, { "stack", WebUtility.UrlEncode(stackTrace) }  })
        {
        }

        public override string Message
        {
            get 
            {
                string? message = base.Trailers.GetValue("message");
                return message != null ? WebUtility.UrlDecode(message) : base.Message;
            }
        }
        public override string StackTrace
        {
            get
            {
                string? message = base.Trailers.GetValue("stack");
                return message != null ? WebUtility.UrlDecode(message) : "";
            }
        }
    }
}
