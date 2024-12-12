namespace EnterpriseAI.Core.Configuration;

public class ApiSecurityConfig
{
    public const string DefaultHeaderName = "X-API-Key";
    public string HeaderName { get; set; } = DefaultHeaderName;
    public required string[] ApiKeys { get; set; }
}
