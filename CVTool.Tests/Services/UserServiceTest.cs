
using CVTool.Data;
using CVTool.Data.Model;
using CVTool.Exceptions;
using CVTool.Models.Authentication;
using CVTool.Models.Users;
using CVTool.Services.JwtUtils;
using CVTool.Services.UserService;
using CVTool.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Moq;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Reflection.Metadata;
using Xunit;

namespace CVTool.Tests.Services
{
    public class UserServiceTest
    {
        [Fact]
        public async Task Authenticate_ReturnsTokens_ForAuthenticatedUser()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = 1, LoginProvider = "testProvider", JwtId = "testKey" , RefreshTokens = new List<RefreshToken>() }
            }.AsQueryable();

            var refreshTokens = new List<RefreshToken>
            {
            }.AsQueryable();

            var userMockSet = new Mock<DbSet<User>>();
            userMockSet.As<IDbAsyncEnumerable<User>>()
            .Setup(m => m.GetAsyncEnumerator())
               .Returns(new TestDbAsyncEnumerator<User>(users.GetEnumerator()));

            userMockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<User>(users.Provider));

            userMockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            userMockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            userMockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var refreshTokenMockSet = new Mock<DbSet<RefreshToken>>();
            refreshTokenMockSet.As<IDbAsyncEnumerable<RefreshToken>>()
            .Setup(m => m.GetAsyncEnumerator())
               .Returns(new TestDbAsyncEnumerator<RefreshToken>(refreshTokens.GetEnumerator()));

            refreshTokenMockSet.As<IQueryable<RefreshToken>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<RefreshToken>(users.Provider));

            refreshTokenMockSet.As<IQueryable<RefreshToken>>().Setup(m => m.Expression).Returns(refreshTokens.Expression);
            refreshTokenMockSet.As<IQueryable<RefreshToken>>().Setup(m => m.ElementType).Returns(refreshTokens.ElementType);
            refreshTokenMockSet.As<IQueryable<RefreshToken>>().Setup(m => m.GetEnumerator()).Returns(refreshTokens.GetEnumerator());

            var mockContext = new Mock<DataContext>();
            mockContext.Setup(c => c.Users).Returns(userMockSet.Object);
            mockContext.Setup(c => c.RefreshTokens).Returns(refreshTokenMockSet.Object);

            var mockJwtUtils = new Mock<IJwtUtils>();
            mockJwtUtils.Setup(m => m.GenerateJwtToken(It.IsAny<User>())).Returns("TOKEN");
            mockJwtUtils.Setup(m => m.GenerateRefreshToken()).Returns(new RefreshToken
            {
                Id = 1,
                Expires = new DateTime(),
                Created = new DateTime(),
                ReasonRevoked = "",
                ReplacedByToken = "",
                Revoked = null,
                Token = "REFRESH TOKEN",
                UserId = 1
            });

            var appSettings = new AppSettings
            {
                RefreshTokenTTL = 1,
                Secret = "Secret"
            };

            var mockAppsettings = new Mock<IOptions<AppSettings>>();
            mockAppsettings.Setup(x => x.Value).Returns(appSettings);

            var userService = new UserService(mockContext.Object, mockJwtUtils.Object, mockAppsettings.Object);

            // Act
            var result = userService.Authenticate(new Models.Authentication.AuthenticateRequestDto()
            {
                ProviderKey = "testKey",
            });

            // Assert
            Assert.NotNull(result.RefreshToken);
            Assert.NotEmpty(result.RefreshToken);
            Assert.NotNull(result.JwtToken);
            Assert.NotEmpty(result.JwtToken);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task AddUser_ForCorrectParams_ReturnsUserData()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = 1, LoginProvider = "testProvider", JwtId = "testKey" , RefreshTokens = new List<RefreshToken>() }
            }.AsQueryable();

            var refreshTokens = new List<RefreshToken>
            {
            }.AsQueryable();

            var userMockSet = new Mock<DbSet<User>>();
            userMockSet.As<IDbAsyncEnumerable<User>>()
            .Setup(m => m.GetAsyncEnumerator())
               .Returns(new TestDbAsyncEnumerator<User>(users.GetEnumerator()));

            userMockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<User>(users.Provider));

            userMockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            userMockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            userMockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var mockContext = new Mock<DataContext>();
            mockContext.Setup(c => c.Users).Returns(userMockSet.Object);

            var mockJwtUtils = new Mock<IJwtUtils>();
            var mockAppsettings = new Mock<IOptions<AppSettings>>();

            var userService = new UserService(mockContext.Object, mockJwtUtils.Object, mockAppsettings.Object);

            // Act
            var result = await userService.AddUser("loginProvider", "providerKey", "email");

            // Assert
            Assert.NotNull(result);
            Assert.IsType<User>(result);
        }

        [Fact]
        public void RefreshToken_ForCorrectTokenRequest_ReturnsAuthenticateResponse()
        {
            // Arrange
            var refreshTokens = new List<RefreshToken>
            {
                new RefreshToken
                {
                    Id =1,
                    Created = DateTime.UtcNow,
                    Expires = DateTime.UtcNow.AddDays(2),
                    Token = "TOKEN1",
                    UserId = 1,
                }
            }.AsQueryable();

            var users = new List<User>
            {
                new User { Id = 1, LoginProvider = "testProvider", JwtId = "testKey" , RefreshTokens = refreshTokens.ToList() }
            }.AsQueryable();

            var userMockSet = new Mock<DbSet<User>>();
            userMockSet.As<IDbAsyncEnumerable<User>>()
            .Setup(m => m.GetAsyncEnumerator())
               .Returns(new TestDbAsyncEnumerator<User>(users.GetEnumerator()));

            userMockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<User>(users.Provider));

            userMockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            userMockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            userMockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var refreshTokenMockSet = new Mock<DbSet<RefreshToken>>();
            refreshTokenMockSet.As<IDbAsyncEnumerable<RefreshToken>>()
            .Setup(m => m.GetAsyncEnumerator())
               .Returns(new TestDbAsyncEnumerator<RefreshToken>(refreshTokens.GetEnumerator()));

            refreshTokenMockSet.As<IQueryable<RefreshToken>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<RefreshToken>(users.Provider));

            refreshTokenMockSet.As<IQueryable<RefreshToken>>().Setup(m => m.Expression).Returns(refreshTokens.Expression);
            refreshTokenMockSet.As<IQueryable<RefreshToken>>().Setup(m => m.ElementType).Returns(refreshTokens.ElementType);
            refreshTokenMockSet.As<IQueryable<RefreshToken>>().Setup(m => m.GetEnumerator()).Returns(refreshTokens.GetEnumerator());

            var mockContext = new Mock<DataContext>();
            mockContext.Setup(c => c.Users).Returns(userMockSet.Object);
            mockContext.Setup(c => c.RefreshTokens).Returns(refreshTokenMockSet.Object);

            var mockJwtUtils = new Mock<IJwtUtils>();
            mockJwtUtils.Setup(m => m.GenerateRefreshToken()).Returns(new RefreshToken
            {
                Id = 1,
                Expires = new DateTime(),
                Created = new DateTime(),
                ReasonRevoked = "",
                ReplacedByToken = "",
                Revoked = null,
                Token = "REFRESH TOKEN",
                UserId = 1
            });
            var appSettings = new AppSettings
            {
                RefreshTokenTTL = 1,
                Secret = "Secret"
            };

            var mockAppsettings = new Mock<IOptions<AppSettings>>();
            mockAppsettings.Setup(x => x.Value).Returns(appSettings);

            var userService = new UserService(mockContext.Object, mockJwtUtils.Object, mockAppsettings.Object);

            // Act
            var result = userService.RefreshToken("TOKEN1");

            // Assert
            Assert.NotNull(result);
            Assert.IsType<AuthenticateResponseDto>(result);
            Assert.Equal("REFRESH TOKEN", result.RefreshToken);
        }

        [Fact]
        public void RefreshToken_ForInvalidToken_ThrowsError()
        {
            // Arrange
            var refreshTokens = new List<RefreshToken>
            {
                new RefreshToken
                {
                    Id =1,
                    Created = DateTime.UtcNow,
                    Expires = DateTime.UtcNow.AddDays(2),
                    Token = "TOKEN1",
                    UserId = 1,
                }
            }.AsQueryable();

            var users = new List<User>
            {
                new User { Id = 1, LoginProvider = "testProvider", JwtId = "testKey" , RefreshTokens = refreshTokens.ToList() }
            }.AsQueryable();

            var userMockSet = new Mock<DbSet<User>>();
            userMockSet.As<IDbAsyncEnumerable<User>>()
            .Setup(m => m.GetAsyncEnumerator())
               .Returns(new TestDbAsyncEnumerator<User>(users.GetEnumerator()));

            userMockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<User>(users.Provider));

            userMockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            userMockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            userMockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var refreshTokenMockSet = new Mock<DbSet<RefreshToken>>();
            refreshTokenMockSet.As<IDbAsyncEnumerable<RefreshToken>>()
            .Setup(m => m.GetAsyncEnumerator())
               .Returns(new TestDbAsyncEnumerator<RefreshToken>(refreshTokens.GetEnumerator()));

            refreshTokenMockSet.As<IQueryable<RefreshToken>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<RefreshToken>(users.Provider));

            refreshTokenMockSet.As<IQueryable<RefreshToken>>().Setup(m => m.Expression).Returns(refreshTokens.Expression);
            refreshTokenMockSet.As<IQueryable<RefreshToken>>().Setup(m => m.ElementType).Returns(refreshTokens.ElementType);
            refreshTokenMockSet.As<IQueryable<RefreshToken>>().Setup(m => m.GetEnumerator()).Returns(refreshTokens.GetEnumerator());

            var mockContext = new Mock<DataContext>();
            mockContext.Setup(c => c.Users).Returns(userMockSet.Object);
            mockContext.Setup(c => c.RefreshTokens).Returns(refreshTokenMockSet.Object);

            var mockJwtUtils = new Mock<IJwtUtils>();
            mockJwtUtils.Setup(m => m.GenerateRefreshToken()).Returns(new RefreshToken
            {
                Id = 1,
                Expires = new DateTime(),
                Created = new DateTime(),
                ReasonRevoked = "",
                ReplacedByToken = "",
                Revoked = null,
                Token = "REFRESH TOKEN",
                UserId = 1
            });
            var appSettings = new AppSettings
            {
                RefreshTokenTTL = 1,
                Secret = "Secret"
            };

            var mockAppsettings = new Mock<IOptions<AppSettings>>();
            mockAppsettings.Setup(x => x.Value).Returns(appSettings);

            var userService = new UserService(mockContext.Object, mockJwtUtils.Object, mockAppsettings.Object);

            // Assert
            Assert.ThrowsAny<AuthException>(() => userService.RefreshToken("INVALID TOKEN"));
        }
    }
}
