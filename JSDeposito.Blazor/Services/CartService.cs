using JSDeposito.Blazor.Models;

public class CartService
{
    private readonly HttpClient _http;

    public PedidoDto? Pedido { get; private set; }
    public int TotalItens => Pedido?.Itens.Sum(i => i.Quantidade) ?? 0;

    public event Action? OnChange;

    private int? _pedidoId;

    public CartService(HttpClient http)
    {
        _http = http;
    }

    // 🔹 Cria pedido se não existir
    private async Task<int> GarantirPedidoAsync()
    {
        if (_pedidoId.HasValue)
            return _pedidoId.Value;

        var response = await _http.PostAsJsonAsync("api/pedidos", new { itens = new List<object>() });
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<CriarPedidoResponseDto>();

        _pedidoId = result!.PedidoId;

        await CarregarPedidoAsync();

        return _pedidoId.Value;
    }

    // 🔹 Busca pedido atualizado
    public async Task CarregarPedidoAsync()
    {
        if (!_pedidoId.HasValue)
            return;

        Pedido = await _http.GetFromJsonAsync<PedidoDto>($"api/pedidos/{_pedidoId}");
        OnChange?.Invoke();
    }

    // 🔹 Adiciona item
    public async Task<(bool Sucesso, string? Erro)> AddItemAsync(
    int produtoId,
    int quantidade)
    {
        var pedidoId = await GarantirPedidoAsync();

        var response = await _http.PostAsJsonAsync(
            $"api/pedidos/{pedidoId}/itens",
            new { produtoId, quantidade }
        );

        // 👉 FALHA (ex: estoque)
        if (!response.IsSuccessStatusCode)
        {
            try
            {
                var erro = await response.Content
                    .ReadFromJsonAsync<ApiErrorResponse>();

                return (false, erro?.Message ?? "Erro ao adicionar item");
            }
            catch
            {
                return (false, "Erro ao adicionar item");
            }
        }

        // 👉 SUCESSO
        await CarregarPedidoAsync();
        return (true, null);
    }

    public async Task AlterarQuantidadeAsync(int produtoId, int quantidade)
    {
        if (!_pedidoId.HasValue)
            return;

        var response = await _http.PutAsJsonAsync(
            $"api/pedidos/{_pedidoId}/itens/{produtoId}",
            new { quantidade });

        if (!response.IsSuccessStatusCode)
        {
            var erro = await response.Content.ReadAsStringAsync();
            throw new Exception(erro);
        }

        await CarregarPedidoAsync();
    }


    // 🔹 Remove item
    public async Task RemoverItemAsync(int produtoId)
    {
        var item = Pedido.Itens.FirstOrDefault(i => i.ProdutoId == produtoId);
        if (item == null) return;

        Pedido.Itens.Remove(item);

        OnChange?.Invoke();
    }
}
