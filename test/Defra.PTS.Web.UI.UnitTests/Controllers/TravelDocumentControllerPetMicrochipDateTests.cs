using Defra.PTS.Web.Application.Services.Interfaces;
using Defra.PTS.Web.Domain.Models;
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

namespace Defra.PTS.Web.UI.UnitTests.Controllers
{
    public class TravelDocumentControllerPetMicrochipDateTests
    {
        private readonly Mock<IValidationService> _mockValidationService = new();
        private readonly Mock<IMediator> _mockMediator = new();
        private readonly Mock<ILogger<TravelDocumentController>> _mockLogger = new();
        private readonly Mock<IOptions<PtsSettings>> _mockPtsSettings = new();
        private readonly Mock<ISelectListLocaliser> _mockBreedHelper = new();
        private Mock<TravelDocumentController> _travelDocumentController;
        private readonly IStringLocalizer<ISharedResource> _localizer;
        public TravelDocumentControllerPetMicrochipDateTests()
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

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(_ => _.Request.Headers.Referer).Returns("aaa");
            _travelDocumentController.Object.ControllerContext = new ControllerContext();
            _travelDocumentController.Object.ControllerContext.HttpContext = mockHttpContext.Object;
        }


        [Fact]
        public void PetMicrochipDate_Returns_RedirectToAction_When_Application_NotInProgress()
        {
            // Arrange
            _travelDocumentController.Setup(x => x.IsApplicationInProgress())
                .Returns(false);

            // Act
            var result = _travelDocumentController.Object.PetMicrochipDate(1) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nameof(TravelDocumentController.Index), result.ActionName);
        }

        

        [Fact]
        public void PetMicrochipDate_Returns_RedirectToAction_When_Page_Does_Not_Meet_PreConditions()
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
                PetKeeperName = new PetKeeperNameViewModel
                {
                    IsCompleted = true,
                },
                PetKeeperPostcode = new PetKeeperPostcodeViewModel
                {
                    IsCompleted = true,
                },
                PetKeeperPhone = new PetKeeperPhoneViewModel
                {
                    IsCompleted = true,
                },
                PetMicrochip = new PetMicrochipViewModel
                {
                    IsCompleted = false,
                },              

            };

            _travelDocumentController.Setup(x => x.GetFormData(false))
                 .Returns(formData);
           
            // Act
            var result = _travelDocumentController.Object.PetMicrochipDate(1) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);            

        }

        [Fact]
        public void PetMicrochipDate_Returns_ViewResult_When_Page_Meets_PreConditions_Completed_True()
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
                PetKeeperName = new PetKeeperNameViewModel
                {
                    IsCompleted = true,
                },
                PetKeeperPostcode = new PetKeeperPostcodeViewModel
                {
                    IsCompleted = true,
                },
                PetKeeperPhone = new PetKeeperPhoneViewModel
                {
                    IsCompleted = true,
                },
                PetMicrochip = new PetMicrochipViewModel
                {
                    IsCompleted = true,
                },
                PetAge = new PetAgeViewModel
                {
                    IsCompleted = true,
                    Day = "01",
                    Month = "1",
                    Year="2022",
                    
                }

            };

            _travelDocumentController.Setup(x => x.GetFormData(false))
                 .Returns(formData);

            // Act
            var result = _travelDocumentController.Object.PetMicrochipDate(1) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("01/01/2022",formData.PetMicrochipDate.BirthDate?.ToString("MM/dd/yyyy"));
        }

        [Fact]
        public void PetMicrochipDate_Returns_ViewResult_When_Page_Meets_PreConditions_Completed_False()
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
                PetKeeperName = new PetKeeperNameViewModel
                {
                    IsCompleted = true,
                },
                PetKeeperPostcode = new PetKeeperPostcodeViewModel
                {
                    IsCompleted = true,
                },
                PetKeeperPhone = new PetKeeperPhoneViewModel
                {
                    IsCompleted = true,
                },
                PetMicrochip = new PetMicrochipViewModel
                {
                    IsCompleted = true,
                },
                PetAge = new PetAgeViewModel
                {
                    IsCompleted = false,
                    Day = "01",
                    Month = "1",
                    Year = "2022",

                }

            };

            _travelDocumentController.Setup(x => x.GetFormData(false))
                 .Returns(formData);

            // Act
            var result = _travelDocumentController.Object.PetMicrochipDate(1) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(null, formData.PetMicrochipDate.BirthDate);
        }

        [Fact]
        public void PetMicrochipDate_WithValidModel_RedirectsToPetSpecies()
        {
            _travelDocumentController
                .Setup(x => x.IsApplicationInProgress())
                .Returns(true);

            // Arrange                                 
            var formData = new TravelDocumentViewModel
            {
                PetKeeperUserDetails = new PetKeeperUserDetailsViewModel
                {
                    IsCompleted = true,
                },
                PetKeeperName = new PetKeeperNameViewModel
                {
                    IsCompleted = true,
                },
                PetKeeperPostcode = new PetKeeperPostcodeViewModel
                {
                    IsCompleted = true,
                },
                PetKeeperPhone = new PetKeeperPhoneViewModel
                {
                    IsCompleted = true,
                },
                PetMicrochip = new PetMicrochipViewModel
                {
                    IsCompleted = true,
                },
                PetAge = new PetAgeViewModel
                {
                    IsCompleted = true,
                    Day = "01",
                    Month = "1",
                    Year = "2022",

                },
                PetMicrochipDate = new PetMicrochipDateViewModel
                {
                    IsCompleted = true,
                    Day = "01",
                    Month = "1",
                    Year = "2022",

                }

            };
            _travelDocumentController.Setup(x => x.GetFormData(false))
                .Returns(formData);

            // Act
            var result = _travelDocumentController.Object.PetMicrochipDate(formData.PetMicrochipDate) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nameof(TravelDocumentController.PetSpecies), result.ActionName);
        }


    }

}
