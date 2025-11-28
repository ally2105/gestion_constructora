using AutoMapper;
using Firmeza.Api.DTOs;
using Firmeza.Core.Models;

namespace Firmeza.Api.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapping from Product to ProductDto and vice versa
            CreateMap<Producto, ProductoDto>();
            CreateMap<ProductoCreateDto, Producto>();

            // Mapping from Client to ClientDto
            CreateMap<Cliente, ClienteDto>()
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Usuario.Nombre))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Usuario.Email));

            // Mapping for creating Clients
            CreateMap<ClienteCreateDto, Usuario>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
            CreateMap<ClienteCreateDto, Cliente>();

            // Mapping for updating Clients
            CreateMap<ClienteUpdateDto, Usuario>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
            CreateMap<ClienteUpdateDto, Cliente>();

            // Mapping for Sales
            CreateMap<DetalleVenta, DetalleVentaDto>()
                .ForMember(dest => dest.ProductoNombre, opt => opt.MapFrom(src => src.Producto.Nombre));
            CreateMap<Venta, VentaDto>()
                .ForMember(dest => dest.ClienteNombre, opt => opt.MapFrom(src => src.Cliente.Usuario.Nombre));
        }
    }
}
