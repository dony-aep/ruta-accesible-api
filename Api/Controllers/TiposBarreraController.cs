using Api.Data;
using Api.Dtos;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TiposBarreraController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TiposBarreraController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/TiposBarrera
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TipoBarreraDto>>> GetTiposBarrera()
        {
            var tipos = await _context.TiposBarrera
                .Select(t => new TipoBarreraDto
                {
                    Id = t.Id,
                    Codigo = t.Codigo,
                    Nombre = t.Nombre,
                    CriterioNorma = t.CriterioNorma
                })
                .ToListAsync();
            return Ok(tipos);
        }

        // GET: api/TiposBarrera/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TipoBarreraDto>> GetTipoBarrera(int id)
        {
            var tipo = await _context.TiposBarrera.FindAsync(id);

            if (tipo == null)
            {
                return NotFound();
            }

            var dto = new TipoBarreraDto
            {
                Id = tipo.Id,
                Codigo = tipo.Codigo,
                Nombre = tipo.Nombre,
                CriterioNorma = tipo.CriterioNorma
            };

            return Ok(dto);
        }

        // POST: api/TiposBarrera
        [HttpPost]
        public async Task<ActionResult<TipoBarreraDto>> PostTipoBarrera(TipoBarreraCrearDto dto)
        {
            // El código tiene índice único en AppDbContext: sin esta comprobación previa
            // el duplicado revienta en SaveChangesAsync y sale como 500.
            var codigoEnUso = await _context.TiposBarrera
                .AnyAsync(t => t.Codigo == dto.Codigo);

            if (codigoEnUso)
            {
                return BadRequest(new
                {
                    mensaje = $"Ya existe un tipo de barrera con el código {dto.Codigo}."
                });
            }

            var tipo = new TipoBarrera
            {
                Codigo = dto.Codigo,
                Nombre = dto.Nombre,
                CriterioNorma = dto.CriterioNorma
            };

            _context.TiposBarrera.Add(tipo);
            await _context.SaveChangesAsync();

            var tipoDto = new TipoBarreraDto
            {
                Id = tipo.Id,
                Codigo = tipo.Codigo,
                Nombre = tipo.Nombre,
                CriterioNorma = tipo.CriterioNorma
            };

            return CreatedAtAction(nameof(GetTipoBarrera), new { id = tipo.Id }, tipoDto);
        }

        // PUT: api/TiposBarrera/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTipoBarrera(int id, TipoBarreraCrearDto dto)
        {
            var tipo = await _context.TiposBarrera.FindAsync(id);

            if (tipo == null)
            {
                return NotFound();
            }

            // Mismo índice único que en el POST, excluyendo el propio registro para que
            // se pueda editar el nombre o el criterio sin cambiar el código.
            var codigoEnUso = await _context.TiposBarrera
                .AnyAsync(t => t.Codigo == dto.Codigo && t.Id != id);

            if (codigoEnUso)
            {
                return BadRequest(new
                {
                    mensaje = $"Ya existe otro tipo de barrera con el código {dto.Codigo}."
                });
            }

            tipo.Nombre = dto.Nombre;
            tipo.Codigo = dto.Codigo;
            tipo.CriterioNorma = dto.CriterioNorma;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/TiposBarrera/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTipoBarrera(int id)
        {
            var tipo = await _context.TiposBarrera.FindAsync(id);
            if (tipo == null)
            {
                return NotFound();
            }

            // Validacion de borrado protegido usando la entidad ReporteAccesibilidad real
            var tieneReportes = await _context.Reportes
                .AnyAsync(r => r.TipoBarreraId == id);
            
            if (tieneReportes)
            {
                return BadRequest(new { 
                    mensaje = "No se puede eliminar el tipo de barrera porque tiene reportes asociados." 
                });
            }

            _context.TiposBarrera.Remove(tipo);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}