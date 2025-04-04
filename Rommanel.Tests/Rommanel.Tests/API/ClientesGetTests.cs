using Application.Querys;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Rommanel.API.Controllers;
using Rommanel.Application.Model;
using Rommanel.Domain.Entities;
using Xunit;

namespace Rommanel.Tests.API.Controllers
{
    public class ClientesGetTests
    {
        private readonly IMediator _mediator;
        private readonly ClientesController _controller;

        public ClientesGetTests()
        {
            _mediator = Substitute.For<IMediator>();
            _controller = new ClientesController(_mediator);
        }

        [Fact]
        public async Task GetAllAsync_ComClientesExistentes_DeveRetornarLista()
        {
            // Arrange
            var clientesMock = new List<ClienteResponse>
                {
                    new ClienteResponse { Id = 1, Nome = "Cliente 1" },
                    new ClienteResponse { Id = 2, Nome = "Cliente 2" }
                };

            _mediator.Send(Arg.Any<GetClienteQuery>())
                .Returns(Task.FromResult(clientesMock));

            // Act
            var result = await _controller.GetAllAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var retorno = Assert.IsAssignableFrom<IEnumerable<ClienteResponse>>(okResult.Value);
            Assert.Equal(2, retorno.Count());
            Assert.Collection(retorno,
                item => Assert.Equal("Cliente 1", item.Nome),
                item => Assert.Equal("Cliente 2", item.Nome));

            await _mediator.Received(1).Send(Arg.Any<GetClienteQuery>());
        }

        [Fact]
        public async Task GetAllAsync_SemClientes_DeveRetornarListaVazia()
        {
            // Arrange
            _mediator.Send(Arg.Any<GetClienteQuery>())
                .Returns(Task.FromResult(new List<ClienteResponse>()));

            // Act
            var result = await _controller.GetAllAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var retorno = Assert.IsAssignableFrom<IEnumerable<ClienteResponse>>(okResult.Value);
            Assert.Empty(retorno);

            await _mediator.Received(1).Send(Arg.Any<GetClienteQuery>());
        }

        [Fact]
        public async Task GetAllAsync_ComErroNoServico_DeveRetornarInternalServerError()
        {
            // Arrange
            var mensagemErro = "Erro ao acessar o banco de dados";

            _mediator.Send(Arg.Any<GetClienteQuery>())
                .ThrowsAsync(new Exception(mensagemErro));

            // Act
            var result = await _controller.GetAllAsync();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
            Assert.Equal(mensagemErro, statusCodeResult.Value);
        }

        [Fact]
        public async Task GetAllAsync_DeveChamarServicoApenasUmaVez()
        {
            // Arrange
            _mediator.Send(Arg.Any<GetClienteQuery>())
                .Returns(Task.FromResult(new List<ClienteResponse>()));

            // Act
            await _controller.GetAllAsync();

            // Assert
            await _mediator.Received(1).Send(Arg.Any<GetClienteQuery>());
        }

        [Fact]
        public async Task GetByIdAsync_ComIdValido_DeveRetornarCliente()
        {
            // Arrange
            int idValido = 1;
            var clienteMock = new ClienteResponse { Id = idValido, Nome = "Cliente Teste" };

            _mediator.Send(Arg.Is<GetClienteByIdQuery>(x => x.Id == idValido))
                .Returns(Task.FromResult(clienteMock));

            // Act
            var result = await _controller.GetByIdAsync(idValido);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var retorno = Assert.IsType<ClienteResponse>(okResult.Value);
            Assert.Equal(idValido, retorno.Id);
            Assert.Equal("Cliente Teste", retorno.Nome);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetByIdAsync_ComIdInvalido_DeveRetornarBadRequest(int idInvalido)
        {
            // Arrange

            // Act
            var result = await _controller.GetByIdAsync(idInvalido);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("ID do cliente é inválido", badRequestResult.Value);
            await _mediator.DidNotReceive().Send(Arg.Any<GetClienteQuery>());
        }

        [Fact]
        public async Task GetByIdAsync_ClienteNaoEncontrado_DeveRetornarNotFound()
        {
            // Arrange
            int idNaoExistente = 999;

            _mediator.Send(Arg.Is<GetClienteByIdQuery>(x => x.Id == idNaoExistente))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.GetByIdAsync(idNaoExistente);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Cliente não encontrado.", notFoundResult.Value);
        }

        [Fact]
        public async Task GetByIdAsync_ErroNoServico_DeveRetornarInternalServerError()
        {
            // Arrange
            int id = 1;
            var mensagemErro = "Erro no banco de dados";

            _mediator.Send(Arg.Is<GetClienteByIdQuery>(x => x.Id == id))
                .ThrowsAsync(new Exception(mensagemErro));

            // Act
            var result = await _controller.GetByIdAsync(id);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
            Assert.Equal(mensagemErro, statusCodeResult.Value);
        }

        [Fact]
        public async Task GetByIdAsync_DeveChamarServicoApenasUmaVez()
        {
            // Arrange
            int id = 1;
            _mediator.Send(Arg.Is<GetClienteByIdQuery>(x => x.Id == id))
                .Returns(Task.FromResult(new ClienteResponse()));

            // Act
            await _controller.GetByIdAsync(id);

            // Assert
            await _mediator.Received(1).Send(Arg.Is<GetClienteByIdQuery>(x => x.Id == id));
        }
    }
}