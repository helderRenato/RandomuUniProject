using Microsoft.AspNetCore.Mvc;
using Projeto.Models;
using System.Diagnostics;
using Projeto.Data;
using Microsoft.AspNetCore.Identity;

namespace Projeto.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _logger = logger;
            _userManager = userManager;
        }

        public virtual IActionResult Index()
        {
            //Apenas retornar a página de load se o usuário na estiver já logado
            var userID = _userManager.GetUserId(User);
            if (userID == null) { return View(); }

            //Caso estiver logado redirecionar consoante a que tipo de usuario é "role"
            if (User.IsInRole("User"))
            {
                return RedirectToAction("Index", "Estudante");
            }
            else
            {
                return RedirectToAction("Index", "Universidade");
            }

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