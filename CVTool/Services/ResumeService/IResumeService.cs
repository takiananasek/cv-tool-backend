using CVTool.Models;
using CVTool.Models.AddResume;
using CVTool.Models.DeleteResume;
using CVTool.Models.EditResume;
using CVTool.Models.GetResume;
using CVTool.Models.GetUserResumes;

namespace CVTool.Services.ResumeService
{
    public interface IResumeService
    {
        Task<AddResumeResponseDTO> UploadResume(AddResumeRequestDTO addResumeRequest);
        Task<DeleteResumeResponseDTO> DeleteResume(DeleteResumeRequestDTO deleteResumeRequest);
        Task<GetResumeResponseDTO> GetResume(GetResumeRequestDTO getResumeRequest);
        Task<GetResumeByUserResponseDTO> GetUserResumes(GetResumeByUserRequestDTO getUserResumesRequest);
        Task<EditResumeResponseDTO> EditResume(EditResumeRequestDTO editResumeRequest);
    }
}
