namespace JSDeposito.Api.Configurations
{
    public class JwtSettings
    {
        public string Secret { get; set; } = string.Empty;
        public int ExpiracaoMinutos { get; set; }
        public int RefreshDias { get; set; }
        public string Emissor { get; set; } = string.Empty;
        public string Audiencia { get; set; } = string.Empty;
    }
}
