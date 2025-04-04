using Application.Commands;
using MediatR;
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
    public class ClientesCreateTests
    {
        private readonly IMediator _mediator;
        private readonly ClientesController _controller;

        public ClientesCreateTests()
        {
            _mediator = Substitute.For<IMediator>();
            _controller = new ClientesController(_mediator);
        }

        [Fact]
        public async Task CreateAsync_ComDadosValidos_RetornaOkComMensagem()
        {
            // Arrange
            var request = new CreateClienteCommand
            {
                Nome = "Cliente Teste",
                CPFCNPJ = "12345678909",
                Email = "teste@email.com",
                Tipo = TipoCliente.Fisica,
                DataNascimento = DateTime.Now.AddYears(-20)
            };

            _mediator.Send(Arg.Any<CreateClienteCommand>()).Returns(Task.FromResult(0));

            // Act
            var result = await _controller.CreateAsync(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Cliente adicionado com sucesso!", okResult.Value);
            await _mediator.Received(1).Send(Arg.Any<CreateClienteCommand>());
        }

        [Theory]
        [InlineData(TipoCliente.Fisica, "", "12345678909", "", "email@valido.com", "Nome")]
        [InlineData(TipoCliente.Fisica, "Cliente", "12345678909", "", "email-invalido", "Email")]
        [InlineData(TipoCliente.Fisica, "Cliente", "", "", "email@valido.com", "CPFCNPJ")]
        [InlineData(TipoCliente.Fisica, "Cliente", "123456789", "", "email@valido.com", "CPFCNPJ")]
        [InlineData(TipoCliente.Juridica, "", "12345678901234", "123", "email@valido.com", "Nome")]
        [InlineData(TipoCliente.Juridica, "Cliente", "12345678901", "123", "", "Email")]
        [InlineData(TipoCliente.Juridica, "Cliente", "", "123", "email@valido.com", "CPFCNPJ")]
        [InlineData(TipoCliente.Juridica, "Cliente", "12345678901", "123", "email@valido.com", "CPFCNPJ")]
        [InlineData(TipoCliente.Juridica, "Cliente", "12345678901234", "", "email@valido.com", "IE")]
        public async Task CreateAsync_ComDadosInvalidos_DeveFalharNaValidacao(
            TipoCliente tipo, string nome, string cpfCnpj, string ie, string email, string propriedadeEsperada)
        {
            // Arrange
            var validator = new ClienteValidator<CreateClienteCommand>();
            var request = new CreateClienteCommand
            {
                Nome = nome,
                CPFCNPJ = cpfCnpj,
                Email = email,
                Tipo = tipo,
                IE = ie,
                DataNascimento = DateTime.Now.AddYears(-20)
            };

            // Act
            var result = await validator.ValidateAsync(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == propriedadeEsperada);
        }

        [Fact]
        public async Task CreateAsync_ComExcecaoDeBanco_Retorna409Conflict()
        {
            // Arrange
            var command = new CreateClienteCommand()
            {
                Nome = "Empresa teste",
                CPFCNPJ = "12345678901234",
                Email= "email@provedor.com.br",
                IE = "ISENTO",
                Tipo = TipoCliente.Juridica
            };
            var exception = new DbUpdateException("Erro de banco", new Exception("Violação de chave única"));

            _mediator.Send(Arg.Any<CreateClienteCommand>())
                .ThrowsAsync(exception);

            // Act
            var result = await _controller.CreateAsync(command);

            // Assert
            var failedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(409, failedResult.StatusCode);
        }

        [Fact]
        public async Task CreateAsync_ComExcecaoGenerica_Retorna500InternalServerError()
        {
            // Arrange
            var command = new CreateClienteCommand()
            {
                Nome = "Empresa teste",
                CPFCNPJ = "12345678901234",
                Email = "email@provedor.com.br",
                IE = "123456789",
                Tipo = TipoCliente.Juridica
            };
            var exceptionMessage = "Erro inesperado";
            var exception = new Exception(exceptionMessage);

            _mediator.Send(Arg.Any<CreateClienteCommand>())
                .ThrowsAsync(exception);

            // Act
            var result = await _controller.CreateAsync(command);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            if (!String.IsNullOrEmpty(objectResult.Value.ToString()))
                Assert.Contains(exceptionMessage, objectResult.Value.ToString());
        }
    }
}