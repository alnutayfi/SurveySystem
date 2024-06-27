using System;

namespace Activity;
public class Activity
{
    public bool IsAuthorizedActivity { get; set; }
    public Guid TraceId { get; set; }
    public DateTime ReceivedAt { get; set; }
    public DateTime ResponseAt { get; set; }
    public string Endpoint { get; set; }
    public string RequestBody { get; set; }
    public string ResponseBody { get; set; }
    public int Status { get; set; }
    public ActivityExceptionData ActivityExceptionData { get; set; }
}


public class ActivityExceptionData
{
    public Guid TraceId { get; set; }
    public DateTime OccurAt { get; set; }
    public string ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
    public string StackTrace { get; set; }
    public string InnerExceptionMessage { get; set; }


    public ActivityExceptionData(DateTime occurAt, Guid traceId, string errorCode, string errorMessage, string stackTrace, string innerExceptionMessage)
    {
        OccurAt = occurAt;
        TraceId = traceId;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
        StackTrace = stackTrace;
        InnerExceptionMessage = innerExceptionMessage;
    }
}