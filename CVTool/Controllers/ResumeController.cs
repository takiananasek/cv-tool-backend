using CVTool.Filters;
using CVTool.Models.AddResume;
using CVTool.Models.DeleteResume;
using CVTool.Models.EditResume;
using CVTool.Models.GetResume;
using CVTool.Models.GetUserResumes;
using CVTool.Services.ResumeService;
using CVTool.Validators.Resolver;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using AuthorizeAttribute = CVTool.Filters.AuthorizeAttribute;

namespace CVTool.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ResumeController : ControllerBase
    {
        private readonly IResumeService _resumeService;
        private readonly IValidatorsResolver _validatorsResolver;
        public ResumeController(IResumeService resumeService, IValidatorsResolver validatorsFactory)
        {
            _validatorsResolver = validatorsFactory;
            _resumeService = resumeService;
        }

        [HttpPost("add")]
        public async Task<AddResumeResponseDTO> UploadResume(AddResumeRequestDTO addResumeRequest)
        {
            var validator = _validatorsResolver.Resolve<AddResumeRequestDTO>();
            await validator.ValidateAndThrowAsync(addResumeRequest);
            return await _resumeService.UploadResume(addResumeRequest);
        }

        [HttpPost("delete")]
        public async Task<DeleteResumeResponseDTO> DeleteResume(DeleteResumeRequestDTO deleteResumeRequest)
        {
            var validator = _validatorsResolver.Resolve<DeleteResumeRequestDTO>();
            await validator.ValidateAndThrowAsync(deleteResumeRequest);     
            return await _resumeService.DeleteResume(deleteResumeRequest);
        }

        [HttpPost("get")]
        public async Task<GetResumeResponseDTO> GetResume(GetResumeRequestDTO getResumeRequest)
        {
            var validator = _validatorsResolver.Resolve<GetResumeRequestDTO>();
            await validator.ValidateAndThrowAsync(getResumeRequest);
            return await _resumeService.GetResume(getResumeRequest);
        }


        [HttpPost("edit")]
        public async Task<EditResumeResponseDTO> Edit(EditResumeRequestDTO editResumeRequest)
        {
            var validator = _validatorsResolver.Resolve<EditResumeRequestDTO>();
            await validator.ValidateAndThrowAsync(editResumeRequest);
            return await _resumeService.EditResume(editResumeRequest);
        }

        [HttpPost("getByUser")]
        public async Task<GetResumeByUserResponseDTO> GetUserResumeList(GetResumeByUserRequestDTO getResumeRequest)
        {
            var validator = _validatorsResolver.Resolve<GetResumeByUserRequestDTO>();
            await validator.ValidateAndThrowAsync(getResumeRequest);
            return await _resumeService.GetUserResumes(getResumeRequest);
        }
    }
}