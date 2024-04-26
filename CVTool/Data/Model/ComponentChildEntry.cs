namespace CVTool.Data.Model
{
    public class ComponentChildEntry
    {
        public int Id { get; set; }
        public int ParentComponentEntryId { get; set; }
        public ComponentEntry? ParentComponentEntry { get; set; }
        public string Label { get; set; }
        public string? Value { get; set; }
    }
}
