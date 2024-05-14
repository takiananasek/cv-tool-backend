using Amazon.S3;
using Amazon.S3.Model;
using CVTool.Data;
using CVTool.Data.Model;
using CVTool.Models.Files;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net;

namespace CVTool.Services.FilesService
{
    public class FilesService : IFilesService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly DataContext _dataContext;
        private FileSettings _fileSettings;
        public FilesService(IAmazonS3 s3Client, DataContext dataContext, IOptions<FileSettings> fileSettings)
        {
            _s3Client = s3Client;
            _dataContext = dataContext;
            _fileSettings = fileSettings.Value;

        }
        public async Task DeleteResumeFiles(Resume resume)
        {
            List<string?> fileNamesToDelete = new List<string?>()
            {
                resume.ProfileImageMetadataName, resume.BackgroundImageMetadataName
            };

            try
            {
                var folderName = Path.Combine("Resources", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                foreach (string? file in fileNamesToDelete)
                {
                    if (!string.IsNullOrEmpty(file))
                    {
                        DeleteObjectRequest deleteRequest = new DeleteObjectRequest
                        {
                            BucketName = _fileSettings.BucketName,
                            Key = file
                        };
                        await _s3Client.DeleteObjectAsync(deleteRequest);
                    }
                }

                var imageMetaDataEntities = await _dataContext.ImageMetaDatas.Where(i => fileNamesToDelete.Contains(i.FileName)).ToListAsync();
                _dataContext.RemoveRange(imageMetaDataEntities);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to delete resume files.");
            }
        }

        public async Task DeleteUnnecessaryFiles(Resume resume, string newProfileMetadataFileName, string newBackgroundMetadataFileName)
        {
            List<string> fileNamesToDelete = new List<string>();
            if (resume.BackgroundImageMetadataName != newBackgroundMetadataFileName && !string.IsNullOrEmpty(resume.BackgroundImageMetadataName))
            {
                fileNamesToDelete.Add(resume.BackgroundImageMetadataName);
            }
            if (resume.ProfileImageMetadataName != newProfileMetadataFileName && !string.IsNullOrEmpty(resume.ProfileImageMetadataName))
            {
                fileNamesToDelete.Add(resume.ProfileImageMetadataName);
            }

            try
            {
                var folderName = Path.Combine("Resources", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                foreach (string file in fileNamesToDelete)
                {
                    DeleteObjectRequest deleteRequest = new DeleteObjectRequest
                    {
                        BucketName = _fileSettings.BucketName,
                        Key = file
                    };
                    await _s3Client.DeleteObjectAsync(deleteRequest);
                }

                var imageMetaDataEntities = await _dataContext.ImageMetaDatas.Where(i => fileNamesToDelete.Contains(i.FileName)).ToListAsync();
                _dataContext.RemoveRange(imageMetaDataEntities);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to delete old resume files.");
            }
        }

        public async Task<UploadFileResponseDTO> UploadFile(IFormFile file)
        {
            if (file.Length > 0)
            {
                Guid newId = Guid.NewGuid();
                string prefix = newId.ToString();

                var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, _fileSettings.BucketName);
                if (!bucketExists) return new UploadFileResponseDTO {
                    HttpStatusCode = HttpStatusCode.NotFound,
                    Key = null
                };

                var key = $"cv-tool/{prefix}{file.FileName}";
                var request = new PutObjectRequest()
                {
                    BucketName = _fileSettings.BucketName,
                    Key = key,
                    InputStream = file.OpenReadStream()
                };
                request.Metadata.Add("Content-Type", file.ContentType);
                await _s3Client.PutObjectAsync(request);

                await _dataContext.ImageMetaDatas.AddAsync(new ImageMetaData
                {
                    Id = newId,
                    FileName = key
                });

                await _dataContext.SaveChangesAsync();
                return  new UploadFileResponseDTO
                {
                    HttpStatusCode = HttpStatusCode.OK,
                    Key = key
                };
            }
            else
            {
                return new UploadFileResponseDTO
                {
                    HttpStatusCode = HttpStatusCode.BadRequest,
                    Key = null
                };
            }
        }
    }
}
