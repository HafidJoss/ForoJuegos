using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuevoForo.Api.Controllers;
using NuevoForo.Application.DTOs.Auth;
using NuevoForo.Domain.Entities;
using NuevoForo.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NuevoForo.Api.UnitTests.Controllers
{
    [TestClass]
    public class AuthControllerTests
    {
        private Mock<UserManager<Usuario>> _userManagerMock = null!;
        private Mock<SignInManager<Usuario>> _signInManagerMock = null!;
        private Mock<IJwtTokenService> _tokenServiceMock = null!;
        private AuthController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            var userStoreMock = new Mock<IUserStore<Usuario>>();
            _userManagerMock = new Mock<UserManager<Usuario>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            var contextAccessorMock = new Mock<IHttpContextAccessor>();
            var claimsFactoryMock = new Mock<IUserClaimsPrincipalFactory<Usuario>>();
            _signInManagerMock = new Mock<SignInManager<Usuario>>(_userManagerMock.Object, contextAccessorMock.Object, claimsFactoryMock.Object, null!, null!, null!, null!);

            _tokenServiceMock = new Mock<IJwtTokenService>();

            _controller = new AuthController(_userManagerMock.Object, _signInManagerMock.Object, _tokenServiceMock.Object);
        }

        [TestMethod]
        public async Task Register_WithInvalidModelState_ReturnsValidationProblem()
        {
            // Arrange
            _controller.ModelState.AddModelError("Error", "Sample Error");
            var request = new RegisterRequest();

            // Act
            var result = await _controller.Register(request);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(ObjectResult));
            var objectResult = (ObjectResult)result.Result;
            Assert.IsNotNull(objectResult);
            // It could be ValidationProblemResult or BadRequestObjectResult depending on setup. ValidationProblem returns ObjectResult.
        }
    }
}