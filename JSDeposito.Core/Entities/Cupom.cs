
namespace JSDeposito.Core.Entities;

public class Cupom
{
    public int Id { get; private set; }
    public string Codigo { get; private set; }
    public decimal? ValorDesconto { get; private set; }
    public int? Percentual { get; private set; }
    public DateTime ValidoAte { get; private set; }
    public int LimiteUso { get; private set; }
    public int Usos { get; private set; }
    public bool Ativo { get; private set; }

    protected Cupom() { }

    public Cupom(
        string codigo,
        decimal? valorDesconto,
        int? percentual,
        DateTime validoAte,
        int limiteUso)
    {
        if ((valorDesconto.HasValue && percentual.HasValue) ||
            (!valorDesconto.HasValue && !percentual.HasValue))
            throw new Exception("Cupom deve ter valor OU percentual");

        Codigo = codigo;
        ValorDesconto = valorDesconto;
        Percentual = percentual;
        ValidoAte = validoAte;
        LimiteUso = limiteUso;
        Usos = 0;
        Ativo = true;
    }

    public void Validar()
    {
        if (!Ativo)
            throw new Exception("Cupom inativo");

        if (DateTime.Now > ValidoAte)
            throw new Exception("Cupom expirado");

        if (Usos >= LimiteUso)
            throw new Exception("Cupom esgotado");
    }

    public decimal CalcularDesconto(decimal totalPedido)
    {
        Validar();

        if (Percentual.HasValue)
            return totalPedido * (Percentual.Value / 100m);

        return ValorDesconto!.Value;
    }

    public void RegistrarUso()
    {
        Usos++;
    }
}