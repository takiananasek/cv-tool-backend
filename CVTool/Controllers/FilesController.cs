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

namespace CVTool.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IAmazonS3 _s3Client;
        private readonly FileSettings _fileSettings;

        public FilesController(IWebHostEnvironment webHostEnvironment, DataContext context, IAmazonS3 s3Client, IOptions<FileSettings> fileSettings)
        {
            _context = context;
            _s3Client = s3Client;
            _fileSettings = fileSettings.Value;
        }

        [HttpPost("upload"), DisableRequestSizeLimit]
        public async Task<IActionResult> Upload()
        {
            try
            {
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();
                var folderName = Path.Combine("Resources", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition)?.FileName?.Trim('"');

                    Guid newId = Guid.NewGuid();
                    string prefix = newId.ToString();

                    var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, _fileSettings.BucketName);
                    if (!bucketExists) return NotFound($"Bucket {_fileSettings.BucketName} does not exist.");
                    var key = $"cv-tool/{prefix}{file.FileName}";
                    var request = new PutObjectRequest()
                    {
                        BucketName = _fileSettings.BucketName,
                        Key = key,
                        InputStream = file.OpenReadStream()
                    };
                    request.Metadata.Add("Content-Type", file.ContentType);
                    await _s3Client.PutObjectAsync(request);

                    await _context.ImageMetaDatas.AddAsync(new Data.Model.ImageMetaData
                    {
                        Id = newId,
                        FileName = key
                    });

                    await _context.SaveChangesAsync();
                    return Ok(new { key });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
    }
}