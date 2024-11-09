using Grpc.Core;

namespace TaskTracker.Gateway.Helpers
{
    public static class GrpcStatusCodeMapper
    {
        public static int MapGrpcStatusCodeToHttp(StatusCode statusCode)
        {
            return statusCode switch
            {
                StatusCode.OK => 200,
                StatusCode.Cancelled => 499,
                StatusCode.Unknown => 500,
                StatusCode.InvalidArgument => 400,
                StatusCode.DeadlineExceeded => 504,
                StatusCode.NotFound => 404,
                StatusCode.AlreadyExists => 409,
                StatusCode.PermissionDenied => 403,
                StatusCode.ResourceExhausted => 429,
                StatusCode.FailedPrecondition => 412,
                StatusCode.Aborted => 409,
                StatusCode.OutOfRange => 400,
                StatusCode.Unimplemented => 501,
                StatusCode.Internal => 500,
                StatusCode.Unavailable => 503,
                StatusCode.DataLoss => 500,
                StatusCode.Unauthenticated => 401,
                _ => 500
            };
        }
    }
}