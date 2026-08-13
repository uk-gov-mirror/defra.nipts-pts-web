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
    public class TravelDocumentControllerPetGenderTests
    {
        private readonly Mock<IValidationService> _mockValidationService = new();
        private readonly Mock<IMediator> _mockMediator = new();
        private readonly Mock<ILogger<TravelDocumentController>> _mockLogger = new();
        private readonly Mock<IOptions<PtsSettings>> _mockPtsSettings = new();
        private readonly Mock<ISelectListLocaliser> _mockBreedHelper = new();
        private Mock<TravelDocumentController> _travelDocumentController;
        private Mock<TravelDocumentViewModel> _travelDocumentViewModel;
        private readonly IStringLocalizer<ISharedResource> _localizer;
        public TravelDocumentControllerPetGenderTests()
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
        public void PetGender_Returns_RedirectToAction_When_Application_NotInProgress()
        {
            // Arrange
            _travelDocumentController.Setup(x => x.IsApplicationInProgress())
                .Returns(false);

            // Act
            var result = _travelDocumentController.Object.PetGender() as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nameof(TravelDocumentController.Index), result.ActionName);
        }

        

        [Fact]
        public void PetGender_Returns_RedirectToAction_When_Page_Does_Not_Meet_PreConditions()
        {
            // Arrange
            _travelDocumentController.Setup(x => x.IsApplicationInProgress())
               .Returns(true);

            var formData = new TravelDocumentViewModel
            {
                PetKeeperUserDetails = new PetKeeperUserDetailsViewModel
                {
                    IsCompleted = true,
                },
                PetKeeperName =new PetKeeperNameViewModel { IsCompleted = true },
                PetKeeperPostcode = new PetKeeperPostcodeViewModel { IsCompleted = true },
                PetKeeperPhone = new PetKeeperPhoneViewModel { IsCompleted = true },
                PetMicrochip = new PetMicrochipViewModel
                {
                    IsCompleted = true,
                },
                PetMicrochipDate = new PetMicrochipDateViewModel
                {
                    IsCompleted = true,
                },
                PetSpecies = new PetSpeciesViewModel
                {
                    PetSpecies = PetSpecies.Cat,
                    IsCompleted = true,
                },
                PetBreed = new PetBreedViewModel { IsCompleted = true },
                PetName = new PetNameViewModel { IsCompleted = false },
                PetGender = new PetGenderViewModel { IsCompleted = false },

            };

            _travelDocumentController.Setup(x => x.GetFormData(false))
                 .Returns(formData);

            // Act
            var result = _travelDocumentController.Object.PetGender() as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);            

        }

        [Fact]
        public void PetGender_Returns_ViewResult_When_Page_Meets_PreConditions()
        {
            // Arrange
            _travelDocumentController.Setup(x => x.IsApplicationInProgress())
               .Returns(true);

            var formData = new TravelDocumentViewModel
            {
                PetKeeperUserDetails = new PetKeeperUserDetailsViewModel
                {
                    IsCompleted = true,
                },
                PetKeeperName = new PetKeeperNameViewModel { IsCompleted = true },
                PetKeeperPostcode = new PetKeeperPostcodeViewModel { IsCompleted = true },
                PetKeeperPhone = new PetKeeperPhoneViewModel { IsCompleted = true },
                PetMicrochip = new PetMicrochipViewModel
                {
                    IsCompleted = true,
                },
                PetMicrochipDate = new PetMicrochipDateViewModel
                {
                    IsCompleted = true,
                },
                PetSpecies = new PetSpeciesViewModel
                {
                    PetSpecies = PetSpecies.Cat,
                    IsCompleted = true,
                },
                PetBreed = new PetBreedViewModel { IsCompleted = true },
                PetName = new PetNameViewModel { IsCompleted = true },
                PetGender = new PetGenderViewModel { IsCompleted = true },

            };

            _travelDocumentController.Setup(x => x.GetFormData(false))
                 .Returns(formData);

            // Act
            var result = _travelDocumentController.Object.PetGender() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }
             

        [Fact]
        public void PetGender_WithValidModel_RedirectsToPetAge()
        {
            // Arrange                                 
            var formData = new TravelDocumentViewModel
            {
                PetKeeperUserDetails = new PetKeeperUserDetailsViewModel
                {
                    IsCompleted = true,
                },
                PetMicrochip = new PetMicrochipViewModel
                {
                    IsCompleted = true,
                },
                PetMicrochipDate = new PetMicrochipDateViewModel
                {
                    IsCompleted = true,
                },
                PetSpecies = new PetSpeciesViewModel
                {
                    PetSpecies = PetSpecies.Dog,
                    IsCompleted = true,
                },
                PetBreed = new PetBreedViewModel
                {
                    IsCompleted = true,
                },
                PetGender = new PetGenderViewModel
                {
                    IsCompleted = true
                }

            };
            _travelDocumentController.Setup(x => x.GetFormData(false))
                .Returns(formData);

            // Act
            var result = _travelDocumentController.Object.PetGender(formData.PetGender) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nameof(TravelDocumentController.PetAge), result.ActionName);
        }
    }

}
