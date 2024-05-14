using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using CVTool.Models.GetResume;
using Newtonsoft.Json;
using System.Text;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using CVTool.Data;
using CVTool.Models.AddResume;
using CVTool.Models.Common;
using Microsoft.VisualStudio.TestPlatform.Common;
using Microsoft.AspNetCore.Authorization;
using CVTool.Filters;
using CVTool.Services.JwtUtils;
using Moq;
using System.Net.Http.Headers;
using Google;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using CVTool.Data.Model;
using System.Reflection;

namespace CVTool.IntegrationTests
{
    /// THIS IS STILL TO DO
    public class ResumeControllerTests: IClassFixture<WebApplicationFactory<Program>>
    {
        public readonly static string ConnectionString = "Data Source=TestDb.db";
        private readonly HttpClient _httpClient;
        public ResumeControllerTests(WebApplicationFactory<Program> factory)
        {
            _httpClient = factory.CreateClient();

            _httpClient = factory.
                WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        RemoveAllDbContextsFromServices(services);

                        var jwtUtils = services.SingleOrDefault(s => s.ServiceType == typeof(IJwtUtils));
                        services.Remove(jwtUtils);
                        var jwtUtilsMock = new Mock<IJwtUtils>();
                        jwtUtilsMock.Setup(x => x.ValidateJwtToken(It.IsAny<string>())).Returns(1);
                        services.Remove(jwtUtils);
                        services.AddScoped<IJwtUtils>(_ => jwtUtilsMock.Object);

                        //   SeedDatabase(factory);

                        var jwtUtils2 = services.SingleOrDefault(s => s.ServiceType == typeof(IJwtUtils));

                        services.AddDbContext<DataContext>(options =>
                        {
                            var projectAssemblyName = Assembly.GetAssembly(typeof(TestWebApplicationFactory<>)).GetName().Name;
                            options.UseSqlite(ConnectionString, x => x.MigrationsAssembly(projectAssemblyName));
                        });

                        services.AddDbContext<DemoContextSqlite>();

                        MigrateDbContext<DemoContextSqlite>(services);
                    });

                    //builder.ConfigureServices(services =>
                    //{
                    //    // var dbcontextoprions= services.SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<DataContext>));
                    //    // services.Remove(dbcontextoprions);

                    //    RemoveAllDbContextsFromServices(services);

                    //    services.AddDbContext<DataContext>(options =>
                    //    {
                    //        var projectAssemblyName = Assembly.GetAssembly(typeof(WebApplicationFactory<Program>)).GetName().Name;
                    //        options.UseSqlite(ConnectionString, x => x.MigrationsAssembly(projectAssemblyName));
                    //    });


                    //    var jwtUtils = services.SingleOrDefault(s => s.ServiceType == typeof(IJwtUtils));
                    //    services.Remove(jwtUtils);
                    //    var jwtUtilsMock = new Mock<IJwtUtils>();
                    //    jwtUtilsMock.Setup(x => x.ValidateJwtToken(It.IsAny<string>())).Returns(1);
                    //    services.Remove(jwtUtils);
                    //    services.AddScoped<IJwtUtils>(_ => jwtUtilsMock.Object);

                    //    //   SeedDatabase(factory);

                    //    var jwtUtils2 = services.SingleOrDefault(s => s.ServiceType == typeof(IJwtUtils));

                    //});
                })
                .CreateClient();
        }

        private void RemoveAllDbContextsFromServices(IServiceCollection services)
        {
            // reverse operation of AddDbContext<XDbContext> which removes  DbContexts from services
            var descriptors = services.Where(d => d.ServiceType.BaseType == typeof(DbContextOptions)).ToList();
            descriptors.ForEach(d => services.Remove(d));

            var dbContextDescriptors = services.Where(d => d.ServiceType.BaseType == typeof(DbContext)).ToList();
            dbContextDescriptors.ForEach(d => services.Remove(d));
        }

        public void MigrateDbContext<TContext>(IServiceCollection serviceCollection) where TContext : DbContext
        {
            var serviceProvider = serviceCollection.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();

            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<TContext>>();
            var context = services.GetService<TContext>();

            try
            {
                logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(TContext).Name);

                if (context.Database.IsSqlServer())
                {
                    throw new Exception("Use Sqlite instead of sql server!");
                }

                context.Database.EnsureDeleted();

                context.Database.Migrate();

                logger.LogInformation("Migrated database associated with context {DbContextName}", typeof(TContext).Name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}", typeof(TContext).Name);
                throw;
            }
        }

        private void SeedDatabase(WebApplicationFactory<Program> _factory)
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetService<DataContext>();

            _dbContext.Users.Add(new Data.Model.User
            {
                Email = "testEmail",
                JwtId = "JwtId",
                LoginProvider = "GOOGLE",
                RefreshTokens = new List<RefreshToken> { },
                Resumes = new List<Resume> { }
            });
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetResume_WithRequest_ReturnsOkResult()
        {

            var requestDto = new GetResumeRequestDTO
            {
                Id = 13
            };
            var jsonContent = JsonConvert.SerializeObject(requestDto);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("/Resume/get", content);


            //Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task AddResume_ValidRequest_ReturnsOk()
        {
            //arrange 
            var request = new AddResumeRequestDTO
            {
                OwnerId = 1,
                Components = new List<ComponentDTO> { },
                BackgroundImageMetadataName = "",
                ProfileImageMetadataName = "",
                Title = ""
            };

            //var jsonContent = JsonConvert.SerializeObject(request);
            //var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            //content.Headers.Add("Authorization", "blablabla");
            var jsonContent = JsonConvert.SerializeObject(request);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "Resume/add");
            requestMessage.Content = content;
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "blablabla");

            var result = await _httpClient.SendAsync(requestMessage);


            //act
            //var result = await _httpClient.PostAsync("Resume/add", content);

            //assert

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }
    }
}
