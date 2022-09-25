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
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _db;

        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;


        public ChatController(
           ApplicationDbContext db,
           IWebHostEnvironment webHostEnvironment,
           UserManager<ApplicationUser> userManager)
        {
            // add value to attributes
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(string userId)
        {
            ViewBag.UserId = userId;
            return View();
        }

        //GET 
        //Buscar uma pessoa randomica na mesma escola que o utilizador
        public IActionResult Create()
        {
            //Que usuário está logado?
            string userID = _userManager.GetUserId(User);

            //Saber o ID da universidade logada
            var uniUser = _db.Estudantes
                            .Where(a => a.UserId == userID)
                            .FirstOrDefault()
                            .UniversidadeFK;

            //Coletar todos os usuários na mesma universidade de maneira randomica
            var userRandom = _db.Estudantes
                        .Where(o => o.UniversidadeFK == uniUser && o.UserId != userID)
                        .OrderBy(c => Guid.NewGuid())
                        .FirstOrDefault();

            return View(userRandom);
        }

        [HttpPost]
        public IActionResult Create(Estudante estudante)
        {
            //Adicionar a conversa como conversa já existente na base de dados Messages
            //Mas só se a conversa já não exitir
            string sender = _userManager.GetUserId(User);
            //Verificar se existe alguma conversa com o usuario 
            try
            {
                var existChat = _db.Chats
                                .Where(a => a.Receiver == estudante.UserId && a.Sender == sender)
                                .FirstOrDefault()
                                .Receiver;
                //retornar para o chat iniciar uma conversa com o usuário
                return RedirectToAction("Index", new { userId = estudante.UserId });
            }
            catch (Exception ex)
            {
                Chat chat = new Chat()
                {
                    Receiver = estudante.UserId,
                    Sender = sender
                };

                _db.Chats.Add(chat);
                _db.SaveChanges();
                //retornar para o chat iniciar uma conversa com o usuário
                return RedirectToAction("Index", new { userId = estudante.UserId });
            }
        }
    }
}

