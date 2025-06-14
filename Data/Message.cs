using System.ComponentModel.DataAnnotations;

namespace RealTimeChat.Data
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string User { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Content { get; set; } = string.Empty;

        [StringLength(20)]
        public string Sentiment { get; set; } = "Neutral";

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}