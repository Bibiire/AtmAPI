namespace Application.ConfigSettings
{
    public class JwtSettings
    {
        public string Site { get; set; }
        public string Audience { get; set; }
        public double ExpirationTime { get; set; }
        public string Secret { get; set; }
    }
}
