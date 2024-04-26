using AutoMapper;
using CVTool.Data;
using CVTool.Data.Model;
using CVTool.Models.AddResume;
using CVTool.Models.DeleteResume;
using CVTool.Models.GetResume;
using CVTool.Models.GetUserResumes;
using Microsoft.EntityFrameworkCore;

namespace CVTool.Services.ResumeService
{
    public class ResumeService: IResumeService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _dataContext;
        public ResumeService(DataContext dataContext, IMapper mapper)
        {
            _mapper = mapper;
            _dataContext = dataContext;
        }

        public async Task<AddResumeResponseDTO> UploadResume(AddResumeRequestDTO addResumeRequest)
        {
            var resume = _mapper.Map<Resume>(addResumeRequest);
            await _dataContext.AddAsync(resume);
            await _dataContext.SaveChangesAsync();
            return new AddResumeResponseDTO() { ResumeId = resume.Id};
        }

        public async Task<DeleteResumeResponseDTO> DeleteResume(DeleteResumeRequestDTO deleteResumeRequest)
        {
            var resume = await _dataContext.Resumes.FirstAsync(r => r.Id == deleteResumeRequest.Id);
             _dataContext.Remove(resume);
            await _dataContext.SaveChangesAsync();
            return new DeleteResumeResponseDTO();
        }

        public async Task<GetResumeResponseDTO> GetResume(GetResumeRequestDTO getResumeRequest)
        {
            var resume = await _dataContext.Resumes
                .Include(r => r.Components)
                .ThenInclude(c => c.ComponentEntries)
                .ThenInclude(ce => ce.Children)
                .FirstAsync(r => r.Id == getResumeRequest.Id);
            var response = _mapper.Map<GetResumeResponseDTO>(resume);
            return response;
        }

        public async Task<GetResumeByUserResponseDTO> GetUserResumes(GetResumeByUserRequestDTO getUserResumesRequest)
        {
            var resumes = await _dataContext.Resumes.Where(r => r.OwnerId == getUserResumesRequest.UserId)
                .Select(r => new ResumeDTO
                {
                    Id = r.Id,
                    Title = r.Title
                })
                .ToListAsync();

            return new GetResumeByUserResponseDTO
            {
                Resumes = resumes
            };
        }
    }
}
