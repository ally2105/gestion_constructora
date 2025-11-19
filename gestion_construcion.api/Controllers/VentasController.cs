using AutoMapper;
using Firmeza.Api.DTOs;
using Firmeza.Core.DTOs;
using Firmeza.Core.Interfaces;
using Firmeza.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Firmeza.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VentasController : ControllerBase
    {
        private readonly IVentaService _ventaService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public VentasController(IVentaService ventaService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _ventaService = ventaService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: /api/Ventas
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<IEnumerable<VentaDto>>> GetAllVentas()
        {
            var ventas = await _unitOfWork.Ventas.GetAllAsync();
            var ventasDto = _mapper.Map<IEnumerable<VentaDto>>(ventas);
            return Ok(ventasDto);
        }

        // GET: /api/Ventas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VentaDto>> GetVentaById(int id)
        {
            var venta = await _unitOfWork.Ventas.GetByIdAsync(id);
            if (venta == null)
            {
                return NotFound();
            }
            var ventaDto = _mapper.Map<VentaDto>(venta);
            return Ok(ventaDto);
        }

        // POST: /api/Ventas
        [HttpPost]
        public async Task<ActionResult<VentaDto>> CreateVenta([FromBody] VentaCreateDto ventaCreateDto)
        {
            var nuevaVenta = await _ventaService.CrearVentaAsync(ventaCreateDto);
            var ventaDto = _mapper.Map<VentaDto>(nuevaVenta);

            return CreatedAtAction(nameof(GetVentaById), new { id = ventaDto.Id }, ventaDto);
        }
    }
}
