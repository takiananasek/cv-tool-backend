using CVTool.Data.Model;

namespace CVTool.Models.Common
{
    public class ComponentDTO
    {
        public int ComponentDocumentId { get; set; }
        public ComponentType ComponentType { get; set; }
        public ICollection<ComponentEntryDTO> ComponentEntries { get; set; }
    }
}
