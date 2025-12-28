namespace JSDeposito.Blazor.Models
{
    public class ItemPedidoDto
    {
        public int ProdutoId { get; set; }
        public string ProdutoNome { get; set; } = "";
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Subtotal => Quantidade * PrecoUnitario;
    }
}
