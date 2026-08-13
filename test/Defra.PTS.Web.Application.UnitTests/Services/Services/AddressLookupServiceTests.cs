using Defra.PTS.Web.Infrastructure.Services.Interfaces;
using Moq;
using Address = Defra.PTS.Web.Domain.Models.Address;

namespace Defra.PTS.Web.Application.UnitTests.Services.Services
{
    public class AddressLookupServiceTests
    {
        protected Mock<HttpMessageHandler> _mockHttpMessageHandler = new();

        [Fact]
        public void ConvertAddress_Return_Success()
        {
            var address = new Address
            {
                AddressLineOne = "10 Downing Street",
                AddressLineTwo = "London",
                County = "Greater London",
                Postcode = "SW1A 2AA",
                TownOrCity = "London"
            };

            var csv = "10 Downing Street;London;London;Greater London;SW1A 2AA";
            var addressFromCsv = new Address(csv);
            var csvFromAdddress = address.ToCsvString();

            Assert.Equal(address.AddressLineOne, addressFromCsv.AddressLineOne);
            Assert.Equal(address.AddressLineTwo, addressFromCsv.AddressLineTwo);
            Assert.Equal(address.County, addressFromCsv.County);
            Assert.Equal(address.Postcode, addressFromCsv.Postcode);
            Assert.Equal(address.TownOrCity, addressFromCsv.TownOrCity);
            Assert.Equal(address.ToDisplayString(), addressFromCsv.ToDisplayString());
            Assert.Equal(csvFromAdddress, csv);
        }

    }
}
