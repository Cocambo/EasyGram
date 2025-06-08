using EasyGram.Models;

namespace EasyGram.ViewModels
{
    public class TestViewModel
    {
        public Topic Topic { get; set; }
        public List<Question> Questions { get; set; }
        public int CurrentQuestionIndex { get; set; }
    }
}
