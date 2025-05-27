using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeterinariaSystem.Models;

namespace VeterinariaSystem.Controllers.Api
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MascotaController : Controller
    {
        private readonly IRepositorioMascota repoMascota;
        private readonly IRepositorioDueno repoDueno;

        public MascotaController(IRepositorioMascota repoMascota, IRepositorioDueno repoDueno)
        {
            this.repoMascota = repoMascota;
            this.repoDueno = repoDueno;
        }

        // GET: api/mascota
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var emailUsuario = User.Identity.Name;
                var dueno = repoDueno.ObtenerPorEmail(emailUsuario);

                if (dueno == null)
                    return NotFound(new { mensaje = "Dueño no encontrado" });

                var mascotas = repoMascota.ObtenerPorDuenoo(dueno.Id);
                return Ok(mascotas);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new { mensaje = "Error al obtener las mascotas", detalle = ex.Message }
                );
            }
        }
    }
}
