namespace EasyGram.ViewModels.Exam
{
    public class ExamViewModel
    {
        public List<ExamQuestionViewModel> Questions { get; set; }
        public Dictionary<int, int> UserAnswers { get; set; }
        public DateTime ExamStartTime { get; set; }
    }
}
