using CVTool.Data.Model;

namespace CVTool.Models.Common
{
    public class ComponentEntryDTO
    {
        public string Label { get; set; }
        public string? Value { get; set; }
        public ICollection<ComponentChildEntryDTO> Children { get; set; }
    }
}
