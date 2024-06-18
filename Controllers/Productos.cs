using JV_PuntoVenta.Data;
using JV_PuntoVenta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace JV_PuntoVenta_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Productos : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public Productos(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/<Productos>
        [HttpGet]
        public async Task<ActionResult<string>> Get()
        {
            var productos = await _context.Productos.ToListAsync();
            return JsonConvert.SerializeObject(productos, Formatting.Indented);
        }

        // GET api/<Productos>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(int id)
        {
            if (id == 0)
            {
                return "No encontrado";
            }

            var producto = await _context.Productos.FirstOrDefaultAsync(m => m.ID == id);
            if (producto == null)
            {
                return "No encontrado";
            }

            return JsonConvert.SerializeObject(producto, Formatting.Indented);
        }

        // POST api/<Productos>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Producto producto)
        {
            if (producto == null)
            {
                return BadRequest("Producto inválido.");
            }

            producto.LastUpdateDate = DateTime.Now;
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = producto.ID }, producto);
        }

        // PUT api/<Productos>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] Producto producto)
        {
            if (id != producto.ID)
            {
                return BadRequest("ID del producto no coincide.");
            }

            var productoExistente = await _context.Productos.FindAsync(id);
            if (productoExistente == null)
            {
                return NotFound("Producto no encontrado.");
            }

            productoExistente.Nombre = producto.Nombre;
            productoExistente.Precio = producto.Precio;
            productoExistente.Categoria = producto.Categoria;
            productoExistente.LastUpdateDate = DateTime.Now;

            _context.Entry(productoExistente).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(productoExistente);
        }

        // DELETE api/<Productos>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound("Producto no encontrado.");
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
