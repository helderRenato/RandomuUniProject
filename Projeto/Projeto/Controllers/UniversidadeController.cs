using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projeto.Data;
using Projeto.Models;

namespace Projeto.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UniversidadeController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;

        public UniversidadeController(
           ApplicationDbContext context,
           IWebHostEnvironment webHostEnvironment,
           UserManager<ApplicationUser> userManager)
        {
            // add value to attributes
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        public async Task<ActionResult> IndexAsync()
        {
            var userId = _userManager.GetUserId(User);
            //Newsletter associada a Universidade
            var newsletter = _context.Universidades
                                .Where(a => a.UserId == userId)
                                .FirstOrDefault()
                                .NewsletterFk;
            var applicationDbContext = _context.Posts.Include(c => c.Newsletter).Where(c => c.NewsletterFk == newsletter);
            return View(await applicationDbContext.ToListAsync());
        }
    }
}
