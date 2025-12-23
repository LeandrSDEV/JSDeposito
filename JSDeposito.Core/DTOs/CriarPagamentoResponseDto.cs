using JSDeposito.Core.Enums;

namespace JSDeposito.Core.DTOs;

public class CriarPagamentoResponseDto
{
    public int PagamentoId { get; set; }
    public StatusPagamento Status { get; set; }
    public object? Pix { get; set; }
}

