using EasyGram.Models;
//доп мод предств
namespace EasyGram.ViewModels
{
    public class QuestionAnswerViewModel
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public List<Answer> Answers { get; set; }
    }
}
