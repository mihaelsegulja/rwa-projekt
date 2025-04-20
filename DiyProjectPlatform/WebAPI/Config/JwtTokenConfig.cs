namespace WebAPI.Config;

public static class JwtTokenConfig
{
    public static string TokenSecret { get; set; }
    public static string TokenIssuer { get; set; }
    public static string TokenAudience { get; set; }
    public static int TokenExpiration { get; set; }
}
