using MediatR;
using Rommanel.Application.Model;

namespace Application.Querys
{
    public class GetClienteQuery : IRequest<List<ClienteResponse>>
    {
    }
}
