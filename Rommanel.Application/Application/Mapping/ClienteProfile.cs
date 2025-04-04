using Application.Commands;
using AutoMapper;
using Rommanel.Application.Model;
using Rommanel.Domain.Entities;

namespace Rommanel.Application.Mapping
{
    public class ClienteProfile : Profile
    {
        public ClienteProfile()
        {
            CreateMap<CreateClienteCommand, Cliente>().ReverseMap();
            CreateMap<UpdateClienteCommand, Cliente>().ReverseMap();
            CreateMap<ClienteResponse, Cliente>().ReverseMap();
        }
    }
}
