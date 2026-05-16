namespace AuthMicroService.Common.Settings;

public static class Settings
{
    public static string? JwtSecreteKey { set; get; }
    public static string? JwtIssuer { set; get; }
    public static string? JwtAudience { set; get; }
}

public static class GoogleSetting
{
    public static string? ClientId { set; get; }
}