using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using VeterinariaSystem.Models;

namespace VeterinariaSystem.Controllers
{
    public class TurnoController : Controller
    {
        private readonly IRepositorioTurno repoTurno;
        private readonly IRepositorioMascota repoMascota;
        private readonly IRepositorioDueno repoDueno;

        public TurnoController(
            IRepositorioTurno repoTurno,
            IRepositorioMascota repoMascota,
            IRepositorioDueno repoDueno
        )
        {
            this.repoTurno = repoTurno;
            this.repoMascota = repoMascota;
            this.repoDueno = repoDueno;
        }

        [Authorize(Roles = "Veterinario, Administrador")]
        public IActionResult Index(int pagina = 1)
        {
            int tamaño = 5;
            var turnos = repoTurno.ObtenerPaginadas(pagina, tamaño);
            int total = repoTurno.ObtenerCantidad();
            ViewBag.Pagina = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)total / tamaño);

            return View("Index", turnos);
        }

        [Authorize(Roles = "Veterinario, Administrador, Dueno")]
        // GET: /Turno/Detalles/5
        public IActionResult Detalles(int id)
        {
            var turno = repoTurno.ObtenerPorId(id);
            if (turno == null)
                return NotFound();
            return View("Detalles", turno);
        }

        [Authorize(Roles = "Dueno, Administrador")]
        // GET: /Turno/Editar/5
        public IActionResult Editar(int id)
        {
            var turno = repoTurno.ObtenerPorId(id);
            if (turno == null)
                return NotFound();

            var mascota = repoMascota.ObtenerPorId(turno.Id_Mascota.Value);
            ViewBag.NombreMascota = mascota?.Nombre;
            ViewBag.NombreMascota = repoMascota.ObtenerPorId(turno.Id_Mascota.Value)?.Nombre;

            return View("Editar", turno);
        }

        [Authorize(Roles = "Dueno, Administrador")]
        // POST: /Turno/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(int id, Turno turno)
        {
            if (id != turno.Id)
                return NotFound();
            ModelState.Remove("Mascota");
            if (!ModelState.IsValid)
            {
                var mascota = repoMascota.ObtenerPorId(turno.Id_Mascota.Value);
                ViewBag.NombreMascota = mascota?.Nombre;
                ViewBag.NombreMascota = repoMascota.ObtenerPorId(turno.Id_Mascota.Value)?.Nombre;
                return View("Editar", turno);
            }

            var turnoExistente = repoTurno.ObtenerPorId(id);
            if (turnoExistente == null)
                return NotFound();

            if (turno.Fecha != turnoExistente.Fecha || turno.Hora != turnoExistente.Hora)
            {
                if (repoTurno.ExisteTurnoEnHorario(turno.Fecha, turno.Hora))
                    ModelState.AddModelError("", "Ya existe un turno en ese horario.");
            }

            if (
                turno.Fecha != turnoExistente.Fecha
                || turno.Id_Mascota != turnoExistente.Id_Mascota
            )
            {
                if (repoTurno.ExisteTurnoParaMascotaEnDia(turno.Id_Mascota.Value, turno.Fecha))
                    ModelState.AddModelError("", "La mascota ya tiene un turno ese día.");
            }

            ModelState.Remove("Mascota");
            if (!ModelState.IsValid)
            {
                var mascota = repoMascota.ObtenerPorId(turno.Id_Mascota.Value);
                ViewBag.NombreMascota = mascota?.Nombre;
                ViewBag.NombreMascota = repoMascota.ObtenerPorId(turno.Id_Mascota.Value)?.Nombre;
                return View("Editar", turno);
            }

            repoTurno.Modificacion(turno);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Turno/Eliminar/5
        [Authorize(Roles = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var turno = repoTurno.ObtenerPorId(id);
            if (turno == null)
                return NotFound();
            return View("Eliminar", turno);
        }

        [Authorize(Roles = "Administrador")]
        // POST: /Turno/Eliminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarConfirmado(int id)
        {
            repoTurno.Baja(id);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Veterinario, Administrador")]
        // GET: /Turno/BuscarPorFecha
        public IActionResult BuscarPorFecha(DateTime? fecha, int pagina = 1)
        {
            int tamaño = 5;

            if (fecha == null)
            {
                ViewBag.Mensaje = "Debe seleccionar una fecha.";
                ViewBag.Pagina = pagina;
                ViewBag.TotalPaginas = 0;
                return View("BuscarPorFecha", new List<Turno>());
            }

            var turnos = repoTurno.ObtenerPorFechaPaginado(fecha.Value, pagina, tamaño);
            int total = repoTurno.ObtenerCantidadPorFecha(fecha.Value);

            ViewBag.Mensaje = $"Mostrando turnos del día {fecha.Value:dd/MM/yyyy}";
            ViewBag.Pagina = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)total / tamaño);

            return View("BuscarPorFecha", turnos);
        }

        [Authorize(Roles = "Veterinario, Administrador, Dueno")]
        public IActionResult BuscarPorMascota(int? idMascota, int pagina = 1)
        {
            var mascotas = repoMascota.ObtenerTodos();
            ;
            int tamaño = 5;
            if (User.IsInRole("Dueno"))
            {
                var email = User.Identity.Name;
                var dueno = repoDueno.ObtenerPorEmail(email);
                mascotas = repoMascota.ObtenerPorDuenoo(dueno.Id);
            }
            else
            {
                mascotas = repoMascota.ObtenerTodos();
            }

            ViewBag.Mascotas = mascotas
                .Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Nombre })
                .ToList();

            if (idMascota == null)
            {
                ViewBag.Pagina = pagina;
                ViewBag.TotalPaginas = 0;
                return View("BuscarPorMascota", new List<Turno>());
            }

            var turnos = repoTurno.ObtenerPorMascotaPaginado(idMascota.Value, pagina, tamaño);
            int total = repoTurno.ObtenerCantidadPorMascota(idMascota.Value);

            if (turnos != null && turnos.Any())
            {
                ViewBag.Mensaje = $"Mostrando turnos para la mascota con ID {idMascota.Value}.";
            }
            else
            {
                ViewBag.Mensaje = $"No hay turnos para la mascota con ID {idMascota.Value}.";
            }

            ViewBag.Pagina = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)total / tamaño);

            return View("BuscarPorMascota", turnos);
        }

        public IActionResult ObtenerPorMascota(int? idMascota, int pagina = 1)
        {
            int tamaño = 5;
            var mascotas = repoMascota.ObtenerTodos();
            ViewBag.Mascotas = mascotas
                .Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Nombre })
                .ToList();

            if (idMascota == null)
            {
                ViewBag.Pagina = pagina;
                ViewBag.TotalPaginas = 0;
                return View("TodosLosTurnosPorMascota", new List<Turno>());
            }

            var turnos = repoTurno.ObtenerPorMascotaPaginado(idMascota.Value, pagina, tamaño);
            int total = repoTurno.ObtenerCantidadPorMascota(idMascota.Value);

            if (turnos != null && turnos.Any())
            {
                ViewBag.Mensaje =
                    $"Mostrando todos los turnos para la mascota con ID {idMascota.Value}.";
            }
            else
            {
                ViewBag.Mensaje = $"No hay turnos para la mascota con ID {idMascota.Value}.";
                turnos = new List<Turno>();
            }

            ViewBag.Pagina = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)total / tamaño);

            return View("TodosLosTurnosPorMascota", turnos);
        }

        //Fin Zona Busquedas
        //Zona Veterinario y Administrador
        [HttpGet]
        [Authorize(Roles = "Administrador, Veterinario")]
        public IActionResult ObtenerPorDuenoo(int idDueno)
        {
            var mascotas = repoMascota
                .ObtenerPorDuenoo(idDueno)
                .Select(m => new { id = m.Id, nombre = m.Nombre });
            return Json(mascotas);
        }

        [Authorize(Roles = "Veterinario, Administrador, Dueno")]
        public IActionResult Crear()
        {
            if (User.IsInRole("Dueno"))
            {
                var email = User.Identity.Name;
                var dueno = repoDueno.ObtenerPorEmail(email);

                if (dueno != null)
                {
                    var mascotas = repoMascota.ObtenerPorDuenoo(dueno.Id);
                    ViewBag.Mascotas = new SelectList(mascotas, "Id", "Nombre");
                }
                else
                {
                    ViewBag.Mascotas = new SelectList(Enumerable.Empty<Mascota>(), "Id", "Nombre");
                }
            }
            else
            {
                ViewBag.Duenos = new SelectList(repoDueno.ObtenerTodos(), "Id", "Nombre");
                ViewBag.Mascotas = new SelectList(Enumerable.Empty<Mascota>(), "Id", "Nombre");
            }

            return View("Crear");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Veterinario, Administrador, Dueno")]
        public IActionResult Crear(Turno turno)
        {
            ModelState.Remove("Mascota");

            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Error en el campo '{state.Key}': {error.ErrorMessage}");
                    }
                }
                ViewBag.Duenos = new SelectList(repoDueno.ObtenerTodos(), "Id", "Nombre");
                return View(turno);
            }

            if (repoTurno.ExisteTurnoEnHorario(turno.Fecha, turno.Hora))
            {
                ModelState.AddModelError("", "Ya existe un turno en ese horario.");
                TempData["Error"] = "Turno ya ocupado.";
            }
            else if (repoTurno.ExisteTurnoParaMascotaEnDia(turno.Id_Mascota.Value, turno.Fecha))
            {
                ModelState.AddModelError("", "La mascota ya tiene un turno ese día.");
                TempData["Error"] = "Turno duplicado.";
            }
            else
            {
                repoTurno.Alta(turno);
                if(User.IsInRole("Dueno"))
                {
                    return RedirectToAction("MisMascotas", "Mascota");
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewBag.Duenos = new SelectList(repoDueno.ObtenerTodos(), "Id", "Nombre");
            return View(turno);
        }
        //Fin Zona veterinario y administrador
    }
}
