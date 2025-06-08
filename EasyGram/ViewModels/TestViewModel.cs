using EasyGram.Models;

namespace EasyGram.ViewModels
{
    public class TestViewModel // модель представления для самой страницы теста
    {
        public Topic Topic { get; set; }
        public List<Question> Questions { get; set; }
        public int CurrentQuestionIndex { get; set; }
    }
}
