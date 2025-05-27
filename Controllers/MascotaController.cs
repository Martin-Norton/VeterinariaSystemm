using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeterinariaSystem.Models;

namespace VeterinariaSystem.Controllers
{
    public class MascotaController : Controller
    {
        private readonly IRepositorioMascota repositorio;
        private readonly IRepositorioDueno repoDueno;
        private readonly IRepositorioConsulta repoConsulta;
        private readonly IRepositorioUsuario repoUsuario;
        private readonly IWebHostEnvironment environment;

        public MascotaController(
            IWebHostEnvironment environment,
            IRepositorioMascota repo,
            IRepositorioDueno repoDueno,
            IRepositorioConsulta repoConsulta,
            IRepositorioUsuario repoUsuario
        )
        {
            this.environment = environment;
            this.repositorio = repo;
            this.repoDueno = repoDueno;
            this.repoConsulta = repoConsulta;
            this.repoUsuario = repoUsuario;
        }

        [Authorize(Roles = "Veterinario, Administrador")]
        public IActionResult Index(int pagina = 1)
        {
            int tamañoPagina = 10;
            int totalMascotas = repositorio.ObtenerCantidadMascotasActivas();
            var mascotas = repositorio.ObtenerTodosPaginado(pagina, tamañoPagina);

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalMascotas / tamañoPagina);

            return View(mascotas);
        }

        [Authorize(Roles = "Veterinario, Administrador, Dueno")]
        // GET: /Mascota/Detalles/5
        public IActionResult Detalles(int id)
        {
            var mascota = repositorio.ObtenerPorId(id);
            if (mascota == null)
                return NotFound();

            return View("Detalles", mascota);
        }

        [Authorize(Roles = "Veterinario, Administrador")]
        // GET: /Mascota/Crear
        public IActionResult Crear()
        {
            ViewBag.Duenos = repoDueno.ObtenerTodos();
            return View("Crear");
        }

        [Authorize(Roles = "Veterinario, Administrador")]
        // POST: /Mascota/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Mascota mascota)
        {
            ModelState.Remove("Dueno");
            if (!ModelState.IsValid)
            {
                ViewBag.Duenos = repoDueno.ObtenerTodos();
                return View("Crear", mascota);
            }
            repositorio.Alta(mascota);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Veterinario, Administrador, Dueno")]
        //GET: /Mascota/Editar/5
        public IActionResult Editar(int id)
        {
            var mascota = repositorio.ObtenerPorId(id);
            if (mascota == null)
                return NotFound();
            return View("Editar", mascota);
        }

        [Authorize(Roles = "Veterinario, Administrador, Dueno")]
        // POST: /Mascota/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(int id, Mascota mascota)
        {
            if (id != mascota.Id)
                return NotFound();
            ModelState.Remove("Dueno");
            if (!ModelState.IsValid)
            {
                ViewBag.Duenos = repoDueno.ObtenerTodos();
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Error en el campo '{state.Key}': {error.ErrorMessage}");
                    }
                }
                return View("Editar", mascota);
            }
            repositorio.Modificacion(mascota);
            var email = User.Identity.Name;
            var dueno = repoDueno.ObtenerPorEmail(email);
            if (User.IsInRole("Dueno"))
            {
                return RedirectToAction(nameof(MisMascotas));
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Roles = "Administrador")]
        // GET: /Mascota/Eliminar/5
        public IActionResult Eliminar(int id)
        {
            var mascota = repositorio.ObtenerPorId(id);
            if (mascota == null)
                return NotFound();

            return View("Eliminar", mascota);
        }

        [Authorize(Roles = "Administrador")]
        // POST: /Mascota/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarConfirmado(int id)
        {
            repositorio.Baja(id);
            return RedirectToAction(nameof(Index));
        }

        //ZonaBusquedas
        [Authorize(Roles = "Veterinario, Administrador")]
        [HttpGet]
        public IActionResult BuscarPorDueno()
        {
            var listaDuenos = repoDueno.ObtenerTodos();
            ViewBag.Duenos = listaDuenos;
            return View();
        }

        [Authorize(Roles = "Veterinario, Administrador")]
        [HttpPost]
        public IActionResult BuscarPorDueno(int idDueno, int pagina = 1)
        {
            int tamañoPagina = 5;
            var listaDuenos = repoDueno.ObtenerTodos();
            ViewBag.Duenos = listaDuenos;
            var totalMascotas = repositorio.ObtenerCantidadPorDueno(idDueno);
            var mascotas = repositorio.ObtenerPorDuenoPaginado(idDueno, pagina, tamañoPagina);

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalMascotas / tamañoPagina);
            return View(mascotas);
        }

        //FinZonaBusquedas
        // Zona Dueno
        [Authorize(Roles = "Dueno")]
        public IActionResult MisMascotas(int pagina = 1)
        {
            int tamañoPagina = 5;
            var email = User.Identity.Name;
            var dueno = repoDueno.ObtenerPorEmail(email);

            var totalMascotas = repositorio.ObtenerCantidadPorDueno(dueno.Id);
            var mascotas = repositorio.ObtenerPorDuenoPaginado(dueno.Id, pagina, tamañoPagina);

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalMascotas / tamañoPagina);

            return View("MisMascotas", mascotas);
        }

        [Authorize(Roles = "Dueno")]
        public IActionResult CrearParaDueno()
        {
            return View("CrearParaDueno");
        }

        [Authorize(Roles = "Dueno")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearParaDueno(Mascota mascota)
        {
            ModelState.Remove("Dueno");
            if (!ModelState.IsValid)
            {
                return View("CrearParaDueno", mascota);
            }
            var email = User.Identity.Name;
            var dueno = repoDueno.ObtenerPorEmail(email);
            mascota.Id_Dueno = dueno.Id;
            mascota.Estado = 1;
            repositorio.Alta(mascota);
            return RedirectToAction(nameof(MisMascotas));
        }

        [Authorize(Roles = "Dueno")]
        public IActionResult HistoriaClinica(int id, int pagina = 1)
        {
            int tamanoPagina = 5;
            var mascota = repositorio.ObtenerPorId(id);
            var email = User.Identity.Name;
            var dueno = repoDueno.ObtenerPorEmail(email);
            var userId = dueno.Id;

            if (mascota == null || mascota.Id_Dueno != userId)
            {
                return Unauthorized();
            }

            int total;
            var historia = repoConsulta.ObtenerHistoriaClinica(id, pagina, tamanoPagina, out total);

            ViewBag.Mascota = mascota;
            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)total / tamanoPagina);
            ViewBag.IdMascota = id;

            return View("HistoriaClinica", historia);
        }

        //Fin Zona Dueno
    }
}
