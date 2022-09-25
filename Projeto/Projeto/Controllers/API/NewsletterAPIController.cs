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
    public class NewsletterAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NewsletterAPIController(ApplicationDbContext context)
        {
            _context = context;
        }




        // GET: api/DonosAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NewsletterViewModel>>> GetUniversidade()
        {
            return await _context.Newsletters
                                 .OrderBy(d => d.Id)
                                 .Select(d => new NewsletterViewModel
                                 {
                                     Id = d.Id
                                 })
                                 .ToListAsync();
        }

        // GET: api/DonosAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Newsletter>> GetUniversidade(int id)
        {
            var newsletter = await _context.Newsletters.FindAsync(id);

            if (newsletter == null)
            {
                return NotFound();
            }

            return newsletter;
        }

    }
}