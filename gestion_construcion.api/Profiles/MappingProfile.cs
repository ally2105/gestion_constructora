using AutoMapper;
using Firmeza.Api.DTOs;
using Firmeza.Core.Models;

namespace Firmeza.Api.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapeo de Producto a ProductoDto y viceversa
            CreateMap<Producto, ProductoDto>();
            CreateMap<ProductoCreateDto, Producto>();

            // Mapeo de Cliente a ClienteDto
            CreateMap<Cliente, ClienteDto>()
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Usuario.Nombre))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Usuario.Email));

            // Mapeo para la creación de Clientes
            CreateMap<ClienteCreateDto, Usuario>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
            CreateMap<ClienteCreateDto, Cliente>();

            // Mapeo para la actualización de Clientes
            CreateMap<ClienteUpdateDto, Usuario>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
            CreateMap<ClienteUpdateDto, Cliente>();

            // Mapeo para Ventas
            CreateMap<DetalleVenta, DetalleVentaDto>()
                .ForMember(dest => dest.ProductoNombre, opt => opt.MapFrom(src => src.Producto.Nombre));
            CreateMap<Venta, VentaDto>()
                .ForMember(dest => dest.ClienteNombre, opt => opt.MapFrom(src => src.Cliente.Usuario.Nombre));
        }
    }
}
