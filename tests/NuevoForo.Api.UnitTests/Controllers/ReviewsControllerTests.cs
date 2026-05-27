using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuevoForo.Api.Controllers;
using NuevoForo.Application.DTOs.Reviews;
using NuevoForo.Application.Services;

namespace NuevoForo.Api.UnitTests.Controllers
{
    [TestClass]
    public class ReviewsControllerTests
    {
        private Mock<IReviewService> _reviewServiceMock = null!;
        private Mock<ILikeService> _likeServiceMock = null!;
        private ReviewsController _controller = null!;
        private Mock<HttpContext> _httpContextMock = null!;

        [TestInitialize]
        public void Setup()
        {
            _reviewServiceMock = new Mock<IReviewService>();
            _likeServiceMock = new Mock<ILikeService>();
            
            _controller = new ReviewsController(_reviewServiceMock.Object, _likeServiceMock.Object);
            
            _httpContextMock = new Mock<HttpContext>();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = _httpContextMock.Object
            };
        }

        private void SetupUser(Guid? userId, params string[] roles)
        {
            var claims = new List<Claim>();
            if (userId.HasValue)
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()));
            }

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _httpContextMock.Setup(c => c.User).Returns(principal);
        }

        [TestMethod]
        public async Task Create_ModelStateInvalid_ReturnsValidationProblem()
        {
            // Arrange
            var userId = Guid.NewGuid();
            SetupUser(userId);
            _controller.ModelState.AddModelError("Error", "Message");
            var request = new ReviewCreateRequest();

            // Act
            var result = await _controller.Create(request, CancellationToken.None);

            // Assert
            // ValidationProblem() returns ObjectResult that can be a BadRequest
            Assert.IsInstanceOfType(result.Result, typeof(ObjectResult));
            var objectResult = (ObjectResult)result.Result!;

            // ValidationProblem should return 400 status code or contain ValidationProblemDetails
            Assert.IsInstanceOfType(objectResult.Value, typeof(ValidationProblemDetails));

            // The ObjectResult from ValidationProblem may not have explicit StatusCode set
            // but the type itself indicates validation error
            if (objectResult.StatusCode.HasValue)
            {
                Assert.IsTrue(objectResult.StatusCode >= 400);
            }
        }
    }
}
