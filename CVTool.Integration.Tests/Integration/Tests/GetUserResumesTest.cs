using AutoMapper;
using CVTool.Data.Model;
using CVTool.Models.GetUserResumes;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CVTool.Tests.Integration.Tests
{
    public class GetUserResumesTest: TestBase
    {
        private IMapper mapper;
        public GetUserResumesTest(): base(new TestWebApplicationFactory<Program>())
        { 
        }

        [SetUp]
        protected void Init()
        {
            mapper = ServiceProvider.GetService<IMapper>();
        }

        protected async Task CreateResumeAndUserAsync()
        {
            var dataContext = ServiceProvider.GetService<DataContextSqlite>();
            var user = await dataContext.Users.AddAsync(new Data.Model.User
            {
                Id = 1,
                Email = "email@gmail.com",
                JwtId = "1234",
                LoginProvider = "Google"
            });
            await dataContext.Resumes.AddAsync(new Data.Model.Resume
            {
               Id = 1,
               Title = "Test",
               BackgroundImageMetadataName = "Test",
               OwnerId = user.Entity.Id,
               Components = new List<Component>()
            });
        }

        [Test]
        public async Task Get_User_Should_return_valid_data()
        {
            //setup
            await CreateResumeAndUserAsync();
            var userId = 1;

            //act
            var url = $"Resume/getByUser";

            var dtoObject = new GetResumeByUserRequestDTO()
            {
                UserId = userId
            };
            string jsonContent = JsonConvert.SerializeObject(dtoObject);

            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await Client.PostAsync(url, content);

           //response.Should().BeEquivalentTo(actualUser);
        }
    }
}
