using Defra.PTS.Web.Application.Features.Users.Commands;
using Defra.PTS.Web.Application.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Defra.PTS.Web.Application.UnitTests.Features.Users.Commands
{
    public class UpdateUserHandlerTests
    {
        [Fact]
        public async Task Handle_ReturnsUpdateUserResponseWithSuccess()
        {
            // Arrange
            var emailAddress = "test@example.com";
            var userId = Guid.NewGuid(); // Assuming some user id
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.UpdateUserAsync(emailAddress)).ReturnsAsync(userId);

            var loggerMock = new Mock<ILogger<UpdateUserHandler>>();

            var handler = new UpdateUserHandler(userServiceMock.Object, loggerMock.Object);
            var request = new UpdateUserRequest(emailAddress);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(userId, result.UserId);
            // Add more assertions here based on your expectations for the result
        }

        [Fact]
        public async Task Handle_LogsErrorAndThrowsException_WhenUserServiceThrowsException()
        {
            // Arrange
            var emailAddress = "test@example.com";
            string errorMessage = "Simulated error";
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.UpdateUserAsync(emailAddress)).ThrowsAsync(new Exception(errorMessage));

            var loggerMock = new Mock<ILogger<UpdateUserHandler>>();

            var handler = new UpdateUserHandler(userServiceMock.Object, loggerMock.Object);
            var request = new UpdateUserRequest(emailAddress);

            // Act + Assert
            try
            {
                await handler.Handle(request, CancellationToken.None);
                Assert.Fail("Expected exception was not thrown");
            }
            catch (Exception ex)
            {
                // Assert
                Assert.NotNull(ex);
                Assert.Equal(errorMessage, ex.Message);
            }

        }
    }
}
