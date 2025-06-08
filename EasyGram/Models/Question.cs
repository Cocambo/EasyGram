using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EasyGram.Models
{
    public class Question
    {
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public string Explanation { get; set; }

        public int TopicId { get; set; }

        [ForeignKey("TopicId")]
        public virtual Topic Topic { get; set; }

        public virtual ICollection<Answer> Answers { get; set; }

        public virtual ICollection<UserProgress> UserProgresses { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public int Order { get; set; }
    }
}
