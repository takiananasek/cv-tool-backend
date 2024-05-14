using CVTool.Data.Model;
using CVTool.Models.Files;

namespace CVTool.Services.FilesService
{
    public interface IFilesService
    {
        Task<UploadFileResponseDTO> UploadFile(IFormFile file);
        Task DeleteResumeFiles(Resume resume);
        Task DeleteUnnecessaryFiles(Resume resume, string newProfileMetadataFileName, string newBackgroundMetadataFileName);
    }
}
