using MediatR;

namespace Application.Commands
{
    public class DeleteClienteCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
