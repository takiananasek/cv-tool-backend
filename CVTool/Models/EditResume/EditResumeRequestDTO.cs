using CVTool.Models.Common;

namespace CVTool.Models.EditResume
{
    public class EditResumeRequestDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ProfileImageMetadataName { get; set; }
        public string BackgroundImageMetadataName { get; set; }
        public ICollection<ComponentDTO> Components { get; set; }
    }
}
