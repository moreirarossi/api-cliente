using Application.Base;
using MediatR;

namespace Application.Commands
{
    public class CreateClienteCommand : ClienteBaseCommand, IRequest<int>
    {

    }

}
