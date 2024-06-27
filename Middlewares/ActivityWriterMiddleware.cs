using System.Text;
using Serilog;
using SurveySystem.Utils;


namespace SurveySystem;

public class ActivityWriterMiddleware
{
    private readonly RequestDelegate _next;

    public ActivityWriterMiddleware(RequestDelegate next)
    {
        _next = next;
    }


    public async Task InvokeAsync(HttpContext context, Activity.Activity activity)
    {
        using (var ms = new MemoryStream())
        {
            await context.Request.Body.CopyToAsync(ms);
            activity.RequestBody = Encoding.Default.GetString(ms.ToArray());
            ms.Seek(0, SeekOrigin.Begin);
            context.Request.Body = ms;
            await _next(context);
        }

        activity.ResponseAt = DateTime.UtcNow.Date.ToLocalTime();

        Task.Run(async () => { Helper.LogActivity(activity); });
    }
}
