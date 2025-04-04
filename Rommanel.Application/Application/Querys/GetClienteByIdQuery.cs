using MediatR;
using Rommanel.Application.Model;

namespace Application.Querys
{
    public class GetClienteByIdQuery : IRequest<ClienteResponse>
    {
        public int Id { get; set; }
    }
}
