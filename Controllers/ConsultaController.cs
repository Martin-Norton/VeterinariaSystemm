using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VeterinariaSystem.Models;

public class ConsultaController : Controller
{
    private readonly IRepositorioConsulta repo;
    private readonly IRepositorioTurno repoTurno;
    private readonly IRepositorioMascota repoMascota;
    private readonly IRepositorioVeterinario repoVeterinario;
    private readonly IRepositorioDueno repoDueno;
    private readonly IWebHostEnvironment environment;

    public ConsultaController(
        IRepositorioConsulta repo,
        IRepositorioTurno repoTurno,
        IRepositorioMascota repoMascota,
        IRepositorioVeterinario repoVeterinario,
        IRepositorioDueno repoDueno,
        IWebHostEnvironment environment
    )
    {
        this.repo = repo;
        this.repoTurno = repoTurno;
        this.repoMascota = repoMascota;
        this.repoVeterinario = repoVeterinario;
        this.repoDueno = repoDueno;
        this.environment = environment;
    }

    [Authorize(Roles = "Veterinario, Administrador")]
    public IActionResult Index(int pagina = 1)
    {
        int tamaño = 5;
        var consultas = repo.ObtenerPaginadas(pagina, tamaño);

        // foreach (var consulta in consultas)
        // {
        //     consulta.Mascota = repoMascota.ObtenerPorId(consulta.Id_Mascota);
        //     consulta.Veterinario = repoVeterinario.ObtenerPorId(consulta.Id_Veterinario);
        // }
        foreach (var consulta in consultas)
        {
            consulta.Mascota =
                repoMascota.ObtenerPorId(consulta.Id_Mascota)
                ?? new Mascota { Nombre = "Desconocida" };
            consulta.Veterinario =
                repoVeterinario.ObtenerPorId(consulta.Id_Veterinario)
                ?? new Veterinario { Nombre = "Desconocido", Apellido = "" };
        }

        int total = repo.ObtenerCantidad();
        ViewBag.Pagina = pagina;
        ViewBag.TotalPaginas = (int)Math.Ceiling((double)total / tamaño);

        return View(consultas);
    }

    [Authorize(Roles = "Veterinario, Administrador")]
    public IActionResult Detalles(int id)
    {
        var consulta = repo.ObtenerPorId(id);
        if (consulta == null)
        {
            return NotFound();
        }
        consulta.Mascota = repoMascota.ObtenerPorId(consulta.Id_Mascota);
        consulta.Veterinario = repoVeterinario.ObtenerPorId(consulta.Id_Veterinario);
        return View(consulta);
    }

    [Authorize(Roles = "Veterinario, Administrador")]
    public IActionResult Editar(int id)
    {
        var consulta = repo.ObtenerPorId(id);
        if (consulta == null)
        {
            return NotFound();
        }
        consulta.Mascota = repoMascota.ObtenerPorId(consulta.Id_Mascota);
        consulta.Veterinario = repoVeterinario.ObtenerPorId(consulta.Id_Veterinario);
        return View(consulta);
    }

    [Authorize(Roles = "Veterinario, Administrador")]
    [HttpPost]
    public IActionResult Editar(int id, Consulta consulta, IFormFile? ArchivoNuevo)
    {
        if (id != consulta.Id)
        {
            return NotFound();
        }
        ModelState.Remove("Mascota");
        ModelState.Remove("Veterinario");

        if (ModelState.IsValid)
        {
            try
            {
                if (ArchivoNuevo != null && ArchivoNuevo.Length > 0)
                {
                    var nombreArchivo =
                        Guid.NewGuid().ToString() + Path.GetExtension(ArchivoNuevo.FileName);
                    var ruta = Path.Combine(environment.WebRootPath, "archivos", nombreArchivo);

                    using (var stream = new FileStream(ruta, FileMode.Create))
                    {
                        ArchivoNuevo.CopyTo(stream);
                    }

                    consulta.ArchivoAdjunto = nombreArchivo;

                    var consultaAnterior = repo.ObtenerPorId(id);
                    if (!string.IsNullOrEmpty(consultaAnterior.ArchivoAdjunto))
                    {
                        var rutaAnterior = Path.Combine(
                            environment.WebRootPath,
                            "archivos",
                            consultaAnterior.ArchivoAdjunto
                        );
                        if (System.IO.File.Exists(rutaAnterior))
                        {
                            System.IO.File.Delete(rutaAnterior);
                        }
                    }
                }
                else
                {
                    var consultaExistente = repo.ObtenerPorId(id);
                    consulta.ArchivoAdjunto = consultaExistente?.ArchivoAdjunto;
                }

                repo.Modificacion(consulta);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al guardar la consulta: " + ex.Message);
            }
        }

        consulta.Mascota = repoMascota.ObtenerPorId(consulta.Id_Mascota);
        consulta.Veterinario = repoVeterinario.ObtenerPorId(consulta.Id_Veterinario);
        return View(consulta);
    }

    [Authorize(Roles = "Veterinario, Administrador")]
    public IActionResult Crear()
    {
        var veterinarios = repoVeterinario
            .ObtenerTodos()
            .Select(v => new { Id = v.Id, NombreCompleto = v.Nombre + " " + v.Apellido })
            .ToList();
        ViewBag.Veterinarios = new SelectList(veterinarios, "Id", "NombreCompleto");
        var duenos = repoDueno
            .ObtenerTodos()
            .Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Nombre + " " + d.Apellido,
            })
            .ToList();
        ViewBag.Duenos = new SelectList(duenos, "Value", "Text");

        ViewBag.Mascotas = new SelectList(new List<Mascota>(), "Id", "Nombre");

        return View();
    }

    public IActionResult ObtenerMascotasPorDueno(int idDueno)
    {
        var mascotas = repoMascota.ObtenerPorDuenoo(idDueno);

        var resultado = mascotas.Select(m => new { id = m.Id, nombre = m.Nombre });

        return Json(resultado);
    }

    [HttpPost]
    [Authorize(Roles = "Veterinario, Administrador")]
    [ValidateAntiForgeryToken]
    public IActionResult Crear(Consulta consulta, IFormFile? Archivo)
    {
        ModelState.Remove("Id_Dueno");
        ModelState.Remove("Mascota");
        ModelState.Remove("Veterinario");

        if (ModelState.IsValid)
        {
            if (Archivo != null && Archivo.Length > 0)
            {
                var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(Archivo.FileName);
                var rutaCarpeta = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "archivos"
                );
                var rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);

                if (!Directory.Exists(rutaCarpeta))
                {
                    Directory.CreateDirectory(rutaCarpeta);
                }

                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    Archivo.CopyTo(stream);
                }

                consulta.ArchivoAdjunto = "/archivos/" + nombreArchivo;
            }
            repo.Alta(consulta);

            return RedirectToAction("Index");
        }
        var veterinarios = repoVeterinario
            .ObtenerTodos()
            .Select(v => new { Id = v.Id, NombreCompleto = v.Nombre + " " + v.Apellido })
            .ToList();
        ViewBag.Veterinarios = new SelectList(veterinarios, "Id", "NombreCompleto");
        var duenos = repoDueno
            .ObtenerTodos()
            .Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Nombre + " " + d.Apellido,
            })
            .ToList();
        ViewBag.Duenos = new SelectList(duenos, "Value", "Text");

        ViewBag.Mascotas = new SelectList(new List<Mascota>(), "Id", "Nombre");

        return View(consulta.Id_Turno != null ? "CrearDesdeTurno" : "Crear", consulta);
    }

    [Authorize(Roles = "Veterinario, Administrador")]
    public IActionResult SeleccionarTurno()
    {
        return View();
    }

    [Authorize(Roles = "Veterinario, Administrador")]
    [HttpPost]
    public IActionResult SeleccionarTurno(bool crearDesdeCero)
    {
        if (crearDesdeCero)
        {
            return RedirectToAction("Crear");
        }
        else
        {
            return RedirectToAction("SeleccionarTurnoExistente");
        }
    }

    [HttpGet]
    [Authorize(Roles = "Veterinario, Administrador")]
    public IActionResult SeleccionarTurnoExistente()
    {
        ViewBag.Duenos = new SelectList(
            repoDueno
                .ObtenerTodos()
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Nombre + " " + d.Apellido,
                })
                .ToList(),
            "Value",
            "Text"
        );

        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Veterinario, Administrador")]
    public IActionResult SeleccionarTurnoExistente(
        int? Id_Dueno,
        int? idMascota,
        int? idTurno,
        string action
    )
    {
        ViewBag.Duenos = new SelectList(
            repoDueno
                .ObtenerTodos()
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Nombre + " " + d.Apellido,
                })
                .ToList(),
            "Value",
            "Text",
            Id_Dueno?.ToString()
        );

        if (Id_Dueno.HasValue)
        {
            var mascotas = repoMascota.ObtenerPorDuenoo(Id_Dueno.Value);
            ViewBag.Mascotas = new SelectList(mascotas, "Id", "Nombre", idMascota);

            if (idMascota.HasValue)
            {
                var turnos = repoTurno
                    .ObtenerPorMascota(idMascota.Value)
                    .Select(t => new SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = t.Fecha.ToString("dd/MM/yyyy HH:mm"),
                    })
                    .ToList();

                ViewBag.Turnos = new SelectList(turnos, "Value", "Text", idTurno?.ToString());

                if (action == "confirmar" && idTurno.HasValue)
                {
                    return RedirectToAction("CrearDesdeTurno", new { idTurno = idTurno.Value });
                }
            }
        }

        return View();
    }

    [HttpGet]
    [Authorize(Roles = "Veterinario, Administrador")]
    public IActionResult CrearDesdeTurno(int idTurno)
    {
        var turno = repoTurno.ObtenerPorId(idTurno);
        if (turno == null)
        {
            return NotFound();
        }

        var consulta = new Consulta
        {
            Id_Turno = turno.Id,
            Id_Mascota = turno.Id_Mascota.Value,
            Fecha = turno.Fecha,
            Motivo = turno.Motivo,
        };

        var veterinarios = repoVeterinario
            .ObtenerTodos()
            .Select(v => new { Id = v.Id, NombreCompleto = v.Nombre + " " + v.Apellido })
            .ToList();
        ViewBag.Veterinarios = new SelectList(
            veterinarios,
            "Id",
            "NombreCompleto",
            consulta.Id_Veterinario
        );

        return View("CrearDesdeTurno", consulta);
    }

    //Zona Busquedas
    [Authorize(Roles = "Veterinario, Administrador")]
    public IActionResult Busquedas()
    {
        return View();
    }

    public IActionResult BuscarPorMascota(int? idDueno, int? idMascota, int pagina = 1)
    {
        var duenos = repoDueno
            .ObtenerTodos()
            .Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Nombre + " " + d.Apellido,
            })
            .ToList();
        ViewBag.Duenos = new SelectList(duenos, "Value", "Text");

        if (idDueno.HasValue)
        {
            var mascotas = repoMascota
                .ObtenerPorDuenoo(idDueno.Value)
                .Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Nombre })
                .ToList();
            ViewBag.Mascotas = new SelectList(mascotas, "Value", "Text");
        }
        else
        {
            ViewBag.Mascotas = new SelectList(new List<SelectListItem>());
        }

        if (!idMascota.HasValue)
        {
            ViewBag.Pagina = 1;
            ViewBag.TotalPaginas = 0;
            return View(new List<Consulta>());
        }

        int tamaño = 5;
        var consultas = repo.BuscarPorMascotaPaginado(idMascota.Value, pagina, tamaño);
        foreach (var consulta in consultas)
        {
            consulta.Mascota = repoMascota.ObtenerPorId(consulta.Id_Mascota);
            consulta.Veterinario = repoVeterinario.ObtenerPorId(consulta.Id_Veterinario);
        }

        int total = repo.ObtenerCantidadPorMascota(idMascota.Value);
        ViewBag.Pagina = pagina;
        ViewBag.TotalPaginas = (int)Math.Ceiling((double)total / tamaño);
        ViewBag.IdDuenoSeleccionado = idDueno;
        ViewBag.IdMascotaSeleccionada = idMascota;

        return View(consultas);
    }

    [Authorize(Roles = "Veterinario, Administrador")]
    public IActionResult BuscarPorVeterinario(int? idVeterinario, int pagina = 1)
    {
        int tamaño = 5;
        ViewBag.Veterinarios = repoVeterinario
            .ObtenerTodos()
            .Select(v => new SelectListItem { Value = v.Id.ToString(), Text = v.Nombre })
            .ToList();

        if (idVeterinario == null)
        {
            ViewBag.Pagina = 1;
            ViewBag.TotalPaginas = 0;
            return View();
        }

        var consultas = repo.BuscarPorVeterinarioPaginado(idVeterinario.Value, pagina, tamaño);
        foreach (var consulta in consultas)
        {
            consulta.Mascota = repoMascota.ObtenerPorId(consulta.Id_Mascota);
            consulta.Veterinario = repoVeterinario.ObtenerPorId(consulta.Id_Veterinario);
        }

        int total = repo.ObtenerCantidadPorVeterinario(idVeterinario.Value);
        ViewBag.Pagina = pagina;
        ViewBag.TotalPaginas = (int)Math.Ceiling((double)total / tamaño);
        ViewBag.IdVeterinarioSeleccionado = idVeterinario.Value;

        return View(consultas);
    }

    [Authorize(Roles = "Veterinario, Administrador")]
    public IActionResult BuscarPorFechas(DateTime? fechaInicio, DateTime? fechaFin, int pagina = 1)
    {
        int tamaño = 5;

        if (!fechaInicio.HasValue || !fechaFin.HasValue)
        {
            ViewBag.Mensaje = "Por favor, seleccione un rango de fechas.";
            ViewBag.Pagina = 1;
            ViewBag.TotalPaginas = 0;
            return View();
        }

        if (fechaInicio > fechaFin)
        {
            ViewBag.Mensaje = "La fecha de inicio debe ser anterior a la fecha de fin.";
            ViewBag.Pagina = 1;
            ViewBag.TotalPaginas = 0;
            return View();
        }

        var consultas = repo.BuscarPorFechasPaginado(
            fechaInicio.Value,
            fechaFin.Value,
            pagina,
            tamaño
        );
        foreach (var consulta in consultas)
        {
            consulta.Mascota = repoMascota.ObtenerPorId(consulta.Id_Mascota);
            consulta.Veterinario = repoVeterinario.ObtenerPorId(consulta.Id_Veterinario);
        }

        int total = repo.ObtenerCantidadPorFechas(fechaInicio.Value, fechaFin.Value);
        ViewBag.Pagina = pagina;
        ViewBag.TotalPaginas = (int)Math.Ceiling((double)total / tamaño);
        ViewBag.FechaInicio = fechaInicio.Value.ToString("yyyy-MM-dd");
        ViewBag.FechaFin = fechaFin.Value.ToString("yyyy-MM-dd");

        return View(consultas);
    }
    //Fin zona Busquedas
}
