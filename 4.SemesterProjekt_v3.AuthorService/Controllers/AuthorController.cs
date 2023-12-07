using _4.SemesterProjekt_v3.AuthorService.Data;
using _4.SemesterProjekt_v3.AuthorService.Entities;
using _4.SemesterProjekt_v3.AuthorService.PubSub;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace _4.SemesterProjekt_v3.AuthorService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthorController : ControllerBase
    {
        private readonly AuthorDbContext _context;
        private readonly IntegrationEventSenderService _integrationEventSenderService;

        public AuthorController(AuthorDbContext context,IntegrationEventSenderService integrationEventSenderService)
        {
            _context = context;
            _integrationEventSenderService = integrationEventSenderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAuthors()
        {
            return Ok(await _context.Author.ToArrayAsync());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthor(int id, Author author)
        {
            using var transaction = _context.Database.BeginTransaction();

            author.Id = id;
            _context.Update(author).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var integrationEventData = JsonConvert.SerializeObject(new
            {
                id = author.Id,
                newname = author.Name,
                version = author.Version
            });

            _context.IntegrationEvent.Add(new IntegrationEvent
            {
                Event = "author.update",
                Data = integrationEventData
            });

            _context.SaveChanges();
            transaction.Commit();

            _integrationEventSenderService.StartPublishingOutstandingIntegrationEvents();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> PostAuthor(Author author)
        {
            using var transaction = _context.Database.BeginTransaction();

            _context.Author.Add(author);
            _context.SaveChanges();

            var integrationEventData = JsonConvert.SerializeObject(new
            {
                id = author.Id,
                name = author.Name,
                version = author.Version
            });

            _context.IntegrationEvent.Add(new IntegrationEvent
            {
                Event = "author.add",
                Data = integrationEventData
            });

            _context.SaveChanges();
            transaction.Commit();

            _integrationEventSenderService.StartPublishingOutstandingIntegrationEvents();

            return Ok(CreatedAtAction("GetAuthor", new { id = author.Id }, author));
        }
    }
}
