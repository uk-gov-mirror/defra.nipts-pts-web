using Defra.PTS.Web.Application.DTOs.Features;
using Defra.PTS.Web.Application.Features.Address.Queries;
using Defra.PTS.Web.Application.Features.DynamicsCrm.Commands;
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
    public class TravelDocumentControllerPetKeeperAddressTests
    {
        private readonly Mock<IValidationService> _mockValidationService = new();
        private readonly Mock<IMediator> _mockMediator = new();
        private readonly Mock<ILogger<TravelDocumentController>> _mockLogger = new();
        private readonly Mock<IOptions<PtsSettings>> _mockPtsSettings = new();
        private readonly Mock<ISelectListLocaliser> _mockBreedHelper = new();
        private Mock<TravelDocumentController> _travelDocumentController;
        private Mock<TravelDocumentViewModel> _travelDocumentViewModel;
        private readonly IStringLocalizer<ISharedResource> _localizer;
        public TravelDocumentControllerPetKeeperAddressTests()
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
        public async Task PetKeeperAddress_Returns_RedirectToAction_When_Application_NotInProgress()
        {
            // Arrange
            _travelDocumentController.Setup(x => x.IsApplicationInProgress())
                .Returns(false);

            // Act
            var result = (await _travelDocumentController.Object.PetKeeperAddress()) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nameof(TravelDocumentController.Index), result.ActionName);
        }

        

        [Fact]
        public async Task PetKeeperAddress_Returns_RedirectToAction_When_Page_Does_Not_Meet_PreConditions()
        {
            // Arrange
            _travelDocumentController.Setup(x => x.IsApplicationInProgress())
               .Returns(true);

            var formData = new TravelDocumentViewModel
            {
                PetKeeperUserDetails = new PetKeeperUserDetailsViewModel
                {
                    IsCompleted = true,
                    PetOwnerDetailsRequired = true,
                },
                PetKeeperName = new PetKeeperNameViewModel
                {
                    IsCompleted = false,
                },
                PetKeeperAddress = new PetKeeperAddressViewModel
                {
                    IsCompleted = false,
                    Address = "Test",
                    Postcode = "Test",
                    
                }
            };

            _travelDocumentController.Setup(x => x.GetFormData(false))
                 .Returns(formData);

            // Act
            var result = (await _travelDocumentController.Object.PetKeeperAddress()) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);            

        }

        [Fact]
        public async Task PetKeeperAddress_Returns_ViewResult_When_Page_Meets_PreConditions_Redirect_PetKeeperPostcode()
        {
            // Arrange
            _travelDocumentController.Setup(x => x.IsApplicationInProgress())
               .Returns(true);

            _travelDocumentController.Setup(x => x.IsApplicationInProgress())
              .Returns(true);

            _mockMediator.Setup(x => x.Send(It.IsAny<ValidateGreatBritianAddressRequest>(), CancellationToken.None))
                 .ReturnsAsync(true);

            var formData = new TravelDocumentViewModel
            {
                PetKeeperUserDetails = new PetKeeperUserDetailsViewModel
                {
                    IsCompleted = true,
                    PetOwnerDetailsRequired = false,
                },
                PetKeeperName = new PetKeeperNameViewModel
                {
                    IsCompleted = true,
                },
                PetKeeperAddress = new PetKeeperAddressViewModel
                {
                    IsCompleted = false,
                    Address = "Test",
                    Postcode = "Test",
                    PostcodeRegion = PostcodeRegion.NonGB
                },
                PetKeeperPostcode = new PetKeeperPostcodeViewModel
                {
                    IsCompleted = false,
                    Postcode = "RM16 2 NT"
                }
            };

            _travelDocumentController.Setup(x => x.GetFormData(false))
                 .Returns(formData);

            // Act
            var result = (await _travelDocumentController.Object.PetKeeperAddress()) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nameof(TravelDocumentController.PetKeeperPostcode), result.ActionName);
        }

        [Fact]
        public async Task PetKeeperAddress_Returns_ViewResult_When_Page_Meets_PreConditions()
        {
            // Arrange
            _travelDocumentController.Setup(x => x.IsApplicationInProgress())
               .Returns(true);

            _travelDocumentController.Setup(x => x.IsApplicationInProgress())
              .Returns(true);

            _mockMediator.Setup(x => x.Send(It.IsAny<ValidateGreatBritianAddressRequest>(), CancellationToken.None))
                 .ReturnsAsync(true);

            _mockMediator.Setup(x => x.Send(It.IsAny<AddressLookupRequest>(), CancellationToken.None))
               .ReturnsAsync(new AddressLookupResponse
               {
                   Addresses = new List<Address>()
                   {
                       new Address()
                       {
                           AddressLineOne = "Test",
                           Postcode = "Test"

                       }
                   }
               });

            var formData = new TravelDocumentViewModel
            {
                PetKeeperUserDetails = new PetKeeperUserDetailsViewModel
                {
                    IsCompleted = true,
                    PetOwnerDetailsRequired = false,
                },
                PetKeeperName = new PetKeeperNameViewModel
                {
                    IsCompleted = true,                    
                },
                PetKeeperAddress = new PetKeeperAddressViewModel
                {
                    IsCompleted = false,
                    Address = "Test",
                    Postcode = "Test",
                    PostcodeRegion = PostcodeRegion.GB
                },
                PetKeeperPostcode = new PetKeeperPostcodeViewModel
                {
                    IsCompleted = false,
                    Postcode = "RM16 2 NT"
                }
            };

            _travelDocumentController.Setup(x => x.GetFormData(false))
                 .Returns(formData);

            // Act
            var result = (await _travelDocumentController.Object.PetKeeperAddress()) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(_travelDocumentController.Object.ViewBag.AddressList);
        }

        [Fact]
        public async Task PetKeeperAddress_WithValidModel_RedirectsToPetKeeperPhone()
        {
            // Arrange                                 
            var formData = new TravelDocumentViewModel
            {
                PetKeeperUserDetails = new PetKeeperUserDetailsViewModel
                {
                    IsCompleted = true,
                    PetOwnerDetailsRequired = true,
                },
                PetKeeperName = new PetKeeperNameViewModel
                {
                    IsCompleted = true,
                },
                PetKeeperAddress = new PetKeeperAddressViewModel
                {
                    IsCompleted = true,
                    Postcode = "RM16 2GT"
                    
                }

            };
            _travelDocumentController.Setup(x => x.GetFormData(false))
                .Returns(formData);

            // Act
            var result = (await _travelDocumentController.Object.PetKeeperAddress(formData.PetKeeperAddress)) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nameof(TravelDocumentController.PetKeeperPhone), result.ActionName);
        }


    }

}
