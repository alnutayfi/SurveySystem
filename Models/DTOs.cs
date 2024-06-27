using System.Text.Json.Serialization;

namespace SurveySystem.Models;


public record UnsuccessfulRequest(
    [property: JsonPropertyName("error")] string Error, 
    [property: JsonPropertyName("message")] string Message);


public record Domain(
    [property: JsonPropertyName("domainName")] string DomainName, 
    [property: JsonPropertyName("adminEmail")] string AdminEmail);


public record SurveyRequest(
    [property: JsonPropertyName("surveyUrl")] string SurveyUrl, 
    [property: JsonPropertyName("domains")] List<Domain> Domains);
    