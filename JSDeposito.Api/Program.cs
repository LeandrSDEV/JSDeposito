using JSDeposito.Api.Data;
using JSDeposito.Core.Configurations;
using JSDeposito.Core.Interfaces;
using JSDeposito.Core.Services;
using JSDeposito.Core.ValueObjects;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region Controllers & Swagger

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JSDeposito.Api",
        Version = "v1"
    });

    // JWT (Usuários)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Digite: Bearer {seu token JWT}"
    });

    // Webhook PIX
    c.AddSecurityDefinition("WebhookToken", new OpenApiSecurityScheme
    {
        Name = "X-Webhook-Token",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "Token secreto do Webhook PIX"
    });

    // JWT global
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

#endregion

#region Database

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("Default"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("Default"))
    );
});

#endregion

#region Configuration Bindings

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));

builder.Services.Configure<DepositoConfig>(
    builder.Configuration.GetSection("Deposito"));

builder.Services.Configure<PixWebhookConfig>(
    builder.Configuration.GetSection("PixWebhook"));

#endregion

#region Authentication

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwt = builder.Configuration
            .GetSection("Jwt")
            .Get<JwtSettings>();

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwt.Emissor,
            ValidAudience = jwt.Audiencia,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt.Secret)
            )
        };
    });

#endregion

#region Repositories

builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<ICupomRepository, CupomRepository>();
builder.Services.AddScoped<IPagamentoRepository, PagamentoRepository>();
builder.Services.AddScoped<IEnderecoRepository, EnderecoRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IPromocaoFreteRepository, PromocaoFreteRepository>();

#endregion

#region Services

builder.Services.AddScoped<ProdutoService>();
builder.Services.AddScoped<PedidoService>();
builder.Services.AddScoped<PagamentoService>();
builder.Services.AddScoped<EnderecoService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CheckoutService>();
builder.Services.AddScoped<PixService>();

builder.Services.AddScoped<FreteService>(sp =>
{
    var promocaoRepo = sp.GetRequiredService<IPromocaoFreteRepository>();
    var depositoConfig = sp.GetRequiredService<IOptions<DepositoConfig>>().Value;

    var origem = new Localizacao(
        depositoConfig.Latitude,
        depositoConfig.Longitude
    );

    return new FreteService(origem, promocaoRepo);
});

#endregion

#region HttpClients

builder.Services.AddHttpClient<IGeocodingService, NominatimGeocodingService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(8);
});

#endregion

var app = builder.Build();

#region Database Seed

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DatabaseSeeder.Seed(context);
}

#endregion

#region Middleware Pipeline

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(app =>
{
    app.Run(async context =>
    {
        var error = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        if (error is SecurityException)
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
    });
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

#endregion

app.Run();
