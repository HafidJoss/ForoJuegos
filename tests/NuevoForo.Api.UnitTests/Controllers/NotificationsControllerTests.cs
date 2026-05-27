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
using NuevoForo.Application.DTOs.Notifications;
using NuevoForo.Application.Services;
using NuevoForo.Domain.Enums;

namespace NuevoForo.Api.UnitTests.Controllers
{
    [TestClass]
    public class NotificationsControllerTests
    {
        private Mock<INotificationService> _mockNotificationService = null!;
        private NotificationsController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockNotificationService = new Mock<INotificationService>();
            _controller = new NotificationsController(_mockNotificationService.Object);
        }

        private void SetupUser(Guid? userId)
        {
            var claims = new List<Claim>();
            if (userId.HasValue)
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()));
            }

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        // --- List Tests ---
        
        [TestMethod]
        public async Task List_UserNotAuth_ExpectedUnauthorized()
        {
            // Arrange
            SetupUser(null);

            // Act
            var result = await _controller.List();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public async Task List_UserAuth_ExpectedOkWithNotifications()
        {
            // Arrange
            var userId = Guid.NewGuid();
            SetupUser(userId);
            var expectedNotifications = new List<NotificationResponse>
            {
                new NotificationResponse { Id = Guid.NewGuid(), Mensaje = "Test 1", Tipo = TipoNotificacion.Sistema },
                new NotificationResponse { Id = Guid.NewGuid(), Mensaje = "Test 2", Tipo = TipoNotificacion.Sistema }
            };

            _mockNotificationService.Setup(s => s.ListAsync(userId, 1, 20, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedNotifications);

            // Act
            var result = await _controller.List(1, 20);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            
            var returnedList = okResult.Value as IReadOnlyList<NotificationResponse>;
            Assert.IsNotNull(returnedList);
            Assert.AreEqual(2, returnedList.Count);
        }

        // --- MarkRead Tests ---

        [TestMethod]
        public async Task MarkRead_UserNotAuth_ExpectedUnauthorized()
        {
            // Arrange
            SetupUser(null);

            // Act
            var result = await _controller.MarkRead(Guid.NewGuid(), CancellationToken.None);

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public async Task MarkRead_Success_ExpectedNoContent()
        {
            // Arrange
            var userId = Guid.NewGuid();
            SetupUser(userId);
            var notificationId = Guid.NewGuid();
            
            _mockNotificationService.Setup(s => s.MarkReadAsync(userId, notificationId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.MarkRead(notificationId, CancellationToken.None);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task MarkRead_NotFound_ExpectedNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            SetupUser(userId);
            var notificationId = Guid.NewGuid();
            
            _mockNotificationService.Setup(s => s.MarkReadAsync(userId, notificationId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.MarkRead(notificationId, CancellationToken.None);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        // --- MarkAllRead Tests ---

        [TestMethod]
        public async Task MarkAllRead_UserNotAuth_ExpectedUnauthorized()
        {
            // Arrange
            SetupUser(null);

            // Act
            var result = await _controller.MarkAllRead(CancellationToken.None);

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public async Task MarkAllRead_UserAuth_ExpectedOkWithCount()
        {
            // Arrange
            var userId = Guid.NewGuid();
            SetupUser(userId);
            
            _mockNotificationService.Setup(s => s.MarkAllReadAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(5);

            // Act
            var result = await _controller.MarkAllRead(CancellationToken.None);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            
            // Check anonymous type
            var valueType = okResult.Value!.GetType();
            var propertyInfo = valueType.GetProperty("updated");
            Assert.IsNotNull(propertyInfo);
            var updatedCount = propertyInfo.GetValue(okResult.Value);
            
            Assert.AreEqual(5, updatedCount);
        }
    }
}
