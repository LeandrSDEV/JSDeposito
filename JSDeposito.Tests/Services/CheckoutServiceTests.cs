using FluentAssertions;
using JSDeposito.Core.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace JSDeposito.Tests.Domain
{
    [Fact]
    public void Nao_Deve_Finalizar_Checkout_Com_Estoque_Insuficiente()
    {
        var produtoRepo = new Mock<IProdutoRepository>();
        produtoRepo.Setup(p => p.BaixarEstoque(It.IsAny<int>(), It.IsAny<int>()))
                   .Throws(new EstoqueInsuficienteException());

        var checkout = new CheckoutService(
            produtoRepo.Object,
            Mock.Of<IPedidoRepository>(),
            Mock.Of<ICupomRepository>(),
            Mock.Of<ILogger<CheckoutService>>()
        );

        Action act = () => checkout.Finalizar(...);

        act.Should().Throw<EstoqueInsuficienteException>();
    }

}
