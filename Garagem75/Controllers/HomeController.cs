using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using Garagem75.Models;
using Microsoft.AspNetCore.Mvc;

namespace Garagem75.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> EnviarAgendamento(string nome, string email, string telefone, string servico, string mensagem)
        {
            try
            {
                // Configuração do SMTP (exemplo: Gmail)
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("betosilvc@gmail.com", "vyjr laij zkpz shum"),
                    EnableSsl = true,
                };

                string corpoEmail = $@"
                    <h2>Novo agendamento recebido</h2>
                    <p><strong>Nome:</strong> {nome}</p>
                    <p><strong>E-mail:</strong> {email}</p>
                    <p><strong>Telefone:</strong> {telefone}</p>
                    <p><strong>Serviço:</strong> {servico}</p>
                    <p><strong>Mensagem:</strong> {mensagem}</p>
                ";

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("betosilvc@gmail.com", "Garagem 75"),
                    Subject = "Novo Agendamento de Serviço",
                    Body = corpoEmail,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add("betosilvc@gmail.com"); // Email que vai receber

                await smtpClient.SendMailAsync(mailMessage);

                TempData["MensagemSucesso"] = "Seu agendamento foi enviado com sucesso!";
                return RedirectToAction("Index"); // ou a página que quiser
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = "Ocorreu um erro ao enviar o agendamento: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
