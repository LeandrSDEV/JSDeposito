namespace JSDeposito.Core.Entities;

public class Cupom
{
    public int Id { get; private set; }
    public string Codigo { get; private set; }
    public decimal ValorDesconto { get; private set; }
    public bool Percentual { get; private set; }
    public DateTime ValidoAte { get; private set; }
    public int LimiteUso { get; private set; }
    public int Usos { get; private set; }
    public bool Ativo { get; private set; }

    protected Cupom() { }

    public Cupom(
        string codigo,
        decimal valorDesconto,
        bool percentual,
        DateTime validoAte,
        int limiteUso)
    {
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

        if (Percentual)
            return totalPedido * (ValorDesconto / 100);

        return ValorDesconto;
    }

    public void RegistrarUso()
    {
        Usos++;
    }
}
