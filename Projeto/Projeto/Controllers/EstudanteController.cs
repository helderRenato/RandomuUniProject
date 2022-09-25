using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using Projeto.Data;
using Projeto.Models;

namespace Projeto.Controllers
{
    [Authorize(Roles = "User")]
    public class EstudanteController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;


        public EstudanteController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<ActionResult> IndexAsync()
        {
            //Universidade associada ao estudante 
            var userId = _userManager.GetUserId(User);
            var universidadeId = _db.Estudantes
                                .Where(x => x.UserId == userId)
                                .FirstOrDefault()
                                .UniversidadeFK;
            //Newsletter associada a Universidade
            var newsletter = _db.Universidades
                                .Where(a => a.Id == universidadeId)
                                .FirstOrDefault()
                                .NewsletterFk; 
            var applicationDbContext = _db.Posts.Include(c => c.Newsletter).Where(c => c.NewsletterFk == newsletter);
            return View(await applicationDbContext.ToListAsync());
        }
    }
}