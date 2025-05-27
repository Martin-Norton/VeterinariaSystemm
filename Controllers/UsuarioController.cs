using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using VeterinariaSystem.Models;

namespace VeterinariaSystem.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment environment;
        private readonly IRepositorioUsuario repositorio;
        private readonly IRepositorioVeterinario repoVeterinario;
        private readonly IRepositorioDueno repoDueno;

        public UsuariosController(
            IConfiguration configuration,
            IWebHostEnvironment environment,
            IRepositorioUsuario repositorio,
            IRepositorioVeterinario repoVeterinario,
            IRepositorioDueno repoDueno
        )
        {
            this.configuration = configuration;
            this.environment = environment;
            this.repositorio = repositorio;
            this.repoVeterinario = repoVeterinario;
            this.repoDueno = repoDueno;
        }

        // [Authorize(Policy = "Administrador")]
        // public ActionResult Index()
        // {
        //     var usuarios = repositorio.ObtenerTodos();
        //     return View(usuarios);
        // }
        [Authorize(Policy = "Administrador")]
        public ActionResult Index(int pagina = 1)
        {
            int tamaño = 5;
            var usuarios = repositorio.ObtenerTodosPaginado(pagina, tamaño);
            int total = repositorio.ObtenerCantidad();

            ViewBag.Pagina = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)total / tamaño);

            return View(usuarios);
        }

        // GET: Usuarios/Details/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Details(int id)
        {
            var e = repositorio.ObtenerPorId(id);
            if (e == null)
                return NotFound();
            return View(e);
        }

        // GET: Usuarios/Create
        [Authorize(Policy = "Administrador")]
        public ActionResult Create()
        {
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Create(Usuario u)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = Usuario.ObtenerRoles();
                return View(u);
            }
            try
            {
                string hashed = Convert.ToBase64String(
                    KeyDerivation.Pbkdf2(
                        password: u.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 10000,
                        numBytesRequested: 256 / 8
                    )
                );
                u.Clave = hashed;
                int res = repositorio.Alta(u);

                if (u.AvatarFile != null && u.Id > 0)
                {
                    string wwwPath = environment.WebRootPath;
                    string path = Path.Combine(wwwPath, "Uploads", "Avatars");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    string fileName = "avatar_" + u.Id + Path.GetExtension(u.AvatarFile.FileName);
                    string pathCompleto = Path.Combine(path, fileName);
                    u.Avatar = Path.Combine("/Uploads/Avatars", fileName);

                    using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                    {
                        u.AvatarFile.CopyTo(stream);
                    }
                    repositorio.Modificacion(u);
                }

                TempData["Mensaje"] = "Usuario creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear el usuario: " + ex.Message);
                ViewBag.Roles = Usuario.ObtenerRoles();
                return View(u);
            }
        }

        [Authorize(Policy = "Administrador")]
        public ActionResult Edit(int id)
        {
            ViewData["Title"] = "Editar usuario";
            var u = repositorio.ObtenerPorId(id);
            if (u == null)
                return NotFound();
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View(u);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Edit(int id, Usuario u)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = Usuario.ObtenerRoles();
                return View(u);
            }

            try
            {
                Usuario? userActual = repositorio.ObtenerPorId(id);
                if (userActual == null)
                    return NotFound();

                userActual.Nombre = u.Nombre;
                userActual.Apellido = u.Apellido;
                userActual.Email = u.Email;
                userActual.Rol = u.Rol;
                userActual.Avatar = u.Avatar;

                repositorio.Modificacion(userActual);
                TempData["Mensaje"] = "Usuario actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al editar usuario: " + ex.Message);
                ViewBag.Roles = Usuario.ObtenerRoles();
                return View(u);
            }
        }

        [Authorize]
        public ActionResult Perfil()
        {
            ViewData["Title"] = "Mi perfil";
            var u = repositorio.ObtenerPorEmail(User.Identity.Name);
            if (u == null)
                return NotFound();
            return View("Edit", u);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Perfil(Usuario u)
        {
            var usuarioActual = repositorio.ObtenerPorEmail(User.Identity.Name);
            if (usuarioActual == null)
                return Challenge();
            if (usuarioActual.Id != u.Id)
                return Forbid();

            if (ModelState.IsValid)
            {
                try
                {
                    usuarioActual.Nombre = u.Nombre;
                    usuarioActual.Apellido = u.Apellido;
                    usuarioActual.Email = u.Email;
                    if (!User.IsInRole(nameof(enRoles.Administrador)))
                    {
                        u.Rol = usuarioActual.Rol;
                    }
                    usuarioActual.Rol = u.Rol;
                    if (u.AvatarFile != null && u.Id > 0)
                    {
                        string wwwPath = environment.WebRootPath;
                        string path = Path.Combine(wwwPath, "Uploads", "Avatars");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        string fileName =
                            "avatar_" + u.Id + Path.GetExtension(u.AvatarFile.FileName);
                        string pathCompleto = Path.Combine(path, fileName);
                        u.Avatar = Path.Combine("/Uploads/Avatars", fileName);

                        using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                        {
                            u.AvatarFile.CopyTo(stream);
                        }
                        usuarioActual.Avatar = u.Avatar;
                    }
                    if (
                        Request.Form["EliminarAvatar"] == "on"
                        && !string.IsNullOrEmpty(usuarioActual.Avatar)
                    )
                    {
                        string avatarPath = Path.Combine(
                            environment.WebRootPath,
                            usuarioActual.Avatar.TrimStart('/')
                        );
                        if (System.IO.File.Exists(avatarPath))
                        {
                            System.IO.File.Delete(avatarPath);
                        }
                        usuarioActual.Avatar = null;
                    }
                    repositorio.Modificacion(usuarioActual);
                    TempData["Mensaje"] = "Perfil actualizado correctamente.";
                    var updatedUser = repositorio.ObtenerPorId(usuarioActual.Id);
                    return View("Edit", updatedUser);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al actualizar el perfil: " + ex.Message);
                    return View("Edit", usuarioActual);
                }
            }

            return View("Index", u);
        }

        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
            var u = repositorio.ObtenerPorId(id);
            if (u == null)
                return NotFound();
            return View(u);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        [Authorize(Policy = "Administrador")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var usuario = repositorio.ObtenerPorId(id);
                if (usuario == null)
                    return NotFound();

                if (!string.IsNullOrEmpty(usuario.Avatar))
                {
                    try
                    {
                        string wwwPath = environment.WebRootPath;
                        string relativePath = usuario.Avatar.TrimStart('/');
                        string fullPath = Path.Combine(wwwPath, relativePath);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(
                            $"Error deleting avatar file {usuario.Avatar}: {ex.Message}"
                        );
                    }
                }
                repositorio.Baja(id);
                TempData["Mensaje"] = "Usuario eliminado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar el usuario: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [AllowAnonymous]
        public ActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginView login, string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            try
            {
                if (ModelState.IsValid)
                {
                    string hashed = Convert.ToBase64String(
                        KeyDerivation.Pbkdf2(
                            password: login.Clave,
                            salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                            prf: KeyDerivationPrf.HMACSHA1,
                            iterationCount: 10000,
                            numBytesRequested: 256 / 8
                        )
                    );

                    var user = repositorio.ObtenerPorEmail(login.Usuario);

                    if (user == null || user.Clave != hashed)
                    {
                        ModelState.AddModelError("", "El email o la clave no son correctos.");
                        ViewData["ReturnUrl"] = returnUrl;
                        return View(login);
                    }

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Email),
                        new Claim("FullName", user.Nombre + " " + user.Apellido),
                        new Claim(ClaimTypes.Role, user.RolNombre),
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims,
                        CookieAuthenticationDefaults.AuthenticationScheme
                    );

                    var authProperties = new AuthenticationProperties { IsPersistent = true };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties
                    );

                    return LocalRedirect(returnUrl);
                }
                ViewData["ReturnUrl"] = returnUrl;
                return View(login);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocurrió un error inesperado: " + ex.Message);
                ViewData["ReturnUrl"] = returnUrl;
                return View(login);
            }
        }

        [Authorize]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public IActionResult Restringido()
        {
            return View();
        }

        [Authorize]
        public IActionResult Avatar(int id)
        {
            try
            {
                var u = repositorio.ObtenerPorId(id);
                if (u == null || string.IsNullOrEmpty(u.Avatar))
                {
                    return NotFound();
                }
                string wwwPath = environment.WebRootPath;
                string fullPath = Path.Combine(wwwPath, u.Avatar.TrimStart('/'));

                if (!System.IO.File.Exists(fullPath))
                {
                    return NotFound();
                }
                byte[] fileBytes = System.IO.File.ReadAllBytes(fullPath);
                string mimeType = GetMimeType(Path.GetExtension(u.Avatar));
                return File(fileBytes, mimeType);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        private string GetMimeType(string extension)
        {
            switch (extension.ToLower())
            {
                case ".png":
                    return "image/png";
                case ".jpg":
                    return "image/jpg";
                case ".jpeg":
                    return "image/jpeg";
                case ".gif":
                    return "image/gif";
                default:
                    return "application/octet-stream";
            }
        }

        // zona modificar clave
        [Authorize]
        public IActionResult CambiarClave()
        {
            ViewData["Title"] = "Cambiar Contraseña";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult CambiarClave(
            string ClaveActual,
            string NuevaClave,
            string ConfirmarClave
        )
        {
            var usuario = repositorio.ObtenerPorEmail(User.Identity.Name);
            if (usuario == null)
                return Challenge();

            string salt = configuration["Salt"];
            byte[] saltBytes = System.Text.Encoding.ASCII.GetBytes(salt);

            string hashActual = Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password: ClaveActual,
                    salt: saltBytes,
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8
                )
            );

            if (usuario.Clave != hashActual)
            {
                ModelState.AddModelError("", "La contraseña actual no es correcta.");
                return View();
            }

            if (string.IsNullOrEmpty(NuevaClave) || NuevaClave.Length < 6)
            {
                ModelState.AddModelError(
                    "",
                    "La nueva contraseña debe tener al menos 6 caracteres."
                );
                return View();
            }

            if (NuevaClave != ConfirmarClave)
            {
                ModelState.AddModelError("", "La confirmación de la contraseña no coincide.");
                return View();
            }

            string nuevoHash = Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password: NuevaClave,
                    salt: saltBytes,
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8
                )
            );

            usuario.Clave = nuevoHash;
            repositorio.ModificacionConClave(usuario);

            TempData["Mensaje"] = "Contraseña actualizada correctamente.";
            return RedirectToAction("Perfil");
        }

        // Zona veterinario
        [Authorize(Roles = "Administrador")]
        public IActionResult CrearVeterinarioUsuario()
        {
            var disponibles = repoVeterinario.ObtenerVeterinariosSinUsuario();
            ViewBag.Veterinarios = new SelectList(disponibles, "Id", "Apellido");
            return View();
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public IActionResult CrearVeterinarioUsuario(Usuario usuario)
        {
            var disponibles = repoVeterinario.ObtenerVeterinariosSinUsuario();

            if (
                !usuario.VeterinarioId.HasValue
                || !disponibles.Any(v => v.Id == usuario.VeterinarioId)
            )
            {
                ModelState.AddModelError(
                    "VeterinarioId",
                    "Debe seleccionar un veterinario válido."
                );
            }

            if (ModelState.IsValid)
            {
                string hashed = Convert.ToBase64String(
                    KeyDerivation.Pbkdf2(
                        password: usuario.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 10000,
                        numBytesRequested: 256 / 8
                    )
                );
                usuario.Clave = hashed;

                usuario.Rol = (int)enRoles.Veterinario;
                repositorio.Alta(usuario);

                return RedirectToAction("Index");
            }

            ViewBag.Veterinarios = new SelectList(disponibles, "Id", "Apellido");
            return View(usuario);
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public IActionResult ObtenerDatosVeterinario(int id)
        {
            var veterinario = repoVeterinario.ObtenerPorId(id);
            if (veterinario == null)
                return NotFound();

            return Json(
                new
                {
                    nombre = veterinario.Nombre,
                    apellido = veterinario.Apellido,
                    email = veterinario.Email,
                }
            );
        }

        //Fin Zona Veterinario

        //Zona Dueno
        //admin
        [Authorize(Roles = "Administrador")]
        public IActionResult CrearDuenoUsuario()
        {
            var disponibles = repoDueno.ObtenerDuenosSinUsuario();
            ViewBag.Duenos = new SelectList(disponibles, "Id", "Apellido");
            return View();
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public IActionResult CrearDuenoUsuario(Usuario usuario)
        {
            var disponibles = repoDueno.ObtenerDuenosSinUsuario();

            if (!usuario.DuenoId.HasValue || !disponibles.Any(d => d.Id == usuario.DuenoId))
            {
                ModelState.AddModelError("DuenoId", "Debe seleccionar un dueño válido.");
            }

            if (ModelState.IsValid)
            {
                string hashed = Convert.ToBase64String(
                    KeyDerivation.Pbkdf2(
                        password: usuario.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 10000,
                        numBytesRequested: 256 / 8
                    )
                );
                usuario.Clave = hashed;
                usuario.Rol = (int)enRoles.Dueno;
                repositorio.Alta(usuario);

                return RedirectToAction("Index");
            }

            ViewBag.Duenos = new SelectList(disponibles, "Id", "Apellido");
            return View(usuario);
        }

        //publico
        [AllowAnonymous]
        [HttpGet]
        public IActionResult BuscarDuenoPorDni()
        {
            return View();
        }

        [HttpPost]
        public IActionResult BuscarDuenoPorDni(int dni)
        {
            var dueno = repoDueno.BuscarPorDni(dni);
            if (dueno == null)
            {
                TempData["Error"] = "No se encontró un dueño con ese DNI.";
                return RedirectToAction("BuscarDuenoPorDni");
            }
            var usuario = repositorio.ObtenerPorEmail(dueno.Email);
            if (usuario != null)
            {
                TempData["Error"] = "El dueño ya tiene una cuenta registrada.";
                return RedirectToAction("BuscarDuenoPorDni");
            }

            return RedirectToAction(
                "RegistrarCuentaDueno",
                new
                {
                    idDueno = dueno.Id,
                    nombreCompleto = dueno.Nombre + " " + dueno.Apellido,
                    Nombre = dueno.Nombre,
                    Apellido = dueno.Apellido,
                    Email = dueno.Email,
                }
            );
        }

        [HttpGet]
        public IActionResult RegistrarCuentaDueno(
            int idDueno,
            string nombreCompleto,
            string Nombre,
            string Apellido,
            string Email
        )
        {
            var usuario = new Usuario { DuenoId = idDueno };
            ViewBag.NombreCompleto = nombreCompleto;
            ViewBag.IdDueno = idDueno;
            ViewBag.Nombre = Nombre;
            ViewBag.Apellido = Apellido;
            ViewBag.Email = Email;
            return View(usuario);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult RegistrarCuentaDueno(Usuario usuario)
        {
            if (!usuario.DuenoId.HasValue)
            {
                Console.WriteLine("El id del dueño no está definido. en el usuario");
                TempData["Error"] =
                    "El id del dueño no está definido. Por favor, vuelva a buscar al dueño por DNI.";
                return RedirectToAction("BuscarDuenoPorDni");
            }

            var dueno = repoDueno.ObtenerPorId(usuario.DuenoId.Value);
            if (dueno == null)
            {
                Console.WriteLine("No se encontró un dueño válido.");
                TempData["Error"] = "No se encontró un dueño válido.";
                ModelState.AddModelError("", "No se encontró un dueño válido.");
                return RedirectToAction("BuscarDuenoPorDni");
            }

            if (ModelState.IsValid)
            {
                string hashed = Convert.ToBase64String(
                    KeyDerivation.Pbkdf2(
                        password: usuario.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 10000,
                        numBytesRequested: 256 / 8
                    )
                );
                usuario.Clave = hashed;
                usuario.Rol = (int)enRoles.Dueno;
                repositorio.Alta(usuario);

                return RedirectToAction("Login", "Usuarios");
            }
            Console.WriteLine("El modelo no es válido.");
            TempData["Error"] = "Error, debe volver a ingresar su dni";
            return RedirectToAction("BuscarDuenoPorDni");
        }

        //Fin Zona Dueno
        //Zona Api
        [AllowAnonymous]
        [HttpPost("api/token")]
        public IActionResult ObtenerToken([FromBody] LoginView login)
        {
            if (
                login == null
                || string.IsNullOrEmpty(login.Usuario)
                || string.IsNullOrEmpty(login.Clave)
            )
            {
                return BadRequest("Datos inválidos");
            }

            string hashed = Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password: login.Clave,
                    salt: Encoding.ASCII.GetBytes(configuration["Salt"]),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8
                )
            );

            var usuario = repositorio.ObtenerPorEmail(login.Usuario);

            if (usuario == null || usuario.Clave != hashed)
            {
                return BadRequest("Usuario no encontrado o contraseña incorrecta");
            }
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.RolNombre),
                new Claim("NombreCompleto", usuario.Nombre + " " + usuario.Apellido),
            };

            // Clave y credenciales de firma
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["TokenJwt:SecretKey"])
            );
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Crear token
            var token = new JwtSecurityToken(
                issuer: configuration["TokenJwt:Issuer"],
                audience: configuration["TokenJwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return Ok(
                new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                }
            );
        }
        //Fin Zona Api
    }
}
