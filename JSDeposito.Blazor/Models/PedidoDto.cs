namespace JSDeposito.Blazor.Models
{
    public class PedidoDto
    {
        public int Id { get; set; }
        public decimal Total { get; set; }
        public List<ItemPedidoDto> Itens { get; set; } = new();
    }
}
