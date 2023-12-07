using _4.SemesterProjekt_v3ArticleService.Data;
using _4.SemesterProjekt_v3ArticleService.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _4.SemesterProjekt_v3ArticleService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArticleController : ControllerBase
    {
        private readonly ArticleDbContext _context;

        public ArticleController(ArticleDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetArticles()
        {
            return Ok(await _context.Article.Include(x => x.Author).ToListAsync());
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostArticle(Article article)
        {
            article.Author = _context.Author.Where(x => x.Id == article.AuthorId).First();
            _context.Article.Add(article);
            await _context.SaveChangesAsync();

            return Ok(CreatedAtAction("GetArticle", new { id = article.Id }, article));
        }
    }
}
