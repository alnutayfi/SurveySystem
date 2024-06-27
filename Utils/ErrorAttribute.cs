namespace SurveySystem.BLL.Utils;

[AttributeUsage(AttributeTargets.All,  AllowMultiple = true)]
public sealed class ErrorAttribute : Attribute
{
    public string Error { get; set; }
    public string Message { get; set; }


    public ErrorAttribute(string error, string message)
    {
        Error = error;
        Message = message;
    }
} 