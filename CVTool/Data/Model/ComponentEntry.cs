namespace CVTool.Data.Model
{
    public class ComponentEntry
    {
        public int Id { get; set; }
        public int ComponentId { get; set; }
        public Component Component { get; set; }
        public string Label { get; set; }
        public string? Value { get; set; }
        public ICollection<ComponentChildEntry> Children { get; set; } = new List<ComponentChildEntry>();
    }
}
