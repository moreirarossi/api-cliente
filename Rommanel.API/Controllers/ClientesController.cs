using Application.Commands;
using Application.Querys;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rommanel.Application.Model;
using Rommanel.Application.Validations;

namespace Rommanel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ClienteValidator<CreateClienteCommand> _createValidator;
        private readonly ClienteValidator<UpdateClienteCommand> _updateValidator;

        public ClientesController(IMediator mediator)
        {
            _mediator = mediator;
            _createValidator = new ClienteValidator<CreateClienteCommand>();
            _updateValidator = new ClienteValidator<UpdateClienteCommand>();
        }

        /// <summary>
        /// Cria um novo cliente
        /// </summary>
        /// <param name="request">Dados do cliente</param>
        /// <response code="201">Cliente criado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="500">Erro interno no servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateClienteCommand request)
        {
            try
            {
                var validationResult = _createValidator.Validate(request);

                if (!validationResult.IsValid)
                    return BadRequest(validationResult.Errors);

                await _mediator.Send(request);
                return Ok("Cliente adicionado com sucesso!");
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.InnerException.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Atualiza um cliente existente
        /// </summary>
        /// <param name="id">ID do cliente</param>
        /// <param name="request">Dados do cliente</param>
        /// <response code="200">Cliente atualizado com sucesso</response>
        /// <response code="400">Dados da requisição inválidos</response>
        /// <response code="404">Cliente não encontrado</response>
        /// <response code="500">Erro interno no servidor</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateClienteCommand request)
        {
            try
            {
                var validationResult = _updateValidator.Validate(request);

                if (!validationResult.IsValid)
                    BadRequest(validationResult.Errors);

                if (id <= 0)
                    return BadRequest("ID do cliente é inválido");

                if (id != request.Id)
                    return BadRequest("O ID do cliente divergente dos dados informados para atualização");

                await _mediator.Send(request);
                return Ok("Cliente atualizado com sucesso!");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound($"Cliente não encontrado.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Obtém váriso clientes
        /// </summary>
        /// <response code="200">Retorna a lista de clientes</response>
        /// <response code="500">Erro interno</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ClienteResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var clientes = await _mediator.Send(new GetClienteQuery());
                return Ok(clientes);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Obtém um cliente por ID
        /// </summary>
        /// <param name="id">ID do cliente</param>
        /// <response code="200">Retorna o cliente</response>
        /// <response code="400">Dados da requisição inválidos</response>
        /// <response code="404">Cliente não encontrado</response>
        /// <response code="500">Erro interno</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("ID do cliente é inválido");

                var Cliente = await _mediator.Send(new GetClienteByIdQuery() { Id = id });
                return Ok(Cliente);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound($"Cliente não encontrado.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Remove um cliente existente
        /// </summary>
        /// <param name="id">ID do cliente</param>
        /// <response code="204">Cliente removido com sucesso</response>
        /// <response code="400">Dados da requisição inválidos</response>
        /// <response code="404">Cliente não encontrado</response>
        /// <response code="500">Erro interno no servidor</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("ID do cliente deve ser maior que zero");

                await _mediator.Send(new DeleteClienteCommand() { Id = id });
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
