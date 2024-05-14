using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace CVTool.Tests.Integration
{
    [TestFixture]
    public class TestBase
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

        [OneTimeSetUp]
        public void GlobalInit()
        {
            Client = _webApplicationFactory.CreateClient();
        }

        [OneTimeTearDown]
        public void GlobalTearDown()
        {
            Client?.Dispose();
            _webApplicationFactory?.Dispose();
        }

        [SetUp]
        protected void Init()
        {
            scope = _webApplicationFactory.Services.CreateScope();
            ServiceProvider = scope.ServiceProvider;
        }

        [TearDown]
        protected void Teardown()
        {
            scope?.Dispose();
        }
    }
}
