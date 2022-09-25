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
    [Authorize(Roles = "Admin")]
    public class PostController : Controller
    {

        /// <summary>
        /// manipula os dados da base de dados
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// manipula os dados dos utilizadores
        /// </summary>
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public PostController(
           ApplicationDbContext context,
           IWebHostEnvironment webHostEnvironment,
           UserManager<ApplicationUser> userManager
           )
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Fazer Load de todos os posts contidos na newsletter da universidade 
        /// </summary>
        public async Task<IActionResult> Index()
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

        //GET
        public IActionResult Create()
        {
            return View();
        }

        //POST

        /// <summary>
        /// Para a criação de um post são necessários (texto, fotografia?, a que newsletter a universidade pertence, e data de criação)
        /// </summary>
        /// <param name="post"></param>
        /// <param name="fotoPost"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Texto, Fotografia")] Post post, IFormFile? fotoPost)
        {
            //A que newsletter está a universidade criadora do post logada?
            //Que Universidade está logada?
            string userID = _userManager.GetUserId(User);
            //Receber o ID da newsletter associado a universidade 
            var IDNewsletter = _context.Universidades
                                       .Where(a => a.UserId == userID)
                                       .FirstOrDefault()
                                       .NewsletterFk; 
              
            post.NewsletterFk = (int) IDNewsletter;
            //data de criação
            post.RegistrationDate = DateTime.Now;
            //Upload de Fotografia
            if (fotoPost != null)
            {
                if (!(fotoPost.ContentType == "image/jpeg" || fotoPost.ContentType == "image/png"))
                {
                    // write the error message
                    ModelState.AddModelError("", "Please, if you want to send a file, please choose an image...");
                    // resend control to view, with data provided by user
                    return View();
                }
                else
                {
                    // define image name
                    Guid g;
                    g = Guid.NewGuid();
                    string imageName = g.ToString();
                    string extensionOfImage = Path.GetExtension(fotoPost.FileName).ToLower();
                    imageName += extensionOfImage;
                    // add image name to vet data
                    post.Fotografia = imageName;
                }
            }

            // validar se dados providos pela universidade estão válidos
            if (ModelState.IsValid)
            {
                try
                {
                    // adicionar post a base de dados
                    _context.Add(post);
                    // commit
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    // if the code arrives here, something wrong has appended
                    // we must fix the error, or at least report it

                    // add a model error to our code
                    ModelState.AddModelError("", "Something went wrong. I can not store data on database");
                    // eventually, before sending control to View
                    // report error. For instance, write a message to the disc
                    // or send an email to admin              

                    // send control to View
                    return View();
                }
                // save image file to disk
                // ********************************
                if (fotoPost != null)
                {
                    // ask the server what address it wants to use
                    string addressToStoreFile = _webHostEnvironment.WebRootPath;
                    string newImageLocalization = Path.Combine(addressToStoreFile, "Photos//User");
                    // see if the folder 'Photos' exists
                    if (!Directory.Exists(newImageLocalization))
                    {
                        Directory.CreateDirectory(newImageLocalization);
                    }
                    // save image file to disk
                    newImageLocalization = Path.Combine(newImageLocalization, post.Fotografia);
                    using var stream = new FileStream(newImageLocalization, FileMode.Create);
                    await fotoPost.CopyToAsync(stream);
                }
            }


            return View();
        }


        //EDIT GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return RedirectToAction("Index");
            }

            //Guardar o ID em uma variável de sessão para evitar erros de inputs exteriores
            HttpContext.Session.SetInt32("PostID", post.Id);
            return View(post);
        }

        //EDIT POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Texto,Fotografia")] Post post, IFormFile? fotoPost)
        {
            //A que newsletter está a universidade criadora do post logada?
            //Que Universidade está logada?
            string userID = _userManager.GetUserId(User);
            //Receber o ID da newsletter associado a universidade 
            var IDNewsletter = _context.Universidades
                                       .Where(a => a.UserId == userID)
                                       .FirstOrDefault()
                                       .NewsletterFk;

            post.NewsletterFk = (int) IDNewsletter;
            //data de criação
            post.RegistrationDate = DateTime.Now;
            if (id != post.Id)
            {
                return NotFound();
            }

            /* confirmar se não houve adulteração de dados no browser
             * para que isto aconteça:
             * 1 - recuperar o valor da variável de sessão
             * 2 - comparar este valor com os dados que chegam do browser
             * 3 - se forem diferentes, temos um problema...
             */

            // (1)
            var idPost = HttpContext.Session.GetInt32("PostID");

            /* se a variável 'idPost' for nula,
            * o que aconteceu?
            *   - houve 'injeção' de dados através de uma ferramenta externa
            *   - demorou-se demasiado tempo na execução da tarefa
            */
            if (idPost == null)
            {
                // neste caso o q fazer????
                ModelState.AddModelError("", "Demorou demasiado tempo a executar a tarefa de edição");
                return View(post);
            }

            // (3)
            if (idPost != post.Id)
            {
                // temos problemas...
                // o que vamos fazer????
                return RedirectToAction("Index");
            }
            //Imagem apenas será trocada caso o usuário escolha outra imagem

            //se a imagem for nula quer dizer que o usuario não pretende mudar a imagem actual
            //então é necessário eliminar a imagem anterior e realocar 
            if(fotoPost == null)
            {
                post.Fotografia = _context.Posts
                           .Where(a => a.Id == id)
                           .FirstOrDefault()
                           .Fotografia;
                string addressToStoreFile = _webHostEnvironment.WebRootPath;
                string newImageLocalization = Path.Combine(addressToStoreFile, "Photos//User");
                newImageLocalization = Path.Combine(newImageLocalization, post.Fotografia);

                using (var stream = System.IO.File.OpenRead(newImageLocalization))
                {
                    fotoPost = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
                    {
                        //Dica deste código retirada apartir do link: https://stackoverflow.com/questions/51704805/how-to-instantiate-an-instance-of-formfile-in-c-sharp-without-moq
                        Headers = new HeaderDictionary(), 
                        ContentType = "image/png"
                    };
               
                }
            }
            else
            {
                if (!(fotoPost.ContentType == "image/jpeg" || fotoPost.ContentType == "image/png"))
                {
                    // write the error message
                    ModelState.AddModelError("", "Please, if you want to send a file, please choose an image...");
                    // resend control to view, with data provided by user
                    return View();
                }
                else
                {
                    Guid g;
                    g = Guid.NewGuid();
                    string imageName = g.ToString();
                    string extensionOfImage = Path.GetExtension(fotoPost.FileName).ToLower();
                    imageName += extensionOfImage;
                    // add image name to vet data
                    post.Fotografia = imageName;
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            if (fotoPost != null)
            {
                // ask the server what address it wants to use
                string addressToStoreFile = _webHostEnvironment.WebRootPath;
                string newImageLocalization = Path.Combine(addressToStoreFile, "Photos//User");
                // see if the folder 'Photos' exists
                if (!Directory.Exists(newImageLocalization))
                {
                    Directory.CreateDirectory(newImageLocalization);
                }
                // save image file to disk
                newImageLocalization = Path.Combine(newImageLocalization, fotoPost.Name);
                using var stream = new FileStream(newImageLocalization, FileMode.Create);
                await fotoPost.CopyToAsync(stream);
            }
            return View(post);
        }

        //Verificar se o Post existe através da sua identificação
        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }

        // GET DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(c => c.Newsletter)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
