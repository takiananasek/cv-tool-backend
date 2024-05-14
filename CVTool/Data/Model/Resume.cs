using System.ComponentModel.DataAnnotations.Schema;

namespace CVTool.Data.Model
{
    [Table("Resumes")]
    public class Resume
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? ProfileImageMetadataName { get; set; }
        public string? BackgroundImageMetadataName { get; set; }
        public int? OwnerId { get; set; }
        public User? Owner { get; set; }
        public ICollection<Component> Components { get; set; } = new List<Component>();
    }
}
