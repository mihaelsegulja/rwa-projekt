namespace WebAPI.Config
{
    public static class JWTTokenConfig
    {
        public static string TokenSecret { get; set; }
        public static string TokenIssuer { get; set; }
        public static string TokenAudience { get; set; }
        public static int TokenExpiration { get; set; }
    }
}
