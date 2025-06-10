namespace EasyGram.ViewModels.Exam
{
    public class CertificateViewModel
    {
        public string UserName { get; set; }
        public string Grade { get; set; }
        public decimal Percentage { get; set; }
        public DateTime CompletedDate { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
    }
}
