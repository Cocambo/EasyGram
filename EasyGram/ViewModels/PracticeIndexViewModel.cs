using EasyGram.Models;

namespace EasyGram.ViewModels
{
    public class PracticeIndexViewModel // модель представления для начальной страницы практики
    {
        public List<Topic> Topics { get; set; }
        public int ProgressPercentage { get; set; }
        public int CompletedQuestions { get; set; }
        public int TotalQuestions { get; set; }
    }
}
