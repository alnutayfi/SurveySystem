using System.Net;
using System.Text.Json;
using SurveySystem.Utils;
using Serilog.Context;
using SurveySystem.Models;

namespace SurveySystem;


public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    

    public async Task Invoke(HttpContext httpContext, Activity.Activity activity)
    {
        try
        {
            activity.TraceId = Guid.NewGuid(); 
            activity.ReceivedAt = DateTime.UtcNow.Date.ToLocalTime();
            LogContext.PushProperty("TraceId", activity.TraceId.ToString());
            httpContext.Response.Headers.Add("X-TraceId", activity.TraceId.ToString());
            
            activity.Endpoint = httpContext.Request.Path.ToString().Split("/").LastOrDefault();
            LogContext.PushProperty("ServiceName", activity.Endpoint);
            
            await _next(httpContext);
        }
        catch (Exception e)
        {
            await HandleGlobalExceptionAsync(httpContext, e, activity);
            Task.Run(async () => Helper.LogActivity(activity));
        }
    }

    
    private async Task HandleGlobalExceptionAsync(HttpContext httpContext, Exception exception, Activity.Activity activity)
    {
        UnsuccessfulRequest unsuccessfulRequest;
        
        if (exception is SurveyException surveyException)
        {
            var isServerError = surveyException.FailureReason.GetError()!.Contains("S");

            if (isServerError)
            {
                unsuccessfulRequest = new UnsuccessfulRequest(
                    surveyException.FailureReason == FailureReason.ServiceUnavailable ? surveyException.FailureReason.GetError() : FailureReason.GeneralError.GetError(),
                    surveyException.FailureReason == FailureReason.ServiceUnavailable ? surveyException.FailureReason.GetMessage() : FailureReason.GeneralError.GetMessage());
                
                activity.ActivityExceptionData = new (DateTime.Now, activity.TraceId,
                    surveyException.FailureReason.GetError(),surveyException.FailureReason.GetMessage(),
                    exception.StackTrace, surveyException.InnerExceptionMessage);
            }
            else
            {
                unsuccessfulRequest = new UnsuccessfulRequest(surveyException.FailureReason.GetError(), surveyException.ClientMessage ?? surveyException.FailureReason.GetMessage());

                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await httpContext.Response.WriteAsync(JsonSerializer.Serialize(unsuccessfulRequest));
                (activity.ResponseBody, activity.Status) = (JsonSerializer.Serialize(unsuccessfulRequest),(int)HttpStatusCode.BadRequest);
            }
        }
        else
        {
            unsuccessfulRequest = new UnsuccessfulRequest(FailureReason.GeneralError.GetError(), FailureReason.GeneralError.GetMessage());

            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(unsuccessfulRequest));
            (activity.ResponseBody, activity.Status) = (JsonSerializer.Serialize(unsuccessfulRequest),(int)HttpStatusCode.InternalServerError);
            Helper.ExceptionLogger(FailureReason.GeneralError.GetError(), FailureReason.GeneralError.GetMessage(), exception.StackTrace, exception.Message);
        }
        
        activity.ResponseAt = DateTime.Now;
    }
}