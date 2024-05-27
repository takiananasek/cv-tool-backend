using CVTool.Services.FilesService;
using Microsoft.AspNetCore.Mvc;
using AuthorizeAttribute = CVTool.Filters.AuthorizeAttribute;

namespace CVTool.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly IFilesService _filesService;

        public FilesController(IFilesService filesService)
        {
            _filesService = filesService;
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