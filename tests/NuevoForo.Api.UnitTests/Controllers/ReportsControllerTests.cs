using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuevoForo.Api.Controllers;
using NuevoForo.Application.DTOs.Reports;
using NuevoForo.Application.Services;
using NuevoForo.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace NuevoForo.Api.UnitTests.Controllers
{
    [TestClass]
    public class ReportsControllerTests
    {
        private Mock<IReportService> _reportServiceMock = null!;
        private ReportsController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _reportServiceMock = new Mock<IReportService>();
            _controller = new ReportsController(_reportServiceMock.Object);
        }

        private void SetUserContext(Guid? userId)
        {
            var user = new ClaimsPrincipal();
            if (userId != null)
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()) };
                user.AddIdentity(new ClaimsIdentity(claims));
            }
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [TestMethod]
        public async Task Create_ModelStateInvalid_ReturnsValidationProblem()
        {
            // Arrange
            var userId = Guid.NewGuid();
            SetUserContext(userId);
            _controller.ModelState.AddModelError("Error", "Message");
            var request = new ReportCreateRequest();

            // Act
            var result = await _controller.Create(request, CancellationToken.None);

            // Assert
            // ValidationProblem() returns ObjectResult with ValidationProblemDetails
            Assert.IsInstanceOfType(result.Result, typeof(ObjectResult));
            var objectResult = (ObjectResult)result.Result!;
            Assert.IsInstanceOfType(objectResult.Value, typeof(ValidationProblemDetails));
        }

        [TestMethod]
        public async Task Create_UserIdNull_ReturnsUnauthorized()
        {
            // Arrange
            SetUserContext(null);
            var request = new ReportCreateRequest();

            // Act
            var result = await _controller.Create(request, CancellationToken.None);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public async Task Create_ValidRequest_ReturnsCreatedAtAction()
        {
            // Arrange
            var userId = Guid.NewGuid();
            SetUserContext(userId);
            var request = new ReportCreateRequest();
            var response = new ReportResponse { Id = Guid.NewGuid() };
            _reportServiceMock.Setup(s => s.CreateAsync(userId, request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.Create(request, CancellationToken.None);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
            var createdResult = (CreatedAtActionResult)result.Result;
            Assert.AreEqual(nameof(ReportsController.GetById), createdResult.ActionName);
            Assert.IsNotNull(createdResult.RouteValues);
            Assert.AreEqual(response.Id, createdResult.RouteValues["id"]);
            Assert.AreEqual(response, createdResult.Value);
        }

        [TestMethod]
        public async Task List_ReturnsOkWithReports()
        {
            // Arrange
            var reports = new List<ReportResponse> { new ReportResponse(), new ReportResponse() };
            _reportServiceMock.Setup(s => s.ListAsync(It.IsAny<EstadoReporte?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(reports);

            // Act
            var result = await _controller.List(null, 1, 20, CancellationToken.None);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result.Result;
            Assert.AreEqual(reports, okResult.Value);
        }

        [TestMethod]
        public async Task GetById_ReportNotFound_ReturnsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _reportServiceMock.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ReportResponse?)null);

            // Act
            var result = await _controller.GetById(id, CancellationToken.None);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetById_ReportExists_ReturnsOk()
        {
            // Arrange
            var id = Guid.NewGuid();
            var report = new ReportResponse { Id = id };
            _reportServiceMock.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(report);

            // Act
            var result = await _controller.GetById(id, CancellationToken.None);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result.Result;
            Assert.AreEqual(report, okResult.Value);
        }

        [TestMethod]
        public async Task Moderate_ModelStateInvalid_ReturnsValidationProblem()
        {
            // Arrange
            var userId = Guid.NewGuid();
            SetUserContext(userId);
            _controller.ModelState.AddModelError("Error", "Message");
            var request = new ModerationActionRequest();

            // Act
            var result = await _controller.Moderate(Guid.NewGuid(), request, CancellationToken.None);

            // Assert
            // ValidationProblem() returns ObjectResult with ValidationProblemDetails
            Assert.IsInstanceOfType(result.Result, typeof(ObjectResult));
            var objectResult = (ObjectResult)result.Result!;
            Assert.IsInstanceOfType(objectResult.Value, typeof(ValidationProblemDetails));
        }

        [TestMethod]
        public async Task Moderate_ModeratorIdNull_ReturnsUnauthorized()
        {
            // Arrange
            SetUserContext(null);
            var request = new ModerationActionRequest();

            // Act
            var result = await _controller.Moderate(Guid.NewGuid(), request, CancellationToken.None);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public async Task Moderate_ReportNotFound_ReturnsNotFound()
        {
            // Arrange
            var moderatorId = Guid.NewGuid();
            SetUserContext(moderatorId);
            var id = Guid.NewGuid();
            var request = new ModerationActionRequest { AccionTomada = AccionModeracion.Ocultar, Rechazar = false };
            _reportServiceMock.Setup(s => s.ModerateAsync(id, moderatorId, request.AccionTomada, request.Rechazar, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ReportResponse?)null);

            // Act
            var result = await _controller.Moderate(id, request, CancellationToken.None);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Moderate_ReportExists_ReturnsOk()
        {
            // Arrange
            var moderatorId = Guid.NewGuid();
            SetUserContext(moderatorId);
            var id = Guid.NewGuid();
            var request = new ModerationActionRequest { AccionTomada = AccionModeracion.Ocultar, Rechazar = false };
            var response = new ReportResponse { Id = id };
            _reportServiceMock.Setup(s => s.ModerateAsync(id, moderatorId, request.AccionTomada, request.Rechazar, It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.Moderate(id, request, CancellationToken.None);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result.Result;
            Assert.AreEqual(response, okResult.Value);
        }
    }
}