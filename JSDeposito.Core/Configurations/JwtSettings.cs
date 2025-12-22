namespace JSDeposito.Core.Configurations
{
    public class JwtSettings
    {
        public string Secret { get; set; }
        public int ExpiracaoMinutos { get; set; }
        public int RefreshDias { get; set; }
        public string Emissor { get; set; }
        public string Audiencia { get; set; }
    }
}
