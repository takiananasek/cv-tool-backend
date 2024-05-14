using Amazon.S3;
using Amazon.S3.Model;
using CVTool.Data;
using CVTool.Filters;
using CVTool.Models.Files;
using CVTool.Validators.Resolver;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AuthorizeAttribute = CVTool.Filters.AuthorizeAttribute;
using System.Net.Http.Headers;
using CVTool.Services.FilesService;

namespace CVTool.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly IFilesService _filesService;

        public FilesController(DataContext context, IAmazonS3 s3Client, IOptions<FileSettings> fileSettings)
        {
        }

        [HttpPost("upload"), DisableRequestSizeLimit]
        public async Task<IActionResult> Upload()
        {
            try
            {
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();

                var response = await _filesService.UploadFile(file);
                switch (response.HttpStatusCode)
                {
                    case System.Net.HttpStatusCode.OK:
                        return Ok(response);
                    case System.Net.HttpStatusCode.NotFound:
                        return NotFound(response);
                    case System.Net.HttpStatusCode.BadRequest:
                        return BadRequest(response);
                    default: return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
    }
}