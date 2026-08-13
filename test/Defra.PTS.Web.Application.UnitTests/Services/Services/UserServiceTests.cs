using Defra.PTS.Web.Application.DTOs.Services;
using Defra.PTS.Web.Application.Services;
using Defra.PTS.Web.Application.Services.Interfaces;
using Defra.PTS.Web.Domain.Models;
using Defra.PTS.Web.Domain.ViewModels.TravelDocument;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Defra.PTS.Web.Application.UnitTests.Services.Services
{
    public class UserServiceTests
    {
        private UserService _sut;
        protected Mock<HttpMessageHandler> _mockHttpMessageHandler = new();
        private readonly Mock<ILogger<UserService>> _mockLogger = new();

        [Fact]
        public async Task AddUser_Return_Success()
        {
            var user = new User 
            { 
                FirstName = "Bob",
                LastName = "Test",
                EmailAddress = "test@email.com",
                Role = "admin",
                ContactId = Guid.NewGuid().ToString(),
                UniqueReference = "unique"                
            };
            
            var expectedUserId = Guid.NewGuid();

            var jsonString = JsonConvert.SerializeObject(expectedUserId);
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

            _sut = new UserService(_mockLogger.Object, httpClient);

            var actualResult = await _sut.AddUserAsync(user);

            Assert.Equal(expectedUserId, actualResult);
        }

        [Fact]
        public async Task AddAddress_Return_Success()
        {
            var travelDocumentViewModel = new TravelDocumentViewModel
            {
                  PetKeeperAddressManual = new PetKeeperAddressManualViewModel
                  {
                       AddressLineOne = "test lane"
                  }
            };

            var expectedUserId = Guid.NewGuid();

            var jsonString = JsonConvert.SerializeObject(expectedUserId);
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

            _sut = new UserService(_mockLogger.Object, httpClient);

            var actualResult = await _sut.AddAddressAsync(Domain.Enums.AddressType.Owner, travelDocumentViewModel);

            Assert.Equal(expectedUserId, actualResult);
        }

        [Fact]
        public async Task AddAddress_Return_Failure()
        {
            var travelDocumentViewModel = new TravelDocumentViewModel
            {
                PetKeeperAddressManual = new PetKeeperAddressManualViewModel
                {
                    AddressLineOne = "test lane"
                }
            };

            var expectedUserId = Guid.NewGuid();

            var jsonString = JsonConvert.SerializeObject(expectedUserId);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var expectedResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = httpContent
            };

            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(expectedResponse);

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://localhost/")
            };

            _sut = new UserService(_mockLogger.Object, httpClient);

            await Assert.ThrowsAsync<HttpRequestException>(async () => await _sut.AddAddressAsync(Domain.Enums.AddressType.Owner, travelDocumentViewModel));
        }

        [Fact]
        public async Task AddUser_BadRequest_ThrowsException()
        {
            var user = new User
            {
                UserId = Guid.NewGuid().ToString(),
                FirstName = "Bob",
                LastName = "Test",
                EmailAddress = "test@email.com",
                Role = "admin",
                ContactId = Guid.NewGuid().ToString(),
                UniqueReference = "unique"
            };

            var expectedUserId = Guid.NewGuid();

            var jsonString = JsonConvert.SerializeObject(expectedUserId);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var expectedResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = httpContent
            };

            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(expectedResponse);

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://localhost/")
            };

            _sut = new UserService(_mockLogger.Object, httpClient);

            await Assert.ThrowsAsync<HttpRequestException>(async () => await _sut.AddUserAsync(user));
        }

        [Fact]
        public async Task AddUser_ThrowsException()
        {
            var user = new User
            {
                UserId = Guid.NewGuid().ToString(),
                FirstName = "Bob",
                LastName = "Test",
                EmailAddress = "test@email.com",
                Role = "admin",
                ContactId = Guid.NewGuid().ToString(),
                UniqueReference = "unique"
            };

            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Error"));

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://localhost/")
            };

            _sut = new UserService(_mockLogger.Object, httpClient);

            await Assert.ThrowsAsync<HttpRequestException>(async () => await _sut.AddUserAsync(user));
        }

        [Fact]
        public async Task AddOwner_Return_Success()
        {
            var user = new User
            {
                FirstName = "Bob",
                LastName = "Test",
                EmailAddress = "test@email.com",
                Role = "admin",
                ContactId = Guid.NewGuid().ToString(),
                UniqueReference = "unique"
            };

            var travelDocument = new TravelDocumentViewModel
            {
                PetKeeperUserDetails = new PetKeeperUserDetailsViewModel
                {
                    AddressLineOne = "Line 1",
                    Postcode = "sw1 4tg",
                    TownOrCity = "London",
                    Phone = "119344234",
                    Email = "test@test.com",
                    Name = "Test", 
                    IsCompleted = true,
                    UserDetailsAreCorrect = Domain.Enums.YesNoOptions.Yes
                }
            };  

            travelDocument.PetKeeperUserDetails.TrimUnwantedData();

            var expectedOwnerId = Guid.NewGuid();

            var jsonString = JsonConvert.SerializeObject(expectedOwnerId);
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

            _sut = new UserService(_mockLogger.Object, httpClient);

            var actualResult = await _sut.AddOwnerAsync(user, travelDocument);

            Assert.Equal(expectedOwnerId, actualResult);
        }

        [Fact]
        public async Task AddOwner_BadRequest_ThrowsException()
        {
            var user = new User
            {
                FirstName = "Bob",
                LastName = "Test",
                EmailAddress = "test@email.com",
                Role = "admin",
                ContactId = Guid.NewGuid().ToString(),
                UniqueReference = "unique"
            };

            var travelDocument = new TravelDocumentViewModel
            {
                PetKeeperUserDetails = new PetKeeperUserDetailsViewModel
                {
                    AddressLineOne = "Line 1",
                    Postcode = "sw1 4tg",
                    TownOrCity = "London",
                    Phone = "119344234",
                    Email = "test@test.com",
                    Name = "Test",
                    IsCompleted = true,
                    UserDetailsAreCorrect = Domain.Enums.YesNoOptions.Yes
                }
            };

            travelDocument.PetKeeperUserDetails.TrimUnwantedData();

            var expectedOwnerId = Guid.NewGuid();

            var jsonString = JsonConvert.SerializeObject(expectedOwnerId);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var expectedResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = httpContent
            };

            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(expectedResponse);

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://localhost/")
            };

            _sut = new UserService(_mockLogger.Object, httpClient);

            await Assert.ThrowsAsync<HttpRequestException>(async () => await _sut.AddOwnerAsync(user, travelDocument));
        }

        [Fact]
        public async Task AddOwner_ThrowsException()
        {
            var user = new User
            {
                FirstName = "Bob",
                LastName = "Test",
                EmailAddress = "test@email.com",
                Role = "admin",
                ContactId = Guid.NewGuid().ToString(),
                UniqueReference = "unique"
            };

            var travelDocument = new TravelDocumentViewModel
            {
                IsApplicationInProgress = true,
                PetKeeperUserDetails = new PetKeeperUserDetailsViewModel
                {
                    AddressLineOne = "Line 1",
                    Postcode = "sw1 4tg",
                    TownOrCity = "London",
                    Phone = "119344234", 
                }, 
                IsSubmitted = true
            };

            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Error"));

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://localhost/")
            };

            _sut = new UserService(_mockLogger.Object, httpClient);

            await Assert.ThrowsAsync<HttpRequestException>(async () => await _sut.AddOwnerAsync(user, travelDocument));
        }

        [Fact]
        public async Task UpdateUser_Return_Success()
        {
            var expectedOwnerId = Guid.NewGuid();

            var jsonString = JsonConvert.SerializeObject(expectedOwnerId);
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

            _sut = new UserService(_mockLogger.Object, httpClient);

            var actualResult = await _sut.UpdateUserAsync("test@email.com");

            Assert.Equal(expectedOwnerId, actualResult);
        }

        [Fact]
        public async Task UpdateUser_BadRequest_ThrowsException()
        {
            var expectedOwnerId = Guid.NewGuid();

            var jsonString = JsonConvert.SerializeObject(expectedOwnerId);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var expectedResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = httpContent
            };

            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(expectedResponse);

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://localhost/")
            };

            _sut = new UserService(_mockLogger.Object, httpClient);

            await Assert.ThrowsAsync<HttpRequestException>(async () => await _sut.UpdateUserAsync("test@email.com"));
        }

        [Fact]
        public async Task UpdateUser_ThrowsException()
        {
            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Error"));

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://localhost/")
            };

            _sut = new UserService(_mockLogger.Object, httpClient);

            await Assert.ThrowsAsync<HttpRequestException>(async () => await _sut.UpdateUserAsync("test@email.com"));
        }

        [Fact]
        public async Task UpdateAddress_Return_Success()
        {
            var expectedOwnerId = Guid.NewGuid();

            var jsonString = JsonConvert.SerializeObject(expectedOwnerId);
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

            _sut = new UserService(_mockLogger.Object, httpClient);

            var actualResult = await _sut.UpdateUserAddressAsync("test@email.com", expectedOwnerId);

            Assert.Equal(expectedOwnerId, actualResult);
        }

        [Fact]
        public async Task UpdateAddress_BadRequest_ThrowsException()
        {
            var expectedOwnerId = Guid.NewGuid();

            var jsonString = JsonConvert.SerializeObject(expectedOwnerId);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var expectedResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = httpContent
            };

            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(expectedResponse);

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://localhost/")
            };

            _sut = new UserService(_mockLogger.Object, httpClient);

            await Assert.ThrowsAsync<HttpRequestException>(async () => await _sut.UpdateUserAddressAsync("test@email.com", expectedOwnerId));
        }

        [Fact]
        public async Task UpdateAddress_ThrowsException()
        {
            var expectedOwnerId = Guid.NewGuid();

            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Error"));

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://localhost/")
            };

            _sut = new UserService(_mockLogger.Object, httpClient);

            await Assert.ThrowsAsync<HttpRequestException>(async () => await _sut.UpdateUserAddressAsync("test@email.com", expectedOwnerId));
        }

        [Fact]
        public async Task GetUserDetail_Return_Success()
        {
            var expectedUserId = Guid.NewGuid();

            var userDetail = new UserDetailDto
            {
                FullName = "Test"
            };


            var jsonString = JsonConvert.SerializeObject(userDetail);
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

            _sut = new UserService(_mockLogger.Object, httpClient);

            var actualResult = await _sut.GetUserDetail(expectedUserId);

            Assert.Equal(userDetail.FullName, actualResult.FullName);
        }

        [Fact]
        public async Task GetUserDetail_Throws_Exception()
        {
            var expectedUserId = Guid.NewGuid();

            var userDetail = new UserDetailDto
            {
                FullName = "Test"
            };


            var jsonString = JsonConvert.SerializeObject(userDetail);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var expectedResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadGateway,
                Content = httpContent
            };

            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Error"));

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://localhost/")
            };

            _sut = new UserService(_mockLogger.Object, httpClient);

            await Assert.ThrowsAsync<HttpRequestException>(async () => await _sut.GetUserDetail(expectedUserId));
        }

    }
}
