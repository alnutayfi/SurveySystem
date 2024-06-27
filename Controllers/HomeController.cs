using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SurveySystem.Models;
using SurveySystem.Utils;

namespace SurveySystem.Controllers;

[ApiController]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly string _smtpServer = "smtp.gmail.com";
    private readonly int _smtpPort = 587; 
    private readonly string _smtpUser = "surveydomain999@gmail.com";
    private readonly string _smtpPass = "iygektwuuxackumy";
    string urlPattern = @"^(https?|ftp|file):\/\/[-A-Za-z0-9+&@#\/%?=~_|!:,.;]*[-A-Za-z0-9+&@#\/%=~_|]$";
    string emailPattern = @"^(https?|ftp|file):\/\/[-A-Za-z0-9+&@#\/%?=~_|!:,.;]*[-A-Za-z0-9+&@#\/%=~_|]$";
    

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }
    
    [HttpGet("Index")]
    public IActionResult Index() => View();
    
    [HttpGet("create-survey")]
    public IActionResult CreateSurvey() => View();
    
    
    [HttpPost("create-survey")]
    public async Task<ActionResult> CreateSurvey([FromBody] SurveyRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.SurveyUrl) || request.Domains == null)
        {
            throw new SurveyException(FailureReason.InvalidRequest);
        }
        
        Regex regex = new Regex(urlPattern, RegexOptions.IgnoreCase);
        bool isUrlValid = regex.IsMatch(request.SurveyUrl);

        if (!isUrlValid)
            throw new SurveyException(FailureReason.InvalidSurveyUrl);
        
        string body = $"Please complete the following survey: {request.SurveyUrl}";
        
        foreach (var domain in request.Domains)
        {
            try
            {
                var regex2 = new Regex(urlPattern, RegexOptions.IgnoreCase);
                bool isEmailValid = regex2.IsMatch(domain.AdminEmail);

                if (!isEmailValid)
                    throw new SurveyException(FailureReason.InvalidEmailAddress);
                
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpUser),
                    Subject = "Domain Survey",
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(domain.AdminEmail);

                using (var smtpClient = new SmtpClient(_smtpServer, _smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                    smtpClient.EnableSsl = true;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.UseDefaultCredentials = false;

                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
            catch (Exception e)
            {
                Log.Error($"Fails to sending survey url to domain '{domain.DomainName}' admin email '{domain.AdminEmail}', Error message {e.Message}");
            }
        }

        var response = new { Result = "success" };
        return Ok(response);
    }
}