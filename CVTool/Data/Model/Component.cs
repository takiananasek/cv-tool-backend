namespace CVTool.Data.Model
{
    public class Component
    {
        public int Id { get; set; }
        public int ResumeId { get; set; }
        public Resume? Resunme { get; set; }
        public int ComponentDocumentId { get; set; }
        public ComponentType ComponentType { get; set; }
        public ICollection<ComponentEntry> ComponentEntries { get; set; } = new List<ComponentEntry>();
    }
}
