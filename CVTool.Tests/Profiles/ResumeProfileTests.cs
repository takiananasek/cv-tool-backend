using AutoMapper;
using CVTool.Services.ResumeService;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CVTool.Tests.Profiles
{
    public class ResumeProfileTests
    {
        [Fact]
        public void AutoMapper_Configuration_IsValid()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<ResumeProfile>());
            config.AssertConfigurationIsValid();
        }
    }
}
