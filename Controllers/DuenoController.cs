using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeterinariaSystem.Models;

namespace VeterinariaSystem.Controllers
{
    public class DuenoController : Controller
    {
        private readonly IRepositorioDueno repositorio;
        private readonly IWebHostEnvironment environment;

        public DuenoController(IWebHostEnvironment environment, IRepositorioDueno repo)
        {
            this.environment = environment;
            this.repositorio = repo;
        }

        [Authorize(Roles = "Veterinario, Administrador")]
        public IActionResult Index()
        {
            // No hace falta pasar lista ni ViewBags, la vista se alimenta con Vue.js, por eso solo devuelve la view
            return View(); 
        }

        // GET: /Dueno/Detalles/5
        [Authorize(Roles = "Veterinario, Administrador")]
        public IActionResult Detalles(int id)
        {
            ViewBag.IdDueno = id;
            return View();
        }

        // GET: /Dueno/Crear
        [Authorize(Roles = "Veterinario, Administrador")]
        public IActionResult Crear()
        {
            return View();
        }

        // POST: /Dueno/Crear
        [HttpPost]
        public IActionResult Crear(Dueno dueno)
        {
            if (ModelState.IsValid)
            {
                var existente = repositorio.ObtenerPorDni(dueno.DNI);
                if (existente != null)
                {
                    ModelState.AddModelError("DNI", "Ya existe un dueño con ese DNI.");
                    return View(dueno);
                }
                repositorio.Alta(dueno);
                return RedirectToAction(nameof(Index));
            }
            return View(dueno);
        }

        [Authorize(Roles = "Administrador")]
        // GET: /Dueno/Editar/5
        public IActionResult Editar(int id)
        {
            var dueno = repositorio.ObtenerPorId(id);
            if (dueno == null)
                return NotFound();
            return View(dueno);
        }

        [Authorize(Roles = "Administrador")]
        // POST: /Dueno/Editar/5
        [HttpPost]
        public IActionResult Editar(int id, Dueno dueno)
        {
            if (id != dueno.Id)
                return NotFound();
            if (ModelState.IsValid)
            {
                repositorio.Modificacion(dueno);
                return RedirectToAction(nameof(Index));
            }
            return View(dueno);
        }

        [Authorize(Roles = "Administrador")]
        // GET: /Dueno/Eliminar/5
        public IActionResult Eliminar(int id)
        {
            var dueno = repositorio.ObtenerPorId(id);
            if (dueno == null)
                return NotFound();
            return View(dueno);
        }

        [Authorize(Roles = "Administrador")]
        // POST: /Dueno/Eliminar/5
        [HttpPost]
        public IActionResult EliminarConfirmado(int id)
        {
            repositorio.Baja(id);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrador,Veterinario")]
        [HttpGet]
        public IActionResult Buscar()
        {
            return View("Buscar");
        }
//se comenta pq ahora lo hace con axios del lado del cliente
        // [Authorize(Roles = "Administrador,Veterinario")]
        // [HttpPost]
        // public IActionResult Buscar(int dni)
        // {
        //     var dueno = repositorio.ObtenerPorDni(dni);
        //     if (dueno == null)
        //     {
        //         ViewBag.Mensaje = "No se encontró ningún dueño con ese DNI.";
        //         return View("Buscar");
        //     }
        //     return View("Buscar", dueno);
        // }
    }
}
