using System.Reflection;
using SurveySystem.BLL.Utils;

namespace SurveySystem.Utils;

public enum FailureReason
{
    [Error( "S001", "General error, please refer to administrator with the traceId")]
    GeneralError,
    [Error( "S002", "Service unavailable, please re-try again")]
    ServiceUnavailable,
    
    [Error( "A001", "Invalid URL")]
    InvalidUrl,

    
    [Error( "V001", "Invalid request")]
    InvalidRequest,
    [Error( "V002", "Invalid property")]
    InvalidProperty,
    [Error( "V003", "Invalid request body format")]
    InvalidRequestBodyFormat,
    [Error( "V004", "Invalid Survey Url")]
    InvalidSurveyUrl,
    [Error( "V005", "Invalid Email Address")]
    InvalidEmailAddress,


    
}


public static class FailureReasonExtension
{
    private static ErrorAttribute GetErrorAttribute(FailureReason failureReason) => 
        (ErrorAttribute)failureReason.GetType().GetField(failureReason.ToString())?.GetCustomAttribute(typeof(ErrorAttribute), false);
    public static string? GetError(this FailureReason failureReason) => GetErrorAttribute(failureReason)?.Error;
    public static string? GetMessage(this FailureReason failureReason) => GetErrorAttribute(failureReason)?.Message;
}