using Defra.PTS.Web.Application.Constants;
using Defra.PTS.Web.Application.DTOs.Services;
using Defra.PTS.Web.Application.Extensions;
using Defra.PTS.Web.Domain.Models;
using Defra.PTS.Web.UI.Constants;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace Defra.PTS.Web.UI.UnitTests.Extensions;

public class MiscExtensionsTests
{
    public MiscExtensionsTests()
    {
    }

    [Theory]
    [InlineData(AppConstants.ApplicationStatus.APPROVED, "govuk-tag--green")]
    [InlineData(AppConstants.ApplicationStatus.UNSUCCESSFUL, "govuk-tag--orange")]
    [InlineData(AppConstants.ApplicationStatus.AWAITINGVERIFICATION, "govuk-tag--blue")]
    [InlineData(AppConstants.ApplicationStatus.REVOKED, "govuk-tag--red")]
    [InlineData(AppConstants.ApplicationStatus.SUSPENDED, "govuk-tag--yellow")]
    [InlineData("Other", "govuk-tag--red")]
    public void StatusBasedCssClassReturnsCorrectClass(string status, string cssClass)
    {
        // Arrange
        var model = new ApplicationSummaryDto
        {
            Status = status
        };

        // Act
        var result = model.StatusBasedCssClass();

        // Assert
        result.Should().Be(cssClass);
    }

    [Theory]
    [InlineData(AppConstants.ApplicationStatus.APPROVED, WebAppConstants.Pages.TravelDocument.ApplicationCertificate)]
    [InlineData(AppConstants.ApplicationStatus.UNSUCCESSFUL, WebAppConstants.Pages.TravelDocument.ApplicationDetails)]
    [InlineData(AppConstants.ApplicationStatus.AWAITINGVERIFICATION, WebAppConstants.Pages.TravelDocument.ApplicationDetails)]
    [InlineData(AppConstants.ApplicationStatus.REVOKED, WebAppConstants.Pages.TravelDocument.ApplicationCertificate)]
    [InlineData(AppConstants.ApplicationStatus.SUSPENDED, WebAppConstants.Pages.TravelDocument.ApplicationCertificate)]
    [InlineData("Other", WebAppConstants.Pages.TravelDocument.ApplicationDetails)]
    public void StatusBasedDetailsUrlReturnsCorrectUrl(string status, string expectedUrl)
    {
        // Arrange
        var id = Guid.NewGuid();
        var model = new ApplicationSummaryDto
        {
            Status = status,
            ApplicationId = id,
        };

        expectedUrl += $"/{id}";

        // Act
        var result = model.StatusBasedDetailsUrl();

        // Assert
        result.Should().Be(expectedUrl);
    }

    [Fact]
    public void AddressList_ConvertTo_SelectListItems()
    {
        // Arrange
        var model = new List<Address>
        {
             new()
             {
                  AddressLineOne = "Test Street 1",
                  AddressLineTwo = "Test Locality 1",
                  TownOrCity = "Test City 1",
                  County = "Test County 1",
                  Postcode = "Test Postcode 1",
             },
             new()
             {
                  AddressLineOne = "Test Street 2",
                  AddressLineTwo = "Test Locality 2",
                  TownOrCity = "Test City 2",
                  County = "Test County 2",
                  Postcode = "Test Postcode 2",
             }
        };

        // Act
        var result = model.ToSelectListItems(hasSelectRow: true);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<List<SelectListItem>>();
            result.Count.Should().Be(model.Count + 1);
            result[0].Text.Should().Be($"{model.Count} addresses found");
            result[1].Text.Should().Be(model[0].ToDisplayString());
            result[1].Value.Should().Be(model[0].ToCsvString());
        }
    }
    [Fact]
    public void AddressList_ConvertTo_SelectListItems_Welsh()
    {
        // Arrange        
        Thread.CurrentThread.CurrentCulture = new CultureInfo("cy"); 
        var model = new List<Address>
        {
             new()
             {
                  AddressLineOne = "Test Street 1",
                  AddressLineTwo = "Test Locality 1",
                  TownOrCity = "Test City 1",
                  County = "Test County 1",
                  Postcode = "Test Postcode 1",
             },
             new()
             {
                  AddressLineOne = "Test Street 2",
                  AddressLineTwo = "Test Locality 2",
                  TownOrCity = "Test City 2",
                  County = "Test County 2",
                  Postcode = "Test Postcode 2",
             }
        };

        // Act
        var result = model.ToSelectListItems(hasSelectRow: true);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<List<SelectListItem>>();
            result.Count.Should().Be(model.Count + 1);
            result[0].Text.Should().Be($"{model.Count} o gyfeiriadau wedi'u canfod");
            result[1].Text.Should().Be(model[0].ToDisplayString());
            result[1].Value.Should().Be(model[0].ToCsvString());
        }
    }

    [Fact]
    public void BreedList_ConvertTo_SelectListItems()
    {
        // Arrange
        var model = new List<BreedDto>
        {
             new()
             {
                  BreedId = 1,
                  BreedName = "Test 1",
             },
             new()
             {
                  BreedId = 2,
                  BreedName = "Test 2",
             }
        };

        // Act
        var result = model.ToSelectListItems(hasSelectRow: true);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<List<SelectListItem>>();
            result.Count.Should().Be(model.Count + 1);
            result[0].Text.Should().Be(string.Empty);
            result[1].Text.Should().Be(model[0].BreedName);
            result[1].Value.Should().Be(model[0].BreedId.ToString());
        }
    }
}

