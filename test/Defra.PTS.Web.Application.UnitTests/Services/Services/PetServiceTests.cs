using Defra.PTS.Web.Application.DTOs.Services;
using Defra.PTS.Web.Application.Services;
using Defra.PTS.Web.Application.Services.Interfaces;
using Defra.PTS.Web.Domain.DTOs;
using Defra.PTS.Web.Domain.Enums;
using Defra.PTS.Web.Domain.ViewModels.TravelDocument;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Defra.PTS.Web.Application.UnitTests.Services.Services
{
    public class PetServiceTests
    {
        private PetService _sut;
        protected Mock<HttpMessageHandler> _mockHttpMessageHandler = new();
        private readonly Mock<ILogger<PetService>> _mockLogger = new();

        [Fact]
        public async Task GetBreeds_Return_200()
        {
            var breeds = new List<BreedDto> { new BreedDto() { BreedId = 1 } };
            var jsonString = JsonConvert.SerializeObject(breeds);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var expectedResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = httpContent
            };

            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(expectedResponse);
            
            var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://localhost/")
            };

            _sut = new PetService(_mockLogger.Object, httpClient);

            var actualResult = await _sut.GetBreeds(PetSpecies.Dog);

            Assert.Equal(breeds[0].BreedId, actualResult[0].BreedId);
        }

        [Fact]
        public async Task GetBreeds_ThrowsException()
        {                       
            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new Exception("Unable to fetch breeds"));

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://localhost/")
            };

            _sut = new PetService(_mockLogger.Object, httpClient);

            await Assert.ThrowsAsync<Exception>(async () => await _sut.GetBreeds(PetSpecies.Dog));
        }

        [Fact]
        public async Task GetColours_Return_200()
        {
            var colours = new List<ColourDto> { new ColourDto() { Id = "1", Name = "Brown" } };
            var jsonString = JsonConvert.SerializeObject(colours);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            
            var expectedResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = httpContent
            };
            
            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(expectedResponse);

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://localhost/")
            };

            _sut = new PetService(_mockLogger.Object, httpClient);

            var actualResult = await _sut.GetColours(PetSpecies.Dog);

            Assert.Equal(colours[0].Id, actualResult[0].Id);
            Assert.Equal(colours[0].Code, actualResult[0].Code);
        }

        [Fact]
        public async Task GetColours_TestToCode_Return_200()
        {
            var colours = new List<ColourDto> { new ColourDto() { Id = "1", Name = "" } };
            var jsonString = JsonConvert.SerializeObject(colours);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var expectedResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = httpContent
            };

            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(expectedResponse);

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://localhost/")
            };

            _sut = new PetService(_mockLogger.Object, httpClient);

            var actualResult = await _sut.GetColours(PetSpecies.Dog);

            Assert.Equal(colours[0].Id, actualResult[0].Id);
            Assert.Equal(colours[0].Code, actualResult[0].Code);
        }

        [Fact]
        public async Task GetColours_ThrowsException()
        {            
            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new Exception("Unable to fetch colours"));

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://localhost/")
            };

            _sut = new PetService(_mockLogger.Object, httpClient);

            await Assert.ThrowsAsync<Exception>(async () => await _sut.GetColours(PetSpecies.Dog));
        }

        [Fact]
        public async Task CreatePet_Return_Success()
        {
            var expectedResult = Guid.NewGuid();
            var jsonString = JsonConvert.SerializeObject(expectedResult);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var expectedResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Created,
                Content = httpContent
            };

            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(expectedResponse);

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://localhost/")
            };

            _sut = new PetService(_mockLogger.Object, httpClient);

            var actualResult = await _sut.CreatePet(new TravelDocumentViewModel());

            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public async Task CreatePet_ThrowsException()
        {
            // Arrange
            var expectedMessage = "Error";

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new Exception(expectedMessage));

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://localhost/")
            };

            _sut = new PetService(_mockLogger.Object, httpClient);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _sut.CreatePet(new TravelDocumentViewModel()));

            Assert.NotNull(ex);
            Assert.Equal(expectedMessage, ex.Message);
        }

        [Fact]
        public async Task CreatePet_ThrowsHttpRequestException()
        {
            // Arrange
            var expectedMessage = "Unable to create pet, Status code: InternalServerError";

            var expectedResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError                 
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(expectedResponse);

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://localhost/")
            };

            _sut = new PetService(_mockLogger.Object, httpClient);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<HttpRequestException>(() => _sut.CreatePet(new TravelDocumentViewModel()));

            Assert.NotNull(ex);
            Assert.Equal(expectedMessage, ex.Message);
        }
    }
}
