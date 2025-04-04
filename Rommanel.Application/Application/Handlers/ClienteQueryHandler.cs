using Application.Querys;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Rommanel.Application.Model;
using Rommanel.Infrastructure.Data;

public class ClienteQueryHandler : IRequestHandler<GetClienteQuery, List<ClienteResponse>>, IRequestHandler<GetClienteByIdQuery, ClienteResponse>
{
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _context;

    public ClienteQueryHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ClienteResponse>> Handle(GetClienteQuery request, CancellationToken cancellationToken)
    {
        var clientes = await _context.Clientes.ToListAsync(cancellationToken: cancellationToken);
        return _mapper.Map<List<ClienteResponse>>(clientes);
    }

    public async Task<ClienteResponse> Handle(GetClienteByIdQuery request, CancellationToken cancellationToken)
    {
        var cliente = await _context.Clientes.FindAsync(new object[] { request.Id }, cancellationToken);
        if (cliente == null)
        {
            return null;
        }
        return _mapper.Map<ClienteResponse>(cliente);
    }
}
