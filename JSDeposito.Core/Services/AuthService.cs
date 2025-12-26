using JSDeposito.Core.Configurations;
using JSDeposito.Core.DTOs;
using JSDeposito.Core.Entities;
using JSDeposito.Core.Interfaces;
using JSDeposito.Core.ValueObjects;
using Microsoft.Extensions.Logging;
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
    private readonly PedidoService _pedidoService;
    private readonly JwtSettings _jwt;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IRefreshTokenRepository refreshTokenRepository,
        PedidoService pedidoService,
        IOptions<JwtSettings> jwt,
        ILogger<AuthService> logger)
    {
        _usuarioRepository = usuarioRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _pedidoService = pedidoService;
        _jwt = jwt.Value;
        _logger = logger;
    }

    public AuthResponse Login(
    string email,
    string senha,
    Guid? tokenAnonimoPedido)
    {
        _logger.LogInformation(
            "Tentativa de login | Email: {Email}",
            email);

        var usuario = _usuarioRepository.ObterPorEmail(email)
            ?? throw new Exception("Credenciais inválidas");

        if (!usuario.ValidarSenha(senha))
        {
            _logger.LogWarning(
                "Falha de login (senha inválida) | Email: {Email}",
                email);

            throw new Exception("Credenciais inválidas");
        }

        _logger.LogInformation(
            "Login realizado com sucesso | UsuarioId: {UsuarioId}",
            usuario.Id);

        // 🔥 associação automática do pedido anônimo
        if (tokenAnonimoPedido.HasValue)
        {
            _logger.LogInformation(
                "Associando pedido anônimo ao usuário | UsuarioId: {UsuarioId} | TokenAnonimo: {Token}",
                usuario.Id,
                tokenAnonimoPedido.Value);

            _pedidoService.AssociarPedidoAnonimoAoUsuario(
                tokenAnonimoPedido.Value,
                usuario.Id
            );
        }

        var accessToken = GerarAccessToken(usuario);
        var refreshToken = GerarRefreshToken(usuario.Id);

        _logger.LogInformation(
            "Tokens gerados | UsuarioId: {UsuarioId}",
            usuario.Id);

        return new AuthResponse(accessToken, refreshToken);
    }

    public AuthResponse Refresh(string refreshToken)
    {
        _logger.LogInformation("Tentativa de refresh token");

        var token = _refreshTokenRepository.Obter(refreshToken);

        if (token == null || !token.EstaValido())
        {
            _logger.LogWarning("Refresh token inválido ou expirado");
            throw new SecurityException("Refresh token inválido");
        }

        var usuario = _usuarioRepository.ObterPorId(token.UsuarioId)
            ?? throw new Exception("Usuário não encontrado");

        _logger.LogInformation(
            "Refresh token válido | UsuarioId: {UsuarioId}",
            usuario.Id);

        token.Revogar();
        _refreshTokenRepository.Revogar(refreshToken);

        var novoAccessToken = GerarAccessToken(usuario);
        var novoRefreshToken = GerarRefreshToken(usuario.Id);

        _logger.LogInformation(
            "Novos tokens emitidos | UsuarioId: {UsuarioId}",
            usuario.Id);

        return new AuthResponse(novoAccessToken, novoRefreshToken);
    }

    public void Logout(string refreshToken)
    {
        _logger.LogInformation("Logout solicitado");

        _refreshTokenRepository.Revogar(refreshToken);

        _logger.LogInformation("Refresh token revogado com sucesso");
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

    public void Register(RegisterRequest request)
    {
        _logger.LogInformation(
            "Tentativa de registro | Email: {Email}",
            request.Email);

        var existe = _usuarioRepository.ObterPorEmail(request.Email);
        if (existe != null)
        {
            _logger.LogWarning(
                "Registro bloqueado — e-mail já existente | Email: {Email}",
                request.Email);

            throw new Exception("E-mail já cadastrado");
        }

        var senhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha);

        var usuario = new Usuario(
            request.Nome,
            request.Email,
            request.Telefone,
            senhaHash,
            role: "Cliente"
        );

        _usuarioRepository.Adicionar(usuario);

        _logger.LogInformation(
            "Usuário registrado com sucesso | UsuarioId: {UsuarioId}",
            usuario.Id);
    }
}
