namespace Login_Register
{
    public class JWTSetting
    {
        public string SecretKey { get; set; }
        public int TokenValidityInMinutes { get; set; }
        public int RefreshTokenExpireDays { get; set; }
    }
}
