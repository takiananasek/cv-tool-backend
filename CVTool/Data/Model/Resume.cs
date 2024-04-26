namespace CVTool.Data.Model
{
    public class Resume
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? ProfileImageMetadataName { get; set; }
        public string? BackgroundImageMetadataName { get; set; }
        public int? OwnerId { get; set; }
        public User? Owner { get; set; }
        public ICollection<Component> Components { get; set; }
    }
}
