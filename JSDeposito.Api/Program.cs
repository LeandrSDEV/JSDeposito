using JSDeposito.Api.Data;
using JSDeposito.Core.Configurations;
using JSDeposito.Core.Interfaces;
using JSDeposito.Core.Services;
using Microsoft.EntityFrameworkCore;
using JSDeposito.Core.ValueObjects;
using Microsoft.Extensions.Options;
using JSDeposito.Core.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("Default"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("Default"))
    );
});

builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<ProdutoService>();
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<PedidoService>();
builder.Services.AddScoped<ICupomRepository, CupomRepository>();
builder.Services.AddScoped<IPagamentoRepository, PagamentoRepository>();
builder.Services.AddScoped<PagamentoService>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<IEnderecoRepository, EnderecoRepository>();
builder.Services.AddScoped<EnderecoService>();
builder.Services.AddScoped<CheckoutService>();

builder.Services.Configure<DepositoConfig>(
    builder.Configuration.GetSection("Deposito")
);

builder.Services.AddScoped<IPromocaoFreteRepository, PromocaoFreteRepository>();

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

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
