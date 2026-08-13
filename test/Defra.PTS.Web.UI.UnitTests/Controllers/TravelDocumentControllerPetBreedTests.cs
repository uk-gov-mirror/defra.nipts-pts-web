using Defra.PTS.Web.Application.DTOs.Features;
using Defra.PTS.Web.Application.DTOs.Services;
using Defra.PTS.Web.Application.Features.Lookups.Queries;
using Defra.PTS.Web.Application.Features.Users.Queries;
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
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using System.Globalization;

namespace Defra.PTS.Web.UI.UnitTests.Controllers
{
    public class TravelDocumentControllerPetBreedTests
    {
        private readonly Mock<IValidationService> _mockValidationService = new();
        private readonly Mock<IMediator> _mockMediator = new();
        private readonly Mock<ILogger<TravelDocumentController>> _mockLogger = new();
        private readonly Mock<IOptions<PtsSettings>> _mockPtsSettings = new();
        private readonly Mock<ISelectListLocaliser> _mockSelectListLocaliser = new();
        private Mock<TravelDocumentController> _travelDocumentController;
        private readonly IStringLocalizer<ISharedResource> _localizer;
        public TravelDocumentControllerPetBreedTests()
        {
            var options = Options.Create(new LocalizationOptions { ResourcesPath = "Resources" });
            var factory = new ResourceManagerStringLocalizerFactory(options, NullLoggerFactory.Instance);
            _localizer = new StringLocalizer<ISharedResource>(factory);
            // Arrange
            var tempData = new TempDataDictionary(Mock.Of<Microsoft.AspNetCore.Http.HttpContext>(), Mock.Of<ITempDataProvider>());              
            _travelDocumentController = new Mock<TravelDocumentController>(_mockValidationService.Object, _mockMediator.Object, _mockLogger.Object, _mockPtsSettings.Object, _mockSelectListLocaliser.Object, _localizer)
            {
                
                CallBase = true
            };
            _travelDocumentController.Object.TempData = tempData;

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(_ => _.Request.Headers.Referer).Returns("aaa");
            _travelDocumentController.Object.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };
        }

        [Fact]
        public async Task PetBreed_Returns_RedirectToAction_When_Application_NotInProgress()
        {
            // Arrange
            _travelDocumentController.Setup(x => x.IsApplicationInProgress())
                .Returns(false);

            // Act
            var result = (await _travelDocumentController.Object.PetBreed()) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nameof(TravelDocumentController.Index), result.ActionName);
        }

        [Fact]
        public async Task PetBreed_Returns_RedirectToAction_When_Page_Does_Not_Meet_PreConditions()
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
                    IsCompleted = false,
                }
              
            };

            _travelDocumentController.Setup(x => x.GetFormData(false))
                 .Returns(formData);

            // Act
            var result = (await _travelDocumentController.Object.PetBreed()) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);

        }

        [Fact]
        public async Task PetBreed_Returns_ViewResult_When_Page_Meets_PreConditions_PetBreed_Not_Matching()
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
                PetBreed = new PetBreedViewModel
                {
                    BreedId = 2,
                    BreedName = "Test2",
                    IsCompleted = true,
                },

            };

            _mockSelectListLocaliser.Setup(x => x.GetBreedList(It.IsAny<PetSpecies>()))
               .ReturnsAsync(
               [
                   new()
                   {
                       BreedId = 2,
                       BreedName = "Test2",
                       Group = "test"
                   }
            ]);

            _travelDocumentController.Setup(x => x.GetFormData(false))
                 .Returns(formData);

            // Act
            var result = (await _travelDocumentController.Object.PetBreed()) as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task PetBreed_Returns_ViewResult_When_Page_Meets_PreConditions_PetBreed_Matching_With_Additional_Info()
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
                PetBreed = new PetBreedViewModel
                {
                    BreedId = 2,
                    BreedName = "Test",
                    BreedAdditionalInfo= "Test Add Info",
                    IsCompleted = true,
                },

            };

            _mockSelectListLocaliser.Setup(x => x.GetBreedList(It.IsAny<PetSpecies>()))
                .ReturnsAsync(
                [
                   new()
                   {
                       BreedId = 2,
                       BreedName = "test",
                       Group = "test"
                   }
             ]);

            _travelDocumentController.Setup(x => x.GetFormData(false))
                 .Returns(formData);

            // Act
            var result = (await _travelDocumentController.Object.PetBreed()) as ViewResult;

            // Assert
            Assert.NotNull(result);
        }


        [Fact]
        public async Task PetBreed_Returns_ViewResult_With_Correctly_Ordered_BreedList_When_User_Is_Welsh()
        {
            // Arrange
            Thread.CurrentThread.CurrentCulture = new CultureInfo("cy-GB"); // Set culture to Welsh

            // Arrange
            _travelDocumentController.Setup(x => x.IsApplicationInProgress())
               .Returns(true);

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
                    PetSpecies = PetSpecies.Cat,
                    IsCompleted = true,
                },
                PetBreed = new PetBreedViewModel
                {
                    BreedId = 2,
                    BreedName = "Test",
                    BreedAdditionalInfo = "Test Add Info",
                    IsCompleted = true,
                },

            };

            _mockSelectListLocaliser.Setup(x => x.GetBreedList(It.IsAny<PetSpecies>()))
                .ReturnsAsync(
                [
                   new()
                   {
                       BreedId = 2,
                       BreedName = "test",
                       Group = "test"
                   }
             ]);

            _travelDocumentController.Setup(x => x.GetFormData(false))
                 .Returns(formData);

            // Act
            var result = (await _travelDocumentController.Object.PetBreed()) as ViewResult;

            // Assert
            Assert.NotNull(result);
        }


        [Fact]
        public async Task PetBreed_WithValidModel_If_BreedName_Matches_RedirectsToPetName()
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

                    BreedId = 0,
                    BreedName = "Test",
                    BreedAdditionalInfo = "Test Add Info",
                    PetSpecies= PetSpecies.Dog,
                    IsCompleted = true,
                },
                

            };

            _mockSelectListLocaliser.Setup(x => x.GetBreedList(It.IsAny<PetSpecies>()))
               .ReturnsAsync(
               [
                   new()
                   {
                       BreedId = 1,
                       BreedName = "test",
                       Group = "test"
                   }
            ]);

            _travelDocumentController.Setup(x => x.GetFormData(false))
                .Returns(formData);

            // Act
            var result = (await _travelDocumentController.Object.PetBreed(formData.PetBreed)) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nameof(TravelDocumentController.PetName), result.ActionName);
        }

        [Fact]
        public async Task PetBreed_WithValidModel_If_BreedName_Matches_RedirectsToPetName_Welsh()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("cy-GB"); // Set culture to Welsh
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

                    BreedId = 40,
                    BreedName = "Test",
                    BreedAdditionalInfo = "Test Add Info",
                    PetSpecies = PetSpecies.Dog,
                    IsCompleted = true,
                },


            };

            _mockSelectListLocaliser.Setup(x => x.GetBreedListWithoutLocalisation(It.IsAny<PetSpecies>()))
               .ReturnsAsync(
               [
                   new()
                   {
                       BreedId = 40,
                       BreedName = "test",
                       Group = "test"
                   }
            ]);

            _mockSelectListLocaliser.Setup(x => x.GetBreedList(It.IsAny<PetSpecies>()))
               .ReturnsAsync(
               [
                   new()
                   {
                       BreedId = 40,
                       BreedName = "test",
                       Group = "test"
                   }
            ]);


            _travelDocumentController.Setup(x => x.GetFormData(false))
                .Returns(formData);

            // Act
            var result = (await _travelDocumentController.Object.PetBreed(formData.PetBreed)) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nameof(TravelDocumentController.PetName), result.ActionName);
        }

        [Fact]
        public async Task PetBreed_WithValidModel_If_BreedNameIsMissed_MatchesToId_RedirectsToPetName()
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

                    BreedId = 10,
                    BreedName = "",
                    BreedAdditionalInfo = "Test Add Info",
                    PetSpecies = PetSpecies.Dog,
                    IsCompleted = true,
                },


            };

            _mockSelectListLocaliser.Setup(x => x.GetBreedList(It.IsAny<PetSpecies>()))
               .ReturnsAsync(
               [
                   new()
                   {
                       BreedId = 10,
                       BreedName = "test",
                       Group = "test"
                   }
            ]);

            _travelDocumentController.Setup(x => x.GetFormData(false))
                .Returns(formData);

            // Act
            var result = (await _travelDocumentController.Object.PetBreed(formData.PetBreed)) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nameof(TravelDocumentController.PetName), result.ActionName);
        }

        [Fact]
        public async Task PetBreed_WithValidModel_If_BreedName_Does_Not_Matches_RedirectsToPetName()
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

                    BreedId = 0,
                    BreedName = "Test3",
                    BreedAdditionalInfo = "Test Add Info",
                    PetSpecies = PetSpecies.Dog,
                    IsCompleted = true,
                },


            };

            _mockSelectListLocaliser.Setup(x => x.GetBreedList(It.IsAny<PetSpecies>()))
                .ReturnsAsync(
                [
                   new()
                   {
                       BreedId = 1,
                       BreedName = "test",
                       Group = "test"
                   }
             ]);

            _travelDocumentController.Setup(x => x.GetFormData(false))
                .Returns(formData);

            // Act
            var result = (await _travelDocumentController.Object.PetBreed(formData.PetBreed)) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nameof(TravelDocumentController.PetName), result.ActionName);

        }

        [Fact]
        public async Task PetBreed_WithValidModel_BreedIdEquatlTo300_If_BreedName_Matches_RedirectsToPetName()
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

                    BreedId = 300,
                    BreedName = "Test",
                    BreedAdditionalInfo = "Test Add Info",
                    PetSpecies = PetSpecies.Dog,
                    IsCompleted = true,
                },


            };

            _mockSelectListLocaliser.Setup(x => x.GetBreedList(It.IsAny<PetSpecies>()))
               .ReturnsAsync(
               [
                   new()
                   {
                       BreedId = 300,
                       BreedName = "Test",
                       Group = "test"
                   }
            ]);

            _travelDocumentController.Setup(x => x.GetFormData(false))
                .Returns(formData);

            // Act
            var result = (await _travelDocumentController.Object.PetBreed(formData.PetBreed)) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nameof(TravelDocumentController.PetName), result.ActionName);

        }

        [Fact]
        public async Task PetBreed_WithValidModel_BreedIdEquatlTo300_If_BreedName_Matches_RedirectsToPetName_Welsh()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("cy-GB"); // Set culture to Welsh

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

                    BreedId = 100,
                    BreedName = "Test",
                    BreedAdditionalInfo = "Test Add Info",
                    PetSpecies = PetSpecies.Dog,
                    IsCompleted = true,
                },


            };

            _mockSelectListLocaliser.Setup(x => x.GetBreedList(It.IsAny<PetSpecies>()))
               .ReturnsAsync(
               [
                   new()
                   {
                       BreedId = 100,
                       BreedName = "Test",
                       Group = "test"
                   }
            ]);

            _mockSelectListLocaliser.Setup(x => x.GetBreedListWithoutLocalisation(It.IsAny<PetSpecies>()))
               .ReturnsAsync(
               [
                   new()
                   {
                       BreedId = 100,
                       BreedName = "Test",
                       Group = "test"
                   }
            ]);

            _travelDocumentController.Setup(x => x.GetFormData(false))
                .Returns(formData);

            // Act
            var result = (await _travelDocumentController.Object.PetBreed(formData.PetBreed)) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nameof(TravelDocumentController.PetName), result.ActionName);

        }

        [Fact]
        public async Task PetBreed_WithValidModel__BreedIdEquatlTo300_If_BreedName_Does_Not_Matches_RedirectsToPetName()
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

                    BreedId = 0,
                    BreedName = "Test3",
                    BreedAdditionalInfo = "Test Add Info",
                    PetSpecies = PetSpecies.Dog,
                    IsCompleted = true,
                },


            };

            _mockSelectListLocaliser.Setup(x => x.GetBreedList(It.IsAny<PetSpecies>()))
               .ReturnsAsync(
               [
                   new()
                   {
                       BreedId = 300,
                       BreedName = "test",
                       Group = "test"
                   }
            ]);

            _travelDocumentController.Setup(x => x.GetFormData(false))
                .Returns(formData);

            // Act
            var result = (await _travelDocumentController.Object.PetBreed(formData.PetBreed)) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nameof(TravelDocumentController.PetName), result.ActionName);

        }


    }

}
