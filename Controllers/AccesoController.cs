using ejemploRecuperacion.Models;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;
using System.Net.Mail;

namespace ejemploRecuperacion.Controllers
{
    public class AccesoController : Controller
    {

        private readonly BdRecuperarContext _contexto;
        public AccesoController(BdRecuperarContext contexto)
        {
            _contexto = contexto;
        }


        public IActionResult IniciarSesion()
        {
            return View();
        }



        [HttpGet]
        public IActionResult IniciarRecuperacion()
        {
            IniciarRecuperacionViewModel modelo = new IniciarRecuperacionViewModel();
            return View(modelo);
        }

        [HttpPost]
        public IActionResult IniciarRecuperacion(IniciarRecuperacionViewModel modelo)
        {

            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            string token = EncriptarToken(Guid.NewGuid().ToString());


            var user = _contexto.Usuarios.Where(u => u.Email == modelo.Email).FirstOrDefault();

            if (user != null)
            {
                user.TokenRecuperacion = token;
                _contexto.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _contexto.Usuarios.Update(user);
                _contexto.SaveChanges();

                EnviarEmail(user.Email, token);
            }

            return View();
        }


        [HttpGet]
        public IActionResult Recuperar(string token)
        {
            RecuperarViewModel model = new RecuperarViewModel();
            model.Token = token;


            if (model.Token == null || model.Token == String.Empty)
            {
                return View();
            }
            var user = _contexto.Usuarios.Where(u => u.TokenRecuperacion == model.Token).FirstOrDefault();

            if (user == null)
            {
                ViewBag.Error = "El token ha expirado";
                return View("IniciarSesion");

            }


            return View();
        }

        [HttpPost]
        public IActionResult Recuperar(RecuperarViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var user = _contexto.Usuarios.Where(u => u.TokenRecuperacion == modelo.Token).FirstOrDefault();

            if (user != null)
            {
                user.Contraseña = modelo.Password;
                user.TokenRecuperacion = null;
                _contexto.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _contexto.Usuarios.Update(user);
                _contexto.SaveChanges();
            }


            ViewBag.Message = "Contraseña modificada correctamente";

            return View("IniciarSesion");
        }

        #region HELPERS
        private string EncriptarToken(string input)
        {
            using (System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
                return Convert.ToBase64String(bytes);
            }
        }

        private void EnviarEmail(string emailDestino, string token)
        {
            string urlDominio = "https://localhost:7057";

            string EmailOrigen = "nikoalvarezzapata@gmail.com";
            string urlDeRecuperacion = String.Format("{0}/Acceso/Recuperar/?token={1}", urlDominio, token);

            MailMessage mensajeDelCorreo = new MailMessage(EmailOrigen, emailDestino, "Recuperación de contraseña",
                "<p>Email de restablecimiento de su contraseña</p><br>" +
                "<a href='" + urlDeRecuperacion + "'>Click para recuperar</a>");

            mensajeDelCorreo.IsBodyHtml = true;

            SmtpClient oSmtpCliente = new SmtpClient("smtp.gmail.com");
            oSmtpCliente.EnableSsl = true;
            oSmtpCliente.UseDefaultCredentials = false;
            oSmtpCliente.Port = 587;
            oSmtpCliente.Credentials = new System.Net.NetworkCredential(EmailOrigen, "quud ldzt vnpp xquv");

            oSmtpCliente.Send(mensajeDelCorreo);

            oSmtpCliente.Dispose();
        }
        #endregion
    }
}
