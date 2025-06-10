namespace EasyGram.ViewModels.Exam
{
    public class ExamResultViewModel
    {
        public int Id { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public decimal Percentage { get; set; }
        public string Grade { get; set; }
        public bool CanDownloadCertificate { get; set; }
        public DateTime CompletedDate { get; set; }
    }
}
