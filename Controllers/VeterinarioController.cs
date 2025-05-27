using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeterinariaSystem.Models;

namespace VeterinariaSystem.Controllers
{
    public class VeterinarioController : Controller
    {
        private readonly IRepositorioVeterinario repo;
        private readonly IWebHostEnvironment environment;

        public VeterinarioController(IWebHostEnvironment environment, IRepositorioVeterinario repo)
        {
            this.environment = environment;
            this.repo = repo;
        }

        // GET: /Veterinario/
        // public IActionResult Index()
        // {
        //     var lista = repo.ObtenerTodos();
        //     return View(lista);
        // }
        public IActionResult Index(int pagina = 1, int cantidadPorPagina = 10)
        {
            var total = repo.ObtenerCantidad();
            var lista = repo.ObtenerTodosPaginado(pagina, cantidadPorPagina);

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)total / cantidadPorPagina);

            return View(lista);
        }

        // GET: /Veterinario/Detalles/5
        public IActionResult Detalles(int id)
        {
            var vet = repo.ObtenerPorId(id);
            if (vet == null)
                return NotFound();
            return View(vet);
        }

        // GET: /Veterinario/Crear
        public IActionResult Crear()
        {
            return View();
        }

        // POST: /Veterinario/Crear
        [HttpPost]
        public IActionResult Crear(Veterinario veterinario)
        {
            if (ModelState.IsValid)
            {
                var existente = repo.ObtenerPorDNI(veterinario.DNI);
                if (existente.Count > 0)
                {
                    ModelState.AddModelError("DNI", "Ya existe un veterinario con ese DNI.");
                    return View(veterinario);
                }
                repo.Alta(veterinario);
                return RedirectToAction(nameof(Index));
            }
            return View(veterinario);
        }

        // GET: /Veterinario/Editar/5
        public IActionResult Editar(int id)
        {
            var vet = repo.ObtenerPorId(id);
            if (vet == null)
                return NotFound();
            return View(vet);
        }

        // POST: /Veterinario/Editar/5
        [HttpPost]
        public IActionResult Editar(int id, Veterinario veterinario)
        {
            if (id != veterinario.Id)
                return NotFound();
            if (ModelState.IsValid)
            {
                repo.Modificacion(veterinario);
                return RedirectToAction(nameof(Index));
            }
            return View(veterinario);
        }

        // GET: /Veterinario/Eliminar/5
        public IActionResult Eliminar(int id)
        {
            var vet = repo.ObtenerPorId(id);
            if (vet == null)
                return NotFound();
            return View(vet);
        }

        // POST: /Veterinario/EliminarConfirmado/5
        [HttpPost]
        public IActionResult EliminarConfirmado(int id)
        {
            repo.Baja(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Veterinario/BuscarPorDni
        public IActionResult BuscarPorDni()
        {
            return View();
        }

        // POST: /Veterinario/BuscarPorDni
        [HttpPost]
        public IActionResult BuscarPorDni(int dni)
        {
            TempData["MensajeBusqueda"] = $"Mostrando resultados para DNI: {dni}";
            var lista = repo.ObtenerPorDNI(dni);
            return View("Index", lista);
        }

        // GET: /Veterinario/BuscarPorMatricula
        public IActionResult BuscarPorMatricula()
        {
            return View();
        }

        // POST: /Veterinario/BuscarPorMatricula
        [HttpPost]
        public IActionResult BuscarPorMatricula(string matricula)
        {
            var lista = repo.ObtenerPorMatricula(matricula);

            TempData["MensajeBusqueda"] = $"Mostrando resultados para Matrícula: {matricula}";
            return View("Index", lista);
        }
    }
}
