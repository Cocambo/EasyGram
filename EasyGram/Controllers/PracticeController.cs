using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EasyGram.Data;
using EasyGram.ViewModels;
using Microsoft.EntityFrameworkCore;
using EasyGram.Models;

namespace EasyGram.Controllers
{
    [Authorize]
    public class PracticeController : Controller
    {
        public readonly AppDbContext _context;
        public readonly UserManager<Users> _userManager;

        public PracticeController(AppDbContext context, UserManager<Users> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var topics = await _context.Topics
                .Include(t => t.Questions)
                .OrderBy(t => t.Order)
                .ToListAsync();

            var totalQuestions = await _context.Questions.CountAsync();
            var completedQuestions = await _context.UserProgresses
                .Where(up => up.UserId == currentUser.Id && up.IsCompleted)
                .CountAsync();

            var progressPercentage = totalQuestions > 0 ? (completedQuestions * 100) / totalQuestions : 0;

            var viewModel = new PracticeIndexViewModel
            {
                Topics = topics,
                ProgressPercentage = progressPercentage,
                CompletedQuestions = completedQuestions,
                TotalQuestions = totalQuestions
            };

            return View(viewModel);
        }

        public async Task<IActionResult> StartTest(int topicId)
        {
            var topic = await _context.Topics
                .Include(t => t.Questions)
                .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(t => t.Id == topicId);

            if (topic == null)
            {
                return NotFound();
            }

            var questions = topic.Questions.OrderBy(q => q.Order).ToList();

            var viewModel = new TestViewModel
            {
                Topic = topic,
                Questions = questions,
                CurrentQuestionIndex = 0
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CheckAnswer(int questionId, int answerId)
        {
            var question = await _context.Questions
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == questionId);

            if (question == null)
            {
                return Json(new { success = false, message = "Вопрос не найден" });
            }

            var selectedAnswer = question.Answers.FirstOrDefault(a => a.Id == answerId);
            var correctAnswer = question.Answers.FirstOrDefault(a => a.IsCorrect);

            if (selectedAnswer == null || correctAnswer == null)
            {
                return Json(new { success = false, message = "Ответ не найден" });
            }

            // Сохранение прогресса пользователя
            var currentUser = await _userManager.GetUserAsync(User);
            var existingProgress = await _context.UserProgresses
                .FirstOrDefaultAsync(up => up.UserId == currentUser.Id && up.QuestionId == questionId);

            if (existingProgress == null)
            {
                var userProgress = new UserProgress
                {
                    UserId = currentUser.Id,
                    QuestionId = questionId,
                    IsCompleted = true,
                    CompletedDate = DateTime.UtcNow
                };

                _context.UserProgresses.Add(userProgress);
                await _context.SaveChangesAsync();
            }

            return Json(new
            {
                success = true,
                isCorrect = selectedAnswer.IsCorrect,
                explanation = question.Explanation,
                correctAnswerText = correctAnswer.Text,
                selectedAnswerText = selectedAnswer.Text
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetProgress()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var totalQuestions = await _context.Questions.CountAsync();
            var completedQuestions = await _context.UserProgresses
                .Where(up => up.UserId == currentUser.Id && up.IsCompleted)
                .CountAsync();

            var progressPercentage = totalQuestions > 0 ? (completedQuestions * 100) / totalQuestions : 0;

            return Json(new
            {
                progressPercentage,
                completedQuestions,
                totalQuestions
            });
        }
    }
}
