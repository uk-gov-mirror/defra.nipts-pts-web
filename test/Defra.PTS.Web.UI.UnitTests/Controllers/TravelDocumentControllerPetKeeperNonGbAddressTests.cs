using Defra.PTS.Web.Application.DTOs.Features;
using Defra.PTS.Web.Application.DTOs.Services;
using Defra.PTS.Web.Application.Extensions;
using Defra.PTS.Web.Application.Features.Address.Queries;
using Defra.PTS.Web.Application.Features.DynamicsCrm.Commands;
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
using System.Security.Claims;

namespace Defra.PTS.Web.UI.UnitTests.Controllers
{
    public class TravelDocumentControllerPetKeeperNonGbAddressTests
    {
        private readonly Mock<IValidationService> _mockValidationService = new();
        private readonly Mock<IMediator> _mockMediator = new();
        private readonly Mock<ILogger<TravelDocumentController>> _mockLogger = new();
        private readonly Mock<IOptions<PtsSettings>> _mockPtsSettings = new();
        private readonly Mock<ISelectListLocaliser> _mockBreedHelper = new();
        private IOptions<PtsSettings> _optionsPtsSettings;
        private Mock<TravelDocumentController> _travelDocumentController;
        private Mock<ControllerContext> _mockControllerContext;
        private Mock<TravelDocumentViewModel> _travelDocumentViewModel;
        private readonly IStringLocalizer<ISharedResource> _localizer;
        public TravelDocumentControllerPetKeeperNonGbAddressTests()
        {
            var options = Options.Create(new LocalizationOptions { ResourcesPath = "Resources" });
            var factory = new ResourceManagerStringLocalizerFactory(options, NullLoggerFactory.Instance);
            _localizer = new StringLocalizer<ISharedResource>(factory);
            var ptsSettings = new PtsSettings
            {
                MagicWordEnabled = true,
            };
            _optionsPtsSettings = Options.Create(ptsSettings);
            _mockControllerContext = new Mock<ControllerContext>();
            _travelDocumentController = new Mock<TravelDocumentController>(_mockValidationService.Object, _mockMediator.Object, _mockLogger.Object, _optionsPtsSettings, _mockBreedHelper.Object, _localizer)
            {
                CallBase = true
            };
            _travelDocumentViewModel = new Mock<TravelDocumentViewModel>();

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(_ => _.Request.Headers.Referer).Returns("aaa");
            _travelDocumentController.Object.ControllerContext = new ControllerContext();
            _travelDocumentController.Object.ControllerContext.HttpContext = mockHttpContext.Object;
        }


        [Fact]
        public async Task PetKeeperNonGbAddressAsync_Returns_RedirectToAction_When_Application_NotInProgress()
        {
            // Arrange
            var tempData = new TempDataDictionary(Mock.Of<Microsoft.AspNetCore.Http.HttpContext>(), Mock.Of<ITempDataProvider>());
            var magicWordViewModel = new MagicWordViewModel { HasUserPassedPasswordCheck = false };
            tempData.SetHasUserUsedMagicWord(magicWordViewModel);
            _travelDocumentController.Object.TempData = tempData;

            // Act
            var result = (await _travelDocumentController.Object.PetKeeperNonGbAddressAsync()) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nameof(TravelDocumentController.Index), result.ActionName);
        }

        [Fact]
        public async Task PetKeeperNonGbAddressAsync_Returns_RedirectToAction_When_User_IsSuspended()
        {
            // Arrange
            var tempData = new TempDataDictionary(Mock.Of<Microsoft.AspNetCore.Http.HttpContext>(), Mock.Of<ITempDataProvider>());
            var magicWordViewModel = new MagicWordViewModel { HasUserPassedPasswordCheck = true };
            tempData.SetHasUserUsedMagicWord(magicWordViewModel);
            tempData.SetIsUserSuspended(true);
            _travelDocumentController.Object.TempData = tempData;

            // Act
            var result = (await _travelDocumentController.Object.PetKeeperNonGbAddressAsync()) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nameof(TravelDocumentController.Index), result.ActionName);
        }

        [Fact]
        public async Task PetKeeperNonGbAddressAsync_Returns_RedirectToAction_When_Page_PreConditions()
        {
            // Arrange
            var tempData = new TempDataDictionary(Mock.Of<Microsoft.AspNetCore.Http.HttpContext>(), Mock.Of<ITempDataProvider>());
            var magicWordViewModel = new MagicWordViewModel { HasUserPassedPasswordCheck = true };
            tempData.SetHasUserUsedMagicWord(magicWordViewModel);
            tempData.SetIsUserSuspended(false);
            _travelDocumentController.Object.TempData = tempData;

            _mockMediator.Setup(x => x.Send(It.IsAny<AddAddressRequest>(), CancellationToken.None))
                  .ReturnsAsync(new AddAddressResponse
                  {
                      IsSuccess = true
                  });

            _mockMediator.Setup(x => x.Send(It.IsAny<GetUserDetailQueryRequest>(), CancellationToken.None))
                  .ReturnsAsync(new GetUserDetailQueryResponse
                  {
                      UserDetail = new UserDetailDto { FullName = "John Doe" }
                  });
            _mockMediator.Setup(x => x.Send(It.IsAny<ValidateGreatBritianAddressRequest>(), CancellationToken.None))
                   .ReturnsAsync(true);
            _travelDocumentController.Setup(x => x.IsApplicationInProgress())
                .Returns(true);

            MockHttpContext();

            // Arrange
            var mockHttpContext = new Mock<HttpContext>();
            var mockSession = new Mock<ISession>();
            mockHttpContext.SetupGet(x => x.Session).Returns(mockSession.Object);
            mockHttpContext.Setup(_ => _.Request.Headers.Referer).Returns("aaa");

            _travelDocumentController.Object.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            var formData = new TravelDocumentViewModel
            {
                PetKeeperUserDetails = new PetKeeperUserDetailsViewModel
                {
                    IsCompleted = true,
                    PostcodeRegion = PostcodeRegion.GB
                }
            };



            _travelDocumentController.Setup(x => x.GetFormData(false))
                 .Returns(formData);

            // Act
            var result = (await _travelDocumentController.Object.PetKeeperNonGbAddressAsync()) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);

        }


        [Fact]
        public void PetKeeperNonGbAddress_WithValidModel_If_UserDetailsAreCorrect_Yes_RedirectsToPetKeeperName()
        {
            // Arrange
            var tempData = new TempDataDictionary(Mock.Of<Microsoft.AspNetCore.Http.HttpContext>(), Mock.Of<ITempDataProvider>());
            var magicWordViewModel = new MagicWordViewModel { HasUserPassedPasswordCheck = true };
            tempData.SetHasUserUsedMagicWord(magicWordViewModel);
            _travelDocumentController.Object.TempData = tempData;
            var formData = new TravelDocumentViewModel
            {
                PetKeeperUserDetails = new PetKeeperUserDetailsViewModel
                {
                    IsCompleted = true,
                    PostcodeRegion = PostcodeRegion.GB
                }
            };
            // Arrange
            _travelDocumentController.Setup(x => x.IsApplicationInProgress())
                .Returns(false);
            _travelDocumentController.Setup(x => x.GetFormData(false))
                .Returns(formData);

            // Act
            var result = _travelDocumentController.Object.PetKeeperNonGbAddress(formData.PetKeeperUserDetails) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nameof(TravelDocumentController.PetKeeperName), result.ActionName);
        }


        private void MockHttpContext()
        {
            // Arrange
            var mockHttpContext = new Mock<HttpContext>();
            var mockIdentity = new Mock<ClaimsIdentity>();
            var identities = new List<ClaimsIdentity>();
            // Create claims
            var claims = new List<Claim>
            {
                new Claim("contactId", "123"),
                new Claim("uniqueReference", "abc"),
                new Claim("firstName", "John"),
                new Claim("lastName", "Doe"),
                new Claim(ClaimTypes.Email, "john.doe@example.com"),
                new Claim(ClaimTypes.Role, "Admin")
            };


            identities.Add(mockIdentity.Object);
            // Setup ClaimsIdentity
            mockIdentity.SetupGet(i => i.Claims).Returns(claims);
            // Setup User
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));


            mockHttpContext.Setup(c => c.User).Returns(user);
            // Setup Identities
            mockHttpContext.SetupGet(c => c.User.Identity).Returns(mockIdentity.Object);
            //mockHttpContext.SetupGet(c => c.User.Identity.IsAuthenticated).Returns(true);
            //mockHttpContext.SetupGet(c => c.User.GetLoggedInContactId()).Returns(new Guid());
            // Setup Identities
            mockHttpContext.SetupGet(c => c.User.Identities).Returns(identities);
            // Set up the behavior of GetHttpContext method on the controller
            _travelDocumentController.Setup(c => c.GetHttpContext()).Returns(mockHttpContext.Object);
            _travelDocumentController.Setup(c => c.GetHttpContext().Session).Returns(mockHttpContext.Object.Session);

        }



    }

}
