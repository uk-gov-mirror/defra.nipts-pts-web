using Defra.PTS.Web.Application.Constants;
using Defra.PTS.Web.Application.DTOs.Features;
using Defra.PTS.Web.Application.DTOs.Services;
using Defra.PTS.Web.Application.Extensions;
using Defra.PTS.Web.Application.Features.DynamicsCrm.Commands;
using Defra.PTS.Web.Application.Features.TravelDocument.Queries;
using Defra.PTS.Web.Application.Features.Users.Commands;
using Defra.PTS.Web.Application.Features.Users.Queries;
using Defra.PTS.Web.Application.Services.Interfaces;
using Defra.PTS.Web.Domain.Enums;
using Defra.PTS.Web.Domain.Models;
using Defra.PTS.Web.Domain.ViewModels;
using Defra.PTS.Web.Domain.ViewModels.TravelDocument;
using Defra.PTS.Web.UI.Controllers;
using Defra.PTS.Web.UI.Helpers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using System.Net;
using System.Security.Claims;
using Xunit;
using Xunit.Sdk;

namespace Defra.PTS.Web.UI.UnitTests.Controllers
{
    public class TravelDocumentControllerTests
    {
        private readonly Mock<IValidationService> _mockValidationService = new();
        private readonly Mock<IMediator> _mockMediator = new();
        private readonly Mock<ILogger<TravelDocumentController>> _mockLogger = new();
        private IOptions<PtsSettings> _optionsPtsSettings;
        private TravelDocumentController _travelDocumentController;

        private readonly IStringLocalizer<ISharedResource> _localizer;
        private readonly Mock<ISelectListLocaliser> _breedHelper = new();

        public TravelDocumentControllerTests()
        {
            var options = Options.Create(new LocalizationOptions { ResourcesPath = "Resources" });
            var factory = new ResourceManagerStringLocalizerFactory(options, NullLoggerFactory.Instance);
            _localizer = new StringLocalizer<ISharedResource>(factory);

            var ptsSettings = new PtsSettings
            {
                MagicWordEnabled = true,
            };
            _optionsPtsSettings = Options.Create(ptsSettings);
            _travelDocumentController = new TravelDocumentController(_mockValidationService.Object, _mockMediator.Object, _mockLogger.Object, _optionsPtsSettings, _breedHelper.Object, _localizer);
        }


        [Fact]
        public void If_MagicWordEnabled_True_RedirectTo_Index()
        {
            // Arrange
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var mockHttpContext = new Mock<HttpContext>();
            var mockRequest = new Mock<Microsoft.AspNetCore.Http.HttpRequest>();
            var mockResponse = new Mock<Microsoft.AspNetCore.Http.HttpResponse>();
            var mockCookies = new Mock<Microsoft.AspNetCore.Http.IRequestCookieCollection>();
            var mockSession = new Mock<ISession>();


            // Setup the session mock to return false for TryGetValue and the desired value for the key
            var sessionValues = new Dictionary<string, byte[]>
            {
                { "ManagementLinkClicked", System.Text.Encoding.UTF8.GetBytes("false") }
            };

            mockSession.Setup(x => x.TryGetValue("ManagementLinkClicked", out It.Ref<byte[]>.IsAny))
            .Returns((string key, out byte[] value) =>
            {
                var result = sessionValues.TryGetValue(key, out value);
                return result;
            });

            mockSession.Setup(x => x.Set("ManagementLinkClicked", It.IsAny<byte[]>()))
            .Callback<string, byte[]>((key, value) => sessionValues[key] = value);


            mockHttpContext.Setup(x => x.Session).Returns(mockSession.Object);
            mockHttpContext.Setup(x => x.Request).Returns(mockRequest.Object);
            mockHttpContext.Setup(x => x.Response).Returns(mockResponse.Object);
            mockRequest.Setup(x => x.Cookies).Returns(mockCookies.Object);

            mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext.Object);

 
            _travelDocumentController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Arrange
            var tempData = new TempDataDictionary(mockHttpContext.Object, Mock.Of<ITempDataProvider>());
            var magicWordViewModel = new MagicWordViewModel { HasUserPassedPasswordCheck = false };
            tempData.SetHasUserUsedMagicWord(magicWordViewModel);
            _travelDocumentController.TempData = tempData;

            // Act
            var result = _travelDocumentController.Index().Result as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nameof(TravelDocumentController.Index), result.ActionName);
        }

        [Fact]
        public void If_ManagementLinkClicked_True_RedirectTo_CheckIdm2SignOut()
        {
            // Arrange
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var mockHttpContext = new Mock<HttpContext>();
            var mockRequest = new Mock<Microsoft.AspNetCore.Http.HttpRequest>();
            var mockResponse = new Mock<Microsoft.AspNetCore.Http.HttpResponse>();
            var mockCookies = new Mock<Microsoft.AspNetCore.Http.IRequestCookieCollection>();
            var mockSession = new Mock<ISession>();


            // Setup the session mock to return true for TryGetValue and the desired value for the key
            var sessionValues = new Dictionary<string, byte[]>
            {
                { "ManagementLinkClicked", System.Text.Encoding.UTF8.GetBytes("true") }
            };

            mockSession.Setup(x => x.TryGetValue("ManagementLinkClicked", out It.Ref<byte[]>.IsAny))
            .Returns((string key, out byte[] value) =>
            {
                var result = sessionValues.TryGetValue(key, out value);
                return result;
            });

            mockSession.Setup(x => x.Set("ManagementLinkClicked", It.IsAny<byte[]>()))
            .Callback<string, byte[]>((key, value) => sessionValues[key] = value);


            mockHttpContext.Setup(x => x.Session).Returns(mockSession.Object);
            mockHttpContext.Setup(x => x.Request).Returns(mockRequest.Object);
            mockHttpContext.Setup(x => x.Response).Returns(mockResponse.Object);
            mockRequest.Setup(x => x.Cookies).Returns(mockCookies.Object);

            mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext.Object);


            _travelDocumentController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Arrange
            var tempData = new TempDataDictionary(mockHttpContext.Object, Mock.Of<ITempDataProvider>());
            var magicWordViewModel = new MagicWordViewModel { HasUserPassedPasswordCheck = false };
            tempData.SetHasUserUsedMagicWord(magicWordViewModel);
            _travelDocumentController.TempData = tempData;

            // Act
            var result = _travelDocumentController.Index().Result as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("CheckIdm2SignOut", result.ActionName);
        }


        [Fact]
        public void If_HasUserPassedPasswordCheck_True_Returns_View()
        {
            // Arrange
            var tempData = new TempDataDictionary(Mock.Of<Microsoft.AspNetCore.Http.HttpContext>(), Mock.Of<ITempDataProvider>());
            var magicWordViewModel = new MagicWordViewModel { HasUserPassedPasswordCheck = true };
            tempData.SetHasUserUsedMagicWord(magicWordViewModel);
            _travelDocumentController.TempData = tempData;
            MockHttpContext();
            _mockMediator.Setup(x => x.Send(It.IsAny<AddUserRequest>(), CancellationToken.None))
               .ReturnsAsync(new AddUserResponse
               {
                   IsSuccess = true
               });
            _mockMediator.Setup(x => x.Send(It.IsAny<GetApplicationsQueryRequest>(), CancellationToken.None))
             .ReturnsAsync(new GetApplicationsQueryResponse
             {
                 Applications = [new ApplicationSummaryDto { ApplicationId = Guid.NewGuid() }]
             });
            // Act
            var result = _travelDocumentController.Index().Result as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Theory]
        [InlineData("404", "Not Found", System.Net.HttpStatusCode.NotFound)]
        [InlineData("500", "Internal Server Error", System.Net.HttpStatusCode.InternalServerError)]
        [InlineData("500", "unexpected Error", null)]
        public void If_HasUserPassedPasswordCheck_True_Returns_View_Error_Code(string expectedErrorCode, string errorMessage, HttpStatusCode? statusCode)
        {
            // Arrange
            var tempData = new TempDataDictionary(Mock.Of<Microsoft.AspNetCore.Http.HttpContext>(), Mock.Of<ITempDataProvider>());
            var magicWordViewModel = new MagicWordViewModel { HasUserPassedPasswordCheck = true };
            tempData.SetHasUserUsedMagicWord(magicWordViewModel);
            _travelDocumentController.TempData = tempData;
            MockHttpContext();
            _mockMediator.Setup(x => x.Send(It.IsAny<AddUserRequest>(), CancellationToken.None))
               .ReturnsAsync(new AddUserResponse
               {
                   IsSuccess = true
               });
            _mockMediator.Setup(x => x.Send(It.IsAny<GetApplicationsQueryRequest>(), CancellationToken.None))
              .ThrowsAsync(new HttpRequestException(errorMessage, null, statusCode));

            // Act
            var result = _travelDocumentController.Index().Result as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("HandleError", result.ActionName);
            Assert.Equal("Error", result.ControllerName);
            Assert.Equal(expectedErrorCode, result.RouteValues.Values.FirstOrDefault().ToString());
        }

        [Fact]
        public void ApplicationDetailRecord_WithValidModel_RedirectsTo_ApplicationCertificate()
        {
            //Arrange
            MockHttpContext();

            // Act
            var result = _travelDocumentController.ApplicationDetailRecord("1", AppConstants.ApplicationStatus.APPROVED) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nameof(TravelDocumentController.ApplicationCertificate), result.ActionName);
        }
        [Fact]
        public void ApplicationDetailRecord_WithValidModel_RedirectsTo_ApplicationDetails()
        {
            //Arrange
            MockHttpContext();

            // Act
            var result = _travelDocumentController.ApplicationDetailRecord("1", AppConstants.ApplicationStatus.UNSUCCESSFUL) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nameof(TravelDocumentController.ApplicationDetails), result.ActionName);
        }

        [Fact]
        public void GetHttpContext_ShouldReturnHttpContext()
        {
            // Arrange

            var expectedHttpContext = new DefaultHttpContext();
            _travelDocumentController.ControllerContext.HttpContext = expectedHttpContext;

            // Act
            var result = _travelDocumentController.GetHttpContext();

            // Assert
            Assert.Equal(expectedHttpContext, result);
        }

        [Fact]
        public void CurrentUserContactId_WhenUserIsAuthenticated_ShouldReturnContactId()
        {
            // Arrange
            var expectedContactId = new Guid("00000000-0000-0000-0000-000000000000");
            var identity = new ClaimsIdentity(
            [
                new Claim("contactId", expectedContactId.ToString())
            ]);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _travelDocumentController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };

            // Act
            var result = _travelDocumentController.CurrentUserContactId();

            // Assert
            Assert.Equal(expectedContactId, result);
        }

        [Fact]
        public void CurrentUserContactId_WhenUserIsNotAuthenticated_ShouldReturnEmptyGuid()
        {
            // Arrange
            _travelDocumentController.ControllerContext.HttpContext = new DefaultHttpContext();

            // Act
            var result = _travelDocumentController.CurrentUserContactId();

            // Assert
            Assert.Equal(Guid.Empty, result);
        }

        [Fact]
        public void GetCurrentUserInfo_WhenUserIsAuthenticated_ShouldReturnUserWithClaims()
        {
            // Arrange
            var expectedUser = new User
            {
                ContactId = "12345678",
                UniqueReference = "ABC123",
                FirstName = "John",
                LastName = "Doe",
                EmailAddress = "john.doe@example.com",
                Role = "Admin"
            };
            var identity = new ClaimsIdentity(
            [
                new Claim("contactId", expectedUser.ContactId),
                new Claim("uniqueReference", expectedUser.UniqueReference),
                new Claim("firstName", expectedUser.FirstName),
                new Claim("lastName", expectedUser.LastName),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", expectedUser.EmailAddress),
                new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", expectedUser.Role)
            ]);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _travelDocumentController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            };

            // Act
            var result = _travelDocumentController.GetCurrentUserInfo();

            // Assert
            Assert.Equal(expectedUser.ContactId, result.ContactId);
            Assert.Equal(expectedUser.UniqueReference, result.UniqueReference);
            Assert.Equal(expectedUser.FirstName, result.FirstName);
            Assert.Equal(expectedUser.LastName, result.LastName);
            Assert.Equal(expectedUser.EmailAddress, result.EmailAddress);
            Assert.Equal(expectedUser.Role, result.Role);
        }

        [Fact]
        public void GetCurrentUserInfo_WhenUserIsNotAuthenticated_ShouldReturnEmptyUser()
        {
            // Arrange
            _travelDocumentController.ControllerContext.HttpContext = new DefaultHttpContext();

            // Act
            var result = _travelDocumentController.GetCurrentUserInfo();

            // Assert
            Assert.NotNull(result);
        }

        private void MockHttpContext()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "http";
            httpContext.Request.Host = new HostString("example.com");
            var session = new MockHttpSession();
            httpContext.Session = session;

            var identityMock = new Mock<ClaimsIdentity>();
            identityMock.SetupGet(i => i.IsAuthenticated).Returns(true);
            var identities = new List<ClaimsIdentity>();
            // Create claims
            var claims = new List<Claim>
            {
                new("contactId", "123"),
                new("uniqueReference", "abc"),
                new("firstName", "John"),
                new("lastName", "Doe"),
                new(ClaimTypes.Email, "john.doe@example.com"),
                new(ClaimTypes.Role, "Admin")
            };

            identities.Add(identityMock.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));
            httpContext.User = user;
            _travelDocumentController.ControllerContext.HttpContext = httpContext;
        }
    }

    public class MockHttpSession : ISession
    {
        // Implement the methods and properties of ISession interface
        // For example, you can use a Dictionary to simulate session data

        // Here's a simple implementation for demonstration purposes
        private readonly Dictionary<string, byte[]> _sessionData = [];

        public byte[] this[string key]
        {
            get => _sessionData.TryGetValue(key, out var value) ? value : null;
            set => _sessionData[key] = value;
        }

        public IEnumerable<string> Keys => _sessionData.Keys;

        public string Id { get; set; }

        public bool IsAvailable => throw new NotImplementedException();

        public bool TryGetValue(string key, out byte[] value) => _sessionData.TryGetValue(key, out value);

        public void Set(string key, byte[] value) => _sessionData[key] = value;

        public void Remove(string key) => _sessionData.Remove(key);

        public Task LoadAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        // Implement other methods and properties as needed
    }

}
