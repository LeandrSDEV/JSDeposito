namespace JSDeposito.Core.Entities;

public class PromocaoFrete
{
    public int Id { get; private set; }
    public string Nome { get; private set; }
    public DateTime Inicio { get; private set; }
    public DateTime Fim { get; private set; }
    public bool Ativa { get; private set; }

    protected PromocaoFrete() { }

    public PromocaoFrete(string nome, DateTime inicio, DateTime fim)
    {
        Atualizar(nome, inicio, fim, true);
    }

    public void Atualizar(string nome, DateTime inicio, DateTime fim, bool ativa)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome inválido");

        if (fim < inicio)
            throw new ArgumentException("Data final não pode ser menor que a inicial");

        Nome = nome;
        Inicio = inicio;
        Fim = fim;
        Ativa = ativa;
    }

    public bool EstaAtiva()
    {
        var agora = DateTime.Now;
        return Ativa && agora >= Inicio && agora <= Fim;
    }
}
