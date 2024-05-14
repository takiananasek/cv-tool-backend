using Microsoft.AspNetCore.Mvc.Testing;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace CVTool.IntegrationTests
{
    /// THIS IS STILL TO DO
    public abstract class TestBase
    {
        private IServiceScope scope;
        private readonly WebApplicationFactory<Program> _webApplicationFactory;
        protected HttpClient Client { get; private set; }

        protected IServiceProvider ServiceProvider { get; private set; }

        protected readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        protected TestBase(WebApplicationFactory<Program> webApplicationFactory)
        {
            _webApplicationFactory = webApplicationFactory;
        }

        public void GlobalInit()
        {
            Client = _webApplicationFactory.CreateClient();
        }

        public void GlobalTearDown()
        {
            Client?.Dispose();
            _webApplicationFactory?.Dispose();
        }

        protected void Init()
        {
            scope = _webApplicationFactory.Services.CreateScope();
            ServiceProvider = scope.ServiceProvider;
        }

        protected void Teardown()
        {
            scope?.Dispose();
        }
    }
}
