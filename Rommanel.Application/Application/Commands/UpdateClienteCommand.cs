using Application.Base;
using MediatR;

namespace Application.Commands
{
    public class UpdateClienteCommand : ClienteBaseCommand, IRequest<bool>
    {
        public int Id { get; set; }
    }
}
