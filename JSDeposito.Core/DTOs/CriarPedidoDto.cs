namespace JSDeposito.Core.DTOs;

public class CriarPedidoDto
{
    public List<ItemPedidoDto> Itens { get; set; } = new();
    public string? CodigoCupom { get; set; }
}

public class ItemPedidoDto
{
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
}
