using EasyGram.Models;
using EasyGram.ViewModels.Exam;

namespace EasyGram.Services
{
    public interface IExamService
    {
        Task<List<Question>> GenerateExamQuestionsAsync();
        Task<ExamResultViewModel> CalculateExamResultAsync(
            string userId, Dictionary<int, int> userAnswers);
        string DetermineGrade(decimal percentage);
    }
}
