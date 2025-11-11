using AutoMapper;
using Firmeza.Api.DTOs;
using gestion_construccion.web.Models;

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
            // Para el nombre y el email, le decimos a AutoMapper que los saque del objeto Usuario anidado.
            CreateMap<Cliente, ClienteDto>()
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Usuario.Nombre))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Usuario.Email));
        }
    }
}
