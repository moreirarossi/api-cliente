using Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Rommanel.API.Controllers;
using Rommanel.Application.Validations;
using Rommanel.Domain.Enum;
using Xunit;

namespace Rommanel.Tests.API.Controllers
{
    public class ClientesUpdateTests
    {
        private readonly IMediator _mediator;
        private readonly ClientesController _controller;

        public ClientesUpdateTests()
        {
            _mediator = Substitute.For<IMediator>();
            _controller = new ClientesController(_mediator);
        }

        [Fact]
        public async Task UpdateAsync_ComDadosValidos_DeveRetornarOk()
        {
            // Arrange
            var mediator = Substitute.For<IMediator>();
            var validator = new ClienteValidator<UpdateClienteCommand>();

            var request = new UpdateClienteCommand
            {
                Id = 1,
                Nome = "Cliente Atualizado",
                CPFCNPJ = "12345678909",
                Email = "email@valido.com",
                Tipo = TipoCliente.Fisica,
                DataNascimento = DateTime.Now.AddYears(-20)
            };

            var validationResult = await validator.ValidateAsync(request);
            Assert.True(validationResult.IsValid);

            // Simula o comportamento esperado do MediatR ao processar o comando
            mediator.Send(Arg.Any<UpdateClienteCommand>()).Returns(Task.FromResult(true));

            var controller = new ClientesController(mediator);

            // Act
            var result = await controller.UpdateAsync(request.Id, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Cliente atualizado com sucesso!", okResult.Value);

            // Verifica se o MediatR recebeu o comando uma vez
            await mediator.Received(1).Send(Arg.Any<UpdateClienteCommand>());
        }


        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task UpdateAsync_ComIdInvalido_DeveRetornarBadRequest(int id)
        {
            // Arrange
            var mediator = Substitute.For<IMediator>();
            var request = new UpdateClienteCommand { Id = 1 }; 

            var controller = new ClientesController(mediator);

            // Act
            var result = await controller.UpdateAsync(id, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("ID do cliente é inválido", badRequestResult.Value);
            await mediator.DidNotReceive().Send(Arg.Any<UpdateClienteCommand>());
        }


        [Fact]
        public async Task UpdateAsync_ComIdDivergente_DeveRetornarBadRequest()
        {
            // Arrange
            var mediator = Substitute.For<IMediator>();
            var request = new UpdateClienteCommand { Id = 1 }; // Usando CQRS com MediatR

            var controller = new ClientesController(mediator);

            // Act
            var result = await controller.UpdateAsync(2, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("O ID do cliente divergente dos dados informados para atualização", badRequestResult.Value);

            await mediator.DidNotReceive().Send(Arg.Any<UpdateClienteCommand>());
        }

        [Theory]
        [InlineData("", "12345678909", "email@valido.com", TipoCliente.Fisica, "Nome")]
        [InlineData("Cliente", "12345678909", "email-invalido", TipoCliente.Fisica, "Email")]
        [InlineData("Cliente", "", "email@valido.com", TipoCliente.Fisica, "CPFCNPJ")]
        [InlineData("Cliente", "123456789", "email@valido.com", TipoCliente.Fisica, "CPFCNPJ")]
        [InlineData("Cliente", "12345678901", "email@valido.com", TipoCliente.Juridica, "CPFCNPJ")]
        public async Task UpdateAsync_ComDadosInvalidos_DeveFalharNaValidacao(
            string nome, string cpfCnpj, string email, TipoCliente tipo, string propriedadeEsperada)
        {
            // Arrange
            var validator = new ClienteValidator<UpdateClienteCommand>();
            var request = new UpdateClienteCommand
            {
                Id = 1,
                Nome = nome,
                CPFCNPJ = cpfCnpj,
                Email = email,
                Tipo = tipo,
                DataNascimento = DateTime.Now.AddYears(-20)
            };

            // Act
            var result = await validator.ValidateAsync(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == propriedadeEsperada);
        }

        [Fact]
        public async Task UpdateAsync_ClienteNaoEncontrado_DeveRetornarNotFound()
        {
            // Arrange
            var mediator = Substitute.For<IMediator>();
            var request = new UpdateClienteCommand { Id = 99 }; // Usando CQRS com MediatR

            mediator.Send(Arg.Any<UpdateClienteCommand>())
                    .ThrowsAsync(new KeyNotFoundException());

            var controller = new ClientesController(mediator);

            // Act
            var result = await controller.UpdateAsync(99, request);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Cliente não encontrado.", notFoundResult.Value);

            await mediator.Received(1).Send(Arg.Any<UpdateClienteCommand>());
        }


        [Fact]
        public async Task UpdateAsync_ComExcecaoGenerica_Retorna500InternalServerError()
        {
            // Arrange
            var mediator = Substitute.For<IMediator>();
            var request = new UpdateClienteCommand { Id = 1 };
            var exceptionMessage = "Erro inesperado";
            var exception = new Exception(exceptionMessage);

            mediator.Send(Arg.Any<UpdateClienteCommand>()).ThrowsAsync(exception);

            var controller = new ClientesController(mediator);

            // Act
            var result = await controller.UpdateAsync(1, request);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);

            if (!string.IsNullOrEmpty(objectResult.Value?.ToString()))
                Assert.Contains(exceptionMessage, objectResult.Value.ToString());

            await mediator.Received(1).Send(Arg.Any<UpdateClienteCommand>());
        }

    }
}