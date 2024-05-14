using Amazon.S3;
using AutoMapper;
using CVTool.Data;
using CVTool.Data.Model;
using CVTool.Models.AddResume;
using CVTool.Models.DeleteResume;
using CVTool.Models.EditResume;
using CVTool.Models.Files;
using CVTool.Models.GetResume;
using CVTool.Models.GetUserResumes;
using CVTool.Services.FilesService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CVTool.Services.ResumeService
{
    public class ResumeService : IResumeService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _dataContext;
        private readonly IFilesService _filesService;
        public ResumeService(DataContext dataContext, IMapper mapper, IFilesService filesService)
        {
            _mapper = mapper;
            _filesService = filesService;
            _dataContext = dataContext;
        }

        public async Task<AddResumeResponseDTO> UploadResume(AddResumeRequestDTO addResumeRequest)
        {
            var resume = _mapper.Map<Resume>(addResumeRequest);
            await _dataContext.AddAsync(resume);
            await _dataContext.SaveChangesAsync();
            return new AddResumeResponseDTO() { ResumeId = resume.Id };
        }

        public async Task<DeleteResumeResponseDTO> DeleteResume(DeleteResumeRequestDTO deleteResumeRequest)
        {
            var resume = await _dataContext.Resumes.FirstAsync(r => r.Id == deleteResumeRequest.Id);
            _dataContext.Remove(resume);
            await _filesService.DeleteResumeFiles(resume); ;
            await _dataContext.SaveChangesAsync();
            return new DeleteResumeResponseDTO();
        }

        public async Task<GetResumeResponseDTO> GetResume(GetResumeRequestDTO getResumeRequest)
        {
            var resume = await _dataContext.Resumes
                .Include(r => r.Components)
                .ThenInclude(c => c.ComponentEntries)
                .ThenInclude(ce => ce.Children)
                .FirstOrDefaultAsync(r => r.Id == getResumeRequest.Id);

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

        public async Task<EditResumeResponseDTO> EditResume(EditResumeRequestDTO editResumeRequest)
        {
            var resume = await _dataContext.Resumes.
                Include(r => r.Components)
                .ThenInclude(c => c.ComponentEntries)
                .ThenInclude(cche => cche.Children).
                FirstAsync(r => r.Id == editResumeRequest.Id);

            await _filesService.DeleteUnnecessaryFiles(resume, editResumeRequest.ProfileImageMetadataName, editResumeRequest.BackgroundImageMetadataName);

            _dataContext.RemoveRange(resume.Components);
            _mapper.Map(editResumeRequest, resume);
            _dataContext.Update(resume);
            await _dataContext.SaveChangesAsync();

            return new EditResumeResponseDTO
            {
                ResumeId = resume.Id
            };
        }
    }
}
