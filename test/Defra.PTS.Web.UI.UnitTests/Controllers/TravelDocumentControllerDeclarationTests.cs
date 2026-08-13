using Defra.PTS.Web.Application.DTOs.Features;
using Defra.PTS.Web.Application.DTOs.Services;
using Defra.PTS.Web.Application.Extensions;
using Defra.PTS.Web.Application.Features.TravelDocument.Commands;
using Defra.PTS.Web.Application.Features.TravelDocument.Queries;
using Defra.PTS.Web.Application.Services.Interfaces;
using Defra.PTS.Web.Domain.Enums;
using Defra.PTS.Web.Domain.Models;
using Defra.PTS.Web.Domain.ViewModels.TravelDocument;
using Defra.PTS.Web.UI.Constants;
using Defra.PTS.Web.UI.Controllers;
using Defra.PTS.Web.UI.Helpers;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Azure.Amqp.Transaction;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using NUglify.Helpers;
using System.Security.Claims;
using static Defra.PTS.Web.UI.Constants.WebAppConstants;

namespace Defra.PTS.Web.UI.UnitTests.Controllers
{
    public class TravelDocumentControllerDeclarationTests
    {
        private readonly Mock<IValidationService> _mockValidationService = new();
        private readonly Mock<IMediator> _mockMediator = new();
        private readonly Mock<ILogger<TravelDocumentController>> _mockLogger = new();
        private readonly Mock<IOptions<PtsSettings>> _mockPtsSettings = new();
        private readonly Mock<ISelectListLocaliser> _breedHelper = new();
        private TravelDocumentController _travelDocumentController;

        private readonly IStringLocalizer<ISharedResource> _localizer;
        public TravelDocumentControllerDeclarationTests()
        {
            var options = Options.Create(new LocalizationOptions { ResourcesPath = "Resources" });
            var factory = new ResourceManagerStringLocalizerFactory(options, NullLoggerFactory.Instance);
            _localizer = new StringLocalizer<ISharedResource>(factory);
            // Arrange
            _travelDocumentController = new TravelDocumentController(_mockValidationService.Object, _mockMediator.Object, _mockLogger.Object, _mockPtsSettings.Object, _breedHelper.Object, _localizer);
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(_ => _.Request.Headers.Referer).Returns("aaa");
            _travelDocumentController.ControllerContext = new ControllerContext();
            _travelDocumentController.ControllerContext.HttpContext = mockHttpContext.Object;
        }


        [Fact]
        public void Declaration_Get_ReturnsRedirectToPetKeeperUserDetails_WhenApplicationNotInProgress()
        {
            // Arrange
            var tempDatamodel = new TravelDocumentViewModel
            {
                RequestId = Guid.NewGuid(),
                IsApplicationInProgress = false,
            };

            var tempData = TempData();
            tempData.SetTravelDocument(tempDatamodel);
            _travelDocumentController.TempData = tempData;

            // Act
            var result = _travelDocumentController.Declaration() as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nameof(TravelDocumentController.Index), result.ActionName);
        }

        [Fact]
        public void Declaration_Get_ReturnsRedirectToPetKeeperUserDetails_WhenApplicationInProgress()
        {
            // Arrange
            var tempDatamodel = new TravelDocumentViewModel
            {
                RequestId = Guid.NewGuid(),
                IsApplicationInProgress = true,
            };

            var tempData = TempData();
            tempData.SetTravelDocument(tempDatamodel);
            _travelDocumentController.TempData = tempData;
           

            // Act
            var result = _travelDocumentController.Declaration() as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(nameof(TravelDocumentController.PetKeeperUserDetails));

        }


        [Fact]
        public void Declaration_Get_ReturnsViewWithModel_WhenApplicationInProgress()
        {
            // Arrange
            var tempDatamodel = new TravelDocumentViewModel
            {
                RequestId = Guid.NewGuid(),
                IsApplicationInProgress = true,
                PetKeeperUserDetails = new PetKeeperUserDetailsViewModel() { IsCompleted = true },
                PetMicrochip = new PetMicrochipViewModel() { IsCompleted = true },
                PetMicrochipDate = new PetMicrochipDateViewModel() { IsCompleted = true },
                PetSpecies = new PetSpeciesViewModel() { IsCompleted = true },
                PetName = new PetNameViewModel() { IsCompleted = true },
                PetGender = new PetGenderViewModel() { IsCompleted = true },
                PetAge = new PetAgeViewModel() { IsCompleted = true },
                PetColour = new PetColourViewModel() { IsCompleted = true },
                PetFeature = new PetFeatureViewModel() { IsCompleted = true },
            };


            var tempData = TempData();
            tempData.SetTravelDocument(tempDatamodel);
            _travelDocumentController.TempData = tempData;


            // Act
            var result = _travelDocumentController.Declaration() as ViewResult;
            var model = result.Model as DeclarationViewModel;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(model);

        }


        [Fact]
        public async Task Declaration_Post_ReturnsViewWithModel_WhenModelStateIsInvalid()
        {
            // Arrange
            var tempDatamodel = new TravelDocumentViewModel
            {
                RequestId = Guid.NewGuid(),
                IsApplicationInProgress = true,
                PetKeeperUserDetails = new PetKeeperUserDetailsViewModel() { IsCompleted = true },
                PetMicrochip = new PetMicrochipViewModel() { IsCompleted = true },
                PetMicrochipDate = new PetMicrochipDateViewModel() { IsCompleted = true },
                PetSpecies = new PetSpeciesViewModel() { IsCompleted = true },
                PetName = new PetNameViewModel() { IsCompleted = true },
                PetGender = new PetGenderViewModel() { IsCompleted = true },
                PetAge = new PetAgeViewModel() { IsCompleted = true },
                PetColour = new PetColourViewModel() { IsCompleted = true },
                PetFeature = new PetFeatureViewModel() { IsCompleted = true },
            };


            var tempData = TempData();
            tempData.SetTravelDocument(tempDatamodel);
            Guid submissionId = Guid.NewGuid();
            tempData.AddToFormSubmissionQueue(submissionId);
            _travelDocumentController.TempData = tempData;

            _travelDocumentController.ModelState.AddModelError("PropertyName", "Error Message");
            var model = new DeclarationViewModel
            {
                RequestId = submissionId
            };

            // Act
            var result = await _travelDocumentController.Declaration(model) as ViewResult;
            var returnedModel = result.Model as DeclarationViewModel;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(returnedModel);
        }


        [Fact]
        public async Task Declaration_Post_ReturnsViewWithModel_WhenFormIsSubmittedModelStateIsvalid()
        {
            // Arrange
            var tempDatamodel = new TravelDocumentViewModel
            {
                RequestId = Guid.NewGuid(),
                IsApplicationInProgress = true,
                PetKeeperUserDetails = new PetKeeperUserDetailsViewModel() { IsCompleted = true },
                PetMicrochip = new PetMicrochipViewModel() { IsCompleted = true },
                PetMicrochipDate = new PetMicrochipDateViewModel() { IsCompleted = true },
                PetSpecies = new PetSpeciesViewModel() { IsCompleted = true },
                PetName = new PetNameViewModel() { IsCompleted = true },
                PetGender = new PetGenderViewModel() { IsCompleted = true },
                PetAge = new PetAgeViewModel() { IsCompleted = true },
                PetColour = new PetColourViewModel() { IsCompleted = true },
                PetFeature = new PetFeatureViewModel() { IsCompleted = true },
                IsSubmitted = true,
            };


            var tempData = TempData();
            tempData.SetTravelDocument(tempDatamodel);
            Guid submissionId = Guid.NewGuid();
            tempData.AddToFormSubmissionQueue(submissionId);
            _travelDocumentController.TempData = tempData;

            //_travelDocumentController.ModelState.AddModelError("PropertyName", "Error Message");
            var model = new DeclarationViewModel();
            //model.RequestId = submissionId;

            // Act
            var result = await _travelDocumentController.Declaration(model) as ViewResult;
            var returnedModel = result.Model as DeclarationViewModel;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(returnedModel);
        }


        [Fact]
        public async Task Declaration_Post_ReturnsViewWithModel_WhenFormIsNotSubmittedModelStateIsvalidWithoutValidationError()
        {
            // Arrange
            _mockMediator.Setup(x => x.Send(It.IsAny<CreateTravelDocumentRequest>(), CancellationToken.None))
                 .ReturnsAsync(new CreateTravelDocumentResponse
                 {
                     IsSuccess = true,
                     Reference = "XYZ",
                 });

            var tempDatamodel = new TravelDocumentViewModel
            {
                RequestId = Guid.NewGuid(),
                IsApplicationInProgress = true,
                PetKeeperUserDetails = new PetKeeperUserDetailsViewModel() { IsCompleted = true },
                PetMicrochip = new PetMicrochipViewModel() { IsCompleted = true },
                PetMicrochipDate = new PetMicrochipDateViewModel() { IsCompleted = true },
                PetSpecies = new PetSpeciesViewModel() { IsCompleted = true },
                PetName = new PetNameViewModel() { IsCompleted = true },
                PetGender = new PetGenderViewModel() { IsCompleted = true },
                PetAge = new PetAgeViewModel() { IsCompleted = true },
                PetColour = new PetColourViewModel() { IsCompleted = true },
                PetFeature = new PetFeatureViewModel() { IsCompleted = true },
                IsSubmitted = false,
            };

            var validationResult = new ValidationResult();
            _mockValidationService.Setup(a => a.ValidateTravelDocument(It.IsAny<TravelDocumentViewModel>())).Returns(validationResult);

            //Set up Temp Data
            var tempData = TempData();
            tempData.SetTravelDocument(tempDatamodel);
            Guid submissionId = Guid.NewGuid();
            tempData.AddToFormSubmissionQueue(submissionId);
            _travelDocumentController.TempData = tempData;

            //Set up Http Context of Moq User
            var mockIdentity = new Mock<ClaimsIdentity>();
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
            identities.Add(mockIdentity.Object);

            // Setup ClaimsIdentity
            mockIdentity.SetupGet(i => i.Claims).Returns(claims);
            ClaimsPrincipal user = new(new ClaimsIdentity(claims));

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "http";
            httpContext.Request.Host = new HostString("example.com");
            httpContext.User = user;

            _travelDocumentController.ControllerContext.HttpContext = httpContext;


            var model = new DeclarationViewModel();


            // Act
            var result = await _travelDocumentController.Declaration(model) as RedirectToActionResult;


            // Assert
            Assert.NotNull(result);
            Assert.Equal(nameof(TravelDocumentController.Acknowledgement), result.ActionName);
        }

        [Fact]
        public async Task Declaration_Post_Throws_Error_WhenFormIsNotSubmittedModelStateIsvalidWithoutValidationError()
        {
            // Arrange
            var expectedMessage = "Error Unexpected";

            _mockMediator.Setup(x => x.Send(It.IsAny<CreateTravelDocumentRequest>(), CancellationToken.None))
                .ThrowsAsync(new Exception(expectedMessage));
                 
            var tempDatamodel = new TravelDocumentViewModel
            {
                RequestId = Guid.NewGuid(),
                IsApplicationInProgress = true,
                PetKeeperUserDetails = new PetKeeperUserDetailsViewModel() { IsCompleted = true },
                PetMicrochip = new PetMicrochipViewModel() { IsCompleted = true },
                PetMicrochipDate = new PetMicrochipDateViewModel() { IsCompleted = true },
                PetSpecies = new PetSpeciesViewModel() { IsCompleted = true },
                PetName = new PetNameViewModel() { IsCompleted = true },
                PetGender = new PetGenderViewModel() { IsCompleted = true },
                PetAge = new PetAgeViewModel() { IsCompleted = true },
                PetColour = new PetColourViewModel() { IsCompleted = true },
                PetFeature = new PetFeatureViewModel() { IsCompleted = true },
                IsSubmitted = false,
            };

            var validationResult = new ValidationResult();
            _mockValidationService.Setup(a => a.ValidateTravelDocument(It.IsAny<TravelDocumentViewModel>())).Returns(validationResult);

            //Set up Temp Data
            var tempData = TempData();
            tempData.SetTravelDocument(tempDatamodel);
            Guid submissionId = Guid.NewGuid();
            tempData.AddToFormSubmissionQueue(submissionId);
            _travelDocumentController.TempData = tempData;

            //Set up Http Context of Moq User
            var mockIdentity = new Mock<ClaimsIdentity>();
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
            identities.Add(mockIdentity.Object);

            // Setup ClaimsIdentity
            mockIdentity.SetupGet(i => i.Claims).Returns(claims);
            ClaimsPrincipal user = new(new ClaimsIdentity(claims));

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "http";
            httpContext.Request.Host = new HostString("example.com");
            httpContext.User = user;

            _travelDocumentController.ControllerContext.HttpContext = httpContext;


            var model = new DeclarationViewModel();


            // Act
            var ex = await Assert.ThrowsAsync<Exception>(async () => await _travelDocumentController.Declaration(model));

            Assert.NotNull(ex);
            Assert.Equal(expectedMessage, ex.Message);         
        }

        private static ITempDataDictionary TempData()
        {
            var tempDataProvider = Mock.Of<ITempDataProvider>();
            var tempDataDictionaryFactory = new TempDataDictionaryFactory(tempDataProvider);
            var tempData = tempDataDictionaryFactory.GetTempData(new DefaultHttpContext());

            return tempData;
        }
    }
}
