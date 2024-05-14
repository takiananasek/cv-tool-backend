using AutoMapper;
using CVTool.Data.Model;
using CVTool.Data;
using CVTool.Models.AddResume;
using CVTool.Services.ResumeService;
using Moq;
using Xunit;
using CVTool.Tests.Helpers;
using System.Reflection.Metadata.Ecma335;
using CVTool.Models.Common;
using CVTool.Models.DeleteResume;
using CVTool.Models.Files;
using Microsoft.Extensions.Options;
using Amazon.S3;
using CVTool.Services.FilesService;

namespace CVTool.Tests.Services
{
    public class ResumeServiceTests
    {

        [Fact]
        public async Task AddResume_ForCorrectPayload_ReturnsCorrectResponse()
        {
            var users = new List<User>
            {
            new User{
                Id = 1,
                JwtId = "123",
                Email = "email",
                LoginProvider = "GOOGLE",
                RefreshTokens = new List<RefreshToken>{},
                Resumes = new List<Resume>{}
            }};

            var resumes = new List<Resume>
            {
            };

            var components = new List<Component>
            { };

            var componenEntries = new List<ComponentEntry>
            { };


            var componentChildEntries = new List<ComponentChildEntry>
            {
            };

            var imageMetadatas = new List<ImageMetaData>
            {
            };

            var usersMock = DbSetMockHelper.GetDbSetMock(users);
            var resumesMock = DbSetMockHelper.GetDbSetMock(resumes);
            var componentsMock = DbSetMockHelper.GetDbSetMock(components);
            var componentsEntriesMock = DbSetMockHelper.GetDbSetMock(componenEntries);
            var componentChildEntriesMock = DbSetMockHelper.GetDbSetMock(componentChildEntries);
            var imageMetadatasMock = DbSetMockHelper.GetDbSetMock(imageMetadatas);

            var dbcontextMock = new Mock<DataContext>();

            dbcontextMock.Setup(c => c.Users).Returns(usersMock.Object);
            dbcontextMock.Setup(c => c.Resumes).Returns(resumesMock.Object);
            dbcontextMock.Setup(c => c.Components).Returns(componentsMock.Object);
            dbcontextMock.Setup(c => c.ComponentEntries).Returns(componentsEntriesMock.Object);
            dbcontextMock.Setup(c => c.ComponentChildEntries).Returns(componentChildEntriesMock.Object);
            dbcontextMock.Setup(c => c.ImageMetaDatas).Returns(imageMetadatasMock.Object);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<Resume>(It.IsAny<AddResumeRequestDTO>())).Returns(new Resume
            {
                Id = 1,
                BackgroundImageMetadataName = "Test",
                ProfileImageMetadataName = "Test",
                Components = new List<Component>
                {
                    new Component
                    {
                        Id = 1,
                        ComponentDocumentId = 1,
                        ComponentEntries = new List<ComponentEntry>
                        {
                            new ComponentEntry
                            {
                                Id = 1,
                                Label = "Test",
                                Value = "Test",
                                Children = new List<ComponentChildEntry>{},
                                ComponentId = 1
                            }
                        },
                        ComponentType = ComponentType.TitleElement,
                        ResumeId = 1,
                    }
                },
                OwnerId = 1
            });

            var addResumeRequestDto = new AddResumeRequestDTO()
            {
                BackgroundImageMetadataName = "Test1",
                ProfileImageMetadataName = "Test2",
                Components = new List<ComponentDTO>
                {
                    new ComponentDTO
                    {
                        ComponentDocumentId = 1,
                        ComponentEntries = new List<ComponentEntryDTO>
                        {
                            new ComponentEntryDTO()
                            {
                                Label = "TestLabel",
                                Value = "TestValue",
                                Children = new List<ComponentChildEntryDTO>{}
                            }
                        },
                        ComponentType = ComponentType.TitleElement,
                    }
                },
                OwnerId = 1,
                Title = "Test",
            };

            var fileServiceMock = new Mock<IFilesService>();
            fileServiceMock.Setup(s => s.DeleteResumeFiles(It.IsAny<Resume>())).Returns(Task.FromResult(true));
            fileServiceMock.Setup(s => s.DeleteUnnecessaryFiles(It.IsAny<Resume>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            var resumeService = new ResumeService(dbcontextMock.Object, mapperMock.Object, fileServiceMock.Object);

            var response =  await resumeService.UploadResume(addResumeRequestDto);

            Assert.NotNull(response);
            Assert.IsType<AddResumeResponseDTO>(response);
            Assert.Equal(1, response.ResumeId);
        }
    }
}
