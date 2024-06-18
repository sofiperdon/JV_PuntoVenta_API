using JV_PuntoVenta.Data;
using JV_PuntoVenta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace JV_PuntoVenta_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Ventas : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public Ventas(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/<Ventas>
        [HttpGet]
        public async Task<ActionResult<string>> Get()
        {
            var ventas = await _context.Ventas.ToListAsync();
            return JsonConvert.SerializeObject(ventas, Formatting.Indented);
        }

        // GET api/<Ventas>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(int id)
        {
            if (id == 0)
            {
                return "No encontrado";
            }

            var venta = await _context.Ventas.FirstOrDefaultAsync(m => m.Id == id);
            if (venta == null)
            {
                return "No encontrado";
            }

            return JsonConvert.SerializeObject(venta, Formatting.Indented);
        }


        // POST api/<Ventas>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<Ventas>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<Ventas>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
