using System.ComponentModel.DataAnnotations;

namespace EasyGram.Models
{
    public class Topic
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public int Order { get; set; } // Для сортировки тем в меню
    }
}
