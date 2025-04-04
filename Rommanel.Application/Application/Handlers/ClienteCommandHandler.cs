using Application.Commands;
using AutoMapper;
using MediatR;
using Rommanel.Domain.Entities;
using Rommanel.Infrastructure.Data;

public class ClienteCommandHandler : IRequestHandler<CreateClienteCommand, int>, IRequestHandler<UpdateClienteCommand, bool>, IRequestHandler<DeleteClienteCommand, bool>
{
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _context;

    public ClienteCommandHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<int> Handle(CreateClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = _mapper.Map<Cliente>(request);
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();
        return cliente.Id;
    }

    public async Task<bool> Handle(UpdateClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = await _context.Clientes.FindAsync(request.Id);
        if (cliente == null) return false;

        _mapper.Map(request, cliente);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Handle(DeleteClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = await _context.Clientes.FindAsync(request.Id);
        if (cliente == null) return false;

        _context.Clientes.Remove(cliente);
        await _context.SaveChangesAsync();
        return true;
    }
}