using JV_PuntoVenta.Data;
using JV_PuntoVenta.Models;
using JV_PuntoVenta_API.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace JV_PuntoVenta_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VentasController(ApplicationDbContext context)
        {
            _context = context;
        }

        private JsonSerializerSettings GetJsonSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        // GET: api/Ventas
        [HttpGet]
        public async Task<ActionResult<string>> Get()
        {
            var ventas = await _context.Ventas
                .Include(v => v.VentaProductos)
                .ThenInclude(vp => vp.Producto)
                .ToListAsync();

            return Ok(JsonConvert.SerializeObject(ventas, GetJsonSerializerSettings()));
        }

        // GET api/Ventas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.VentaProductos)
                .ThenInclude(vp => vp.Producto)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venta == null)
            {
                return NotFound("No encontrado.");
            }

            return Ok(JsonConvert.SerializeObject(venta, GetJsonSerializerSettings()));
        }

        //POST api/Ventas
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] VentaViewModel ventaViewModel)
        {
            if (!ModelState.IsValid)//Validacion del modelo
            {
                return BadRequest(ModelState);
            }

            var options = new JsonSerializerOptions 
            {
                ReferenceHandler = ReferenceHandler.Preserve //Opcion necesaria, si no tira un error de "bucle"  al buscar los productos
            };

            var venta = new Venta //Se crea la venta y se le agrega la fecha
            {
                TransactionDateTime = DateTime.Now,
                VentaProductos = new List<VentaProducto>()
            };

            decimal total = 0;

            foreach (var ventaProductoViewModel in ventaViewModel.VentaProductos)
            {
                //Se busca cada producto
                var producto = await _context.Productos.FindAsync(ventaProductoViewModel.ProductoId);
                if (producto == null)
                {
                    return BadRequest($"Producto con ID {ventaProductoViewModel.ProductoId} no encontrado.");
                }

                //Se crea un registro de venta producto por cada producto en la vetna
                var ventaProducto = new VentaProducto
                {
                    ProductoId = ventaProductoViewModel.ProductoId,
                    Cantidad = ventaProductoViewModel.Cantidad,
                    Venta = venta
                };

                //Se va calculando el tota;
                total += producto.Precio * ventaProductoViewModel.Cantidad;
                venta.VentaProductos.Add(ventaProducto);
            }

            venta.Total = total;

            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = venta.Id }, System.Text.Json.JsonSerializer.Serialize(venta, options));
        }

        // DELETE api/Ventas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.VentaProductos)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venta == null)
            {
                return NotFound("Producto no encontrado");
            }

            _context.VentaProductos.RemoveRange(venta.VentaProductos);
            _context.Ventas.Remove(venta);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VentaExists(int id)
        {
            return _context.Ventas.Any(e => e.Id == id);
        }
    }
}
