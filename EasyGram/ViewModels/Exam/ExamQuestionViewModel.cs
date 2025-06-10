namespace EasyGram.ViewModels.Exam
{
    public class ExamQuestionViewModel
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string TopicTitle { get; set; }
        public List<AnswerOptionViewModel> Answers { get; set; }
        public bool IsAnswered { get; set; }
    }
}
