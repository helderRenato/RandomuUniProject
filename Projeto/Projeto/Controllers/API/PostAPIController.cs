using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projeto.Data;
using Projeto.Models;

namespace Projeto.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostAPIController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public PostAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Post
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostsViewModel>>> GetPosts()
        {
            return await _context.Posts
                                 .Include(a => a.NewsletterFk)
                                 .OrderByDescending(a => a.Id)
                                 .Select(a => new PostsViewModel
                                 {
                                     Id = a.Id,
                                     Texto = a.Texto,
                                     Fotografia = a.Fotografia,
                                 })
                                .ToListAsync();
        }

        // GET: api/Post/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }

        // DELETE: api/Post/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}