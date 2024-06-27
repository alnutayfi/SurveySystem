namespace SurveySystem.Utils;

public class SurveyException : Exception
{
    public readonly FailureReason FailureReason;
    public string ClientMessage;
    public string InnerExceptionMessage;


    public SurveyException(FailureReason failureReason, Exception e = null) : base(failureReason.ToString())
    {
        FailureReason = failureReason;
        ClientMessage = failureReason.GetMessage();

        if (e is not null)
        {
            InnerExceptionMessage = e.Message;
            Helper.ExceptionLogger(FailureReason.GetError(), FailureReason.GetMessage(), e.StackTrace, e.Message);
        }
        else
            Helper.ExceptionLogger(FailureReason.GetError(), FailureReason.GetMessage(), StackTrace);
    }
    
    

    public SurveyException(FailureReason failureReason, string clientMessage, Exception e = null) : base(failureReason.ToString())
    {
        FailureReason = failureReason;
        ClientMessage = clientMessage;
        if (e is not null)
        {
            Helper.ExceptionLogger(FailureReason.GetError(), FailureReason.GetMessage(), e.StackTrace, e.Message);
            InnerExceptionMessage = e.Message;
        }
        else
            Helper.ExceptionLogger(FailureReason.GetError(), FailureReason.GetMessage(), StackTrace);
    }
}