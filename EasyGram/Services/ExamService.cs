using EasyGram.Models;
using EasyGram.Data;
using EasyGram.ViewModels.Exam;
using Microsoft.EntityFrameworkCore;

namespace EasyGram.Services
{
    public class ExamService : IExamService
    {
        private readonly AppDbContext _context;
        private const int QuestionsPerTopic = 3; // количество вопросов, которые будет выбираться из каждой темы

        public ExamService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Question>> GenerateExamQuestionsAsync() // возвращает случайные вопросы из каждой темы(формирование заданий)
        {
            var examQuestions = new List<Question>();

            var topics = await _context.Topics
                .Include(t => t.Questions)
                .ThenInclude(q => q.Answers)
                .Where(t => t.Questions.Any()) // хотя бы один вопрос есть у темы
                .ToListAsync();

            foreach (var topic in topics)
            {
                var randomQuestions = topic.Questions
                    .AsEnumerable() // преобразует коллекцию в IEnumerable<T>
                    .OrderBy(q => Guid.NewGuid())
                    .Take(QuestionsPerTopic)
                    .ToList();
                examQuestions.AddRange(randomQuestions); 
            }

            return examQuestions // возвращаем вопросы экзамены 
                .AsEnumerable()
                .OrderBy(q => Guid.NewGuid())
                .ToList();
        }
        // расчет результата экзамена и сохранение его
        public async Task<ExamResultViewModel> CalculateExamResultAsync(string userId, Dictionary<int, int> userAnswers) // key - id question, value - answer selected id 
        { 
            int correctAnswers = 0; // счетчик
            var questions = await _context.Questions // запрос заданий и ответов
                .Include(q => q.Answers)
                .Where(q => userAnswers.Keys.Contains(q.Id))
                .ToListAsync();

            foreach (var question in questions) 
            {
                if (userAnswers.TryGetValue(question.Id, out int selectedAnswerId)) // для получения значений по ключу в selectedAnswerId
                {
                    var selectedAnswer = question.Answers.FirstOrDefault(a => a.Id == selectedAnswerId);
                    if (selectedAnswer?.IsCorrect == true) // если правильный, то прибавляем 
                    {
                        correctAnswers++;
                    }
                }
            }

            decimal percentage = Math.Round((decimal)correctAnswers / userAnswers.Count * 100, 2);
            string grade = DetermineGrade(percentage);  // преобразовываем в Оценку

            
            var examResult = new ExamResult // сохраняем результат экзамена в БД
            {
                UserId = userId,
                ExamDate = DateTime.UtcNow, 
                TotalQuestions = userAnswers.Count,
                CorrectAnswers = correctAnswers,
                Percentage = percentage,
                Grade = grade,
                IsCertificateIssued = percentage >= 70
            };
            _context.ExamResults.Add(examResult);
            await _context.SaveChangesAsync();

            return new ExamResultViewModel
            {
                Id = examResult.Id,
                TotalQuestions = examResult.TotalQuestions,
                CorrectAnswers = examResult.CorrectAnswers,
                Percentage = examResult.Percentage,
                Grade = examResult.Grade,
                CanDownloadCertificate = examResult.IsCertificateIssued,
                CompletedDate = examResult.ExamDate
            };
        }

        public string DetermineGrade(decimal percentage)
        {
            if (percentage == 100) return "Превосходно";
            if (percentage >= 85) return "Отлично";
            if (percentage >= 70) return "Хорошо";
            return "Неудовлетворительно";
        }
    }
}
