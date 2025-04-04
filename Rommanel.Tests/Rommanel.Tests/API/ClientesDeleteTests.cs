using Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Rommanel.API.Controllers;
using Xunit;

namespace Rommanel.Tests.API.Controllers
{
    public class ClientesDeleteTests
    {
        private readonly IMediator _mediator;
        private readonly ClientesController _controller;

        public ClientesDeleteTests()
        {
            _mediator = Substitute.For<IMediator>();
            _controller = new ClientesController(_mediator);
        }


        [Fact]
        public async Task DeleteAsync_ComIdValido_DeveRetornarNoContent()
        {
            // Arrange
            int idValido = 1;
            _mediator.Send(Arg.Any<DeleteClienteCommand>()).Returns(Task.FromResult(true));

            // Act
            var result = await _controller.DeleteAsync(idValido);

            // Assert
            Assert.IsType<NoContentResult>(result);
            await _mediator.Received(1).Send(Arg.Any<DeleteClienteCommand>());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task DeleteAsync_ComIdInvalido_DeveRetornarBadRequest(int idInvalido)
        {
            // Act
            var result = await _controller.DeleteAsync(idInvalido);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("ID do cliente deve ser maior que zero", badRequestResult.Value);
            await _mediator.DidNotReceive().Send(Arg.Any<DeleteClienteCommand>());
        }

        [Fact]
        public async Task DeleteAsync_ClienteNaoEncontrado_DeveRetornarNotFound()
        {
            // Arrange
            int idNaoExistente = 999;
            var mensagemErro = "Cliente não encontrado";

            _mediator.Send(Arg.Any<DeleteClienteCommand>())
                .ThrowsAsync(new KeyNotFoundException(mensagemErro));

            // Act
            var result = await _controller.DeleteAsync(idNaoExistente);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(mensagemErro, notFoundResult.Value);
        }

        [Fact]
        public async Task DeleteAsync_OperacaoInvalida_DeveRetornarBadRequest()
        {
            // Arrange
            int id = 1;
            var mensagemErro = "Cliente não pôde ser excluído";

            _mediator.Send(Arg.Any<DeleteClienteCommand>())
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            var result = await _controller.DeleteAsync(id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(mensagemErro, badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteAsync_ErroInesperado_DeveRetornarInternalServerError()
        {
            // Arrange
            int id = 1;
            var mensagemErro = "Erro inesperado no banco de dados";

            _mediator.Send(Arg.Any<DeleteClienteCommand>())
                .ThrowsAsync(new Exception(mensagemErro));

            // Act
            var result = await _controller.DeleteAsync(id);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
            Assert.Equal(mensagemErro, statusCodeResult.Value);
        }
    }
}