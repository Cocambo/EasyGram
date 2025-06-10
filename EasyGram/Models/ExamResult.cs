using Microsoft.AspNetCore.Identity;

namespace EasyGram.Models
{
    public class ExamResult
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime ExamDate { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public decimal Percentage { get; set; }
        public string Grade { get; set; }
        public bool IsCertificateIssued { get; set; }
        public virtual Users User { get; set; }
    }
}
