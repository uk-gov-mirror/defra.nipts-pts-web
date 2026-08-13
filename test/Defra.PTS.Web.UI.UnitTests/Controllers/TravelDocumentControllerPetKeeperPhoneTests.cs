using Defra.PTS.Web.Application.Services.Interfaces;
using Defra.PTS.Web.Domain.Enums;
using Defra.PTS.Web.Domain.Models;
using Defra.PTS.Web.Domain.ViewModels.TravelDocument;
using Defra.PTS.Web.UI.Constants;
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

namespace Defra.PTS.Web.UI.UnitTests.Controllers
{
    public class TravelDocumentControllerPetKeeperPhoneTests
    {
        private readonly Mock<IValidationService> _mockValidationService = new();
        private readonly Mock<IMediator> _mockMediator = new();
        private readonly Mock<ILogger<TravelDocumentController>> _mockLogger = new();
        private readonly Mock<IOptions<PtsSettings>> _mockPtsSettings = new();
        private readonly Mock<ISelectListLocaliser> _mockBreedHelper = new();
        private Mock<TravelDocumentController> _travelDocumentController;
        private Mock<TravelDocumentViewModel> _travelDocumentViewModel;
        private readonly IStringLocalizer<ISharedResource> _localizer;
        public TravelDocumentControllerPetKeeperPhoneTests()
        {
            var options = Options.Create(new LocalizationOptions { ResourcesPath = "Resources" });
            var factory = new ResourceManagerStringLocalizerFactory(options, NullLoggerFactory.Instance);
            _localizer = new StringLocalizer<ISharedResource>(factory);
            // Arrange
            var tempData = new TempDataDictionary(Mock.Of<Microsoft.AspNetCore.Http.HttpContext>(), Mock.Of<ITempDataProvider>());              
            _travelDocumentController = new Mock<TravelDocumentController>(_mockValidationService.Object, _mockMediator.Object, _mockLogger.Object, _mockPtsSettings.Object, _mockBreedHelper.Object, _localizer)
            {
                
                CallBase = true
            };
            _travelDocumentController.Object.TempData = tempData;
            _travelDocumentViewModel = new Mock<TravelDocumentViewModel>();

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(_ => _.Request.Headers.Referer).Returns("aaa");
            _travelDocumentController.Object.ControllerContext = new ControllerContext();
            _travelDocumentController.Object.ControllerContext.HttpContext = mockHttpContext.Object;
        }


        [Fact]
        public void PetKeeperPhone_Returns_RedirectToAction_When_Application_NotInProgress()
        {
            // Arrange
            _travelDocumentController.Setup(x => x.IsApplicationInProgress())
                .Returns(false);

            // Act
            var result = _travelDocumentController.Object.PetKeeperPhone() as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nameof(TravelDocumentController.Index), result.ActionName);
        }

        

        [Fact]
        public void PetKeeperPhone_Returns_RedirectToAction_When_Page_Meet_PreConditions()
        {
            // Arrange
            _travelDocumentController.Setup(x => x.IsApplicationInProgress())
               .Returns(true);

            var formData = new TravelDocumentViewModel
            {
                PetKeeperUserDetails = new PetKeeperUserDetailsViewModel
                {
                    PetOwnerDetailsRequired = false,
                    IsCompleted = true,
                },
                PetKeeperName = new PetKeeperNameViewModel
                {
                    IsCompleted = true,
                }
            };

            _travelDocumentController.Setup(x => x.GetFormData(false))
                 .Returns(formData);

            // Act
            var result = _travelDocumentController.Object.PetKeeperPhone() as ViewResult;

            // Assert
            Assert.NotNull(result);            

        }

        [Fact]
        public void PetKeeperPhone_WithValidModel_RedirectsToPetMicrochip()
        {
            // Arrange                                 
              var formData = new TravelDocumentViewModel
            {
                PetKeeperUserDetails = new PetKeeperUserDetailsViewModel
                {
                    PetOwnerDetailsRequired = true,
                    IsCompleted = true,
                },
                PetKeeperName = new PetKeeperNameViewModel
                {
                    IsCompleted = true,
                },
                PetKeeperPhone = new PetKeeperPhoneViewModel
                {
                    IsCompleted= true,
                }
            };
            _travelDocumentController.Setup(x => x.GetFormData(false))
                .Returns(formData);

            // Act
            var result = _travelDocumentController.Object.PetKeeperPhone(formData.PetKeeperPhone) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nameof(TravelDocumentController.PetMicrochip), result.ActionName);
        }


    }

}
