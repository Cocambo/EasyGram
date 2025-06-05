using EasyGram.Data;
using EasyGram.Services;
using Microsoft.AspNetCore.Mvc;
using EasyGram.Models;
using Microsoft.EntityFrameworkCore;


namespace EasyGram.Controllers
{
    public class TheoryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMarkdownService _markdownService;

        public TheoryController(AppDbContext context, IMarkdownService markdownService)
        {
            _context = context;
            _markdownService = markdownService;
        }

        public async Task<IActionResult> Index(int? id)
        {
            var topics = await _context.Topics
                .OrderBy(t => t.Order)
                .ToListAsync();

            ViewBag.Topics = topics;

            if (id.HasValue)
            {
                var topic = await _context.Topics.FindAsync(id.Value);
                if (topic != null)
                {
                    ViewBag.SelectedTopic = topic;
                    ViewBag.ContentHtml = _markdownService.ToHtml(topic.Content);
                }
            }
            else if (topics.Any())
            {
                var firstTopic = topics.First();
                ViewBag.SelectedTopic = firstTopic;
                ViewBag.ContentHtml = _markdownService.ToHtml(firstTopic.Content);
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetTopic(int id)  // Через AJAX динамические прогружаем теорию
        {
            var topic = await _context.Topics.FindAsync(id);
            if (topic == null)
            {
                return NotFound();
            }

            var contentHtml = _markdownService.ToHtmlString(topic.Content);
            return Json(new { title = topic.Title, content = contentHtml });
        }
    }
}
