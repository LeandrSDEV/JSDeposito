using JSDeposito.Core.Configurations;
using JSDeposito.Core.Entities;
using JSDeposito.Core.Interfaces;
using JSDeposito.Core.ValueObjects;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JSDeposito.Core.Services;

public class AuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly JwtSettings _jwt;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IOptions<JwtSettings> jwt)
    {
        _usuarioRepository = usuarioRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwt = jwt.Value;
    }

    public AuthResponse Login(string email, string senha)
    {
        var usuario = _usuarioRepository.ObterPorEmail(email)
            ?? throw new Exception("Credenciais inválidas");

        if (!BCrypt.Net.BCrypt.Verify(senha, usuario.SenhaHash))
            throw new Exception("Credenciais inválidas");

        var accessToken = GerarAccessToken(usuario);
        var refreshToken = GerarRefreshToken(usuario.Id);

        return new AuthResponse(accessToken, refreshToken);
    }

    public AuthResponse Refresh(string refreshToken)
    {
        var token = _refreshTokenRepository.Obter(refreshToken);

        if (token == null || !token.EstaValido())
            throw new SecurityException("Refresh token inválido");

        var usuario = _usuarioRepository.ObterPorId(token.UsuarioId)
            ?? throw new Exception("Usuário não encontrado");

        token.Revogar();
        _refreshTokenRepository.Revogar(refreshToken);

        var novoAccessToken = GerarAccessToken(usuario);
        var novoRefreshToken = GerarRefreshToken(usuario.Id);

        return new AuthResponse(novoAccessToken, novoRefreshToken);
    }

    public void Logout(string refreshToken)
    {
        _refreshTokenRepository.Revogar(refreshToken);
    }

    private string GerarAccessToken(Usuario usuario)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Role, usuario.Role),
            new Claim(ClaimTypes.Email, usuario.Email)
        };

        if (string.IsNullOrEmpty(_jwt.Secret))
            throw new Exception("JWT Secret não configurado");

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwt.Secret));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwt.Emissor,
            audience: _jwt.Audiencia,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.ExpiracaoMinutos),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GerarRefreshToken(int usuarioId)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        var refreshToken = new RefreshToken(
            usuarioId,
            token,
            DateTime.UtcNow.AddDays(_jwt.RefreshDias)
        );

        _refreshTokenRepository.Salvar(refreshToken);

        return token;
    }
}
