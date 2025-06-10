using EasyGram.Data;
using EasyGram.Models;
using EasyGram.Services;
using EasyGram.ViewModels;
using EasyGram.ViewModels.Exam;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using System.Security.Claims;
using System.Text.Json;

namespace EasyGram.Controllers
{
    [Authorize]
    public class ExamController : Controller
    {
        private readonly AppDbContext _context;
        public readonly UserManager<Users> _userManager;
        private readonly IEmailService _emailService;
        private readonly IExamService _examService;

        public ExamController(AppDbContext context, UserManager<Users> userManager, IEmailService emailService, IExamService examService)
        {
            _context = context;
            _userManager = userManager;
            _examService = examService;
            _emailService = emailService;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var totalQuestions = await _context.Questions.CountAsync(); // считаем прогресс пользователя по практике
            var completedQuestions = await _context.UserProgresses
                .Where(up => up.UserId == currentUser.Id && up.IsCorrect)
                .CountAsync();

            var progressPercentage = totalQuestions > 0 ? (int)Math.Round((double)completedQuestions / totalQuestions * 100) : 0;

            ViewBag.IsExamAvailable = progressPercentage >= 70;

            var topicsCount = await _context.Topics.CountAsync();
            var viewModel = new ExamStartViewModel
            {
                TotalTopics = topicsCount,
                QuestionsPerTopic = 3,
                TotalQuestions = topicsCount * 3
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> StartExam()
        {
            var questions = await _examService.GenerateExamQuestionsAsync();

            var examViewModel = new ExamViewModel
            {
                Questions = questions.Select(q => new ExamQuestionViewModel
                {
                    QuestionId = q.Id,
                    QuestionText = q.Text,
                    TopicTitle = q.Topic.Title,
                    Answers = q.Answers.Select(a => new AnswerOptionViewModel
                    {
                        AnswerId = a.Id,
                        Text = a.Text
                    }).ToList(),
                    IsAnswered = false
                }).ToList(),
                UserAnswers = new Dictionary<int, int>(),
                ExamStartTime = DateTime.UtcNow
            };

            HttpContext.Session.SetString("ExamData", JsonSerializer.Serialize(examViewModel));

            return View("TakeExam", examViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitExam(ExamViewModel model)
        {
            var unanswered = model.Questions.Count(q => !model.UserAnswers.ContainsKey(q.QuestionId));
            if (unanswered > 0)
            {
                ModelState.AddModelError("", $"Не выполнено заданий: {unanswered}");
                return View("TakeExam", model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _examService.CalculateExamResultAsync(userId, model.UserAnswers);

            return View("ExamResult", result);
        }

        [HttpPost]
        public async Task<IActionResult> DownloadCertificate(ExamResultViewModel model)
        {
            if (!model.CanDownloadCertificate)
            {
                return BadRequest("Недостаточно баллов для получения сертификата");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FindAsync(userId);

            var certificateData = new CertificateViewModel
            {
                UserName = user.FullName,
                Grade = model.Grade,
                Percentage = model.Percentage,
                CompletedDate = model.CompletedDate,
                TotalQuestions = model.TotalQuestions,
                CorrectAnswers = model.CorrectAnswers
            };

            var pdf = new ViewAsPdf("Certificate", certificateData) // Certificate.ViewModel/certificateData
            {
                FileName = $"Certificate_{DateTime.UtcNow:yyyyMMdd}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
                CustomSwitches = "--disable-smart-shrinking --print-media-type",
                PageMargins = new Rotativa.AspNetCore.Options.Margins(10, 10, 10, 10)
            };

            return pdf;
        }

        [HttpPost]
        public async Task<IActionResult> EmailCertificate(ExamResultViewModel model)
        {
            if (!model.CanDownloadCertificate)
            {
                return BadRequest("Недостаточно баллов для получения сертификата");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FindAsync(userId);

            var certificateData = new CertificateViewModel
            {
                UserName = user.Email,
                Grade = model.Grade,
                Percentage = model.Percentage,
                CompletedDate = model.CompletedDate,
                TotalQuestions = model.TotalQuestions,
                CorrectAnswers = model.CorrectAnswers
            };
            
            var tempFilePath = Path.Combine(Path.GetTempPath(), $"Certificate_{Guid.NewGuid()}.pdf"); // путь в папку temp

            var pdf = new ViewAsPdf("Certificate", certificateData)
            {
                FileName = $"Certificate_{DateTime.UtcNow:yyyyMMdd}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
                CustomSwitches = "--disable-smart-shrinking --print-media-type",
                PageMargins = new Rotativa.AspNetCore.Options.Margins(10, 10, 10, 10),
                SaveOnServerPath = tempFilePath                                                  // сохраняем во временный файл
            };
            await pdf.BuildFile(ControllerContext);

            try
            {
                if (System.IO.File.Exists(tempFilePath))
                {
                    var pdfBytes = await System.IO.File.ReadAllBytesAsync(tempFilePath);
                    await _emailService.SendCertificateAsync(user.Email, pdfBytes, certificateData);
                    TempData["Message"] = "Сертификат успешно отправлен на ваш email";
                    return View("ExamResult", model);
                }
            }
            finally
            {
                if (System.IO.File.Exists(tempFilePath))
                {
                    System.IO.File.Delete(tempFilePath);
                }               
            }

            return BadRequest("Не удалось сгенерировать PDF");
        }

    }
}
