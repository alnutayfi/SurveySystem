using Serilog;

namespace SurveySystem.Utils;

public class Helper
{
    public static void ExceptionLogger(string errorCode, string errorMessage, string stackTrace, string innerErrorMessage = null)
    {
        if(errorCode.Contains('S')) 
            Log.Error("({ErrCode}) [{ErrMsg}] [exception stackTrace : {ExTrace}] [Inner exception message :{InnerExMsg}]", 
                errorCode, errorMessage, stackTrace, innerErrorMessage);
        else
            Log.Warning("({ErrCode}) [{ErrMsg}] [exception stackTrace : {ExTrace}] [Inner exception message :{InnerExMsg}]", 
                errorCode, errorMessage, stackTrace, innerErrorMessage);
    }
    
    
    public static async void LogActivity(Activity.Activity activity)
    {
        await File.AppendAllTextAsync("SurveySystem_activity" + DateTime.Now.ToString("dd-MM-yyyy") + ".log",
            $"[{activity.TraceId}] [{activity.Endpoint}] [{activity.Status}] [{activity.ReceivedAt}] [{activity.ResponseAt}]  \n[Request : \n{activity.RequestBody}] \n[Response : \n{activity.ResponseBody}]\n");
    }
}