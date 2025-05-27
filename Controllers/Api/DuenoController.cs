using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeterinariaSystem.Models;

namespace VeterinariaSystem.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class DuenoController : ControllerBase
    {
        private readonly IRepositorioDueno _repo;

        public DuenoController(IRepositorioDueno repo)
        {
            _repo = repo;
        }

        //obtener detalles de un Dueno
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var dueno = _repo.ObtenerPorId(id);
            if (dueno == null)
                return NotFound();
            return Ok(dueno);
        }

        //Obtener listado de Duenos para el index
        [HttpGet("pagina/{pagina}")]
        public IActionResult GetPorPagina(int pagina = 1)
        {
            const int cantidadPorPagina = 10;
            var total = _repo.ContarDueno();
            var duenos = _repo.ObtenerTodosPaginado(pagina, cantidadPorPagina);

            return Ok(
                new
                {
                    duenos = duenos,
                    totalPaginas = (int)Math.Ceiling((double)total / cantidadPorPagina),
                }
            );
        }

        //Obtener Dueno por Dni(muestra detalles tambien)
        [HttpGet("/api/dueno/dni/{dni}")]
        public IActionResult BuscarPorDniApi(int dni)
        {
            var dueno = _repo.BuscarPorDni(dni);
            if (dueno == null)
                return NotFound();

            return Ok(dueno);
        }

        //crear un dueño
        [HttpPost]
        public IActionResult Crear([FromBody] Dueno dueno)
        {
            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Error en el campo '{state.Key}': {error.ErrorMessage}");
                    }
                }

                return BadRequest(ModelState);
            }

            var existente = _repo.ObtenerPorDni(dueno.DNI);
            if (existente != null)
            {
                ModelState.AddModelError("DNI", "Ya existe un dueño con ese DNI.");
                return BadRequest(ModelState);
            }
            Console.WriteLine("se ingreso al HTTP POST de crear");

            _repo.Alta(dueno);
            return Ok();
        }

        //editar dueño
        [HttpPut("{id}")]
        public IActionResult Editar(int id, [FromBody] Dueno dueno)
        {
            if (id != dueno.Id)
                return BadRequest("ID no coincide.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existente = _repo.ObtenerPorId(id);
            if (existente == null)
                return NotFound();

            _repo.Modificacion(dueno);
            return Ok();
        }

        //borrar dueño
        [HttpDelete("{id}")]
        public IActionResult Eliminar(int id)
        {
            var dueno = _repo.ObtenerPorId(id);
            if (dueno == null)
                return NotFound();

            _repo.Baja(id);
            return Ok();
        }
    }
}
