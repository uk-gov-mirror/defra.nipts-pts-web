using Defra.PTS.Web.Application.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;

namespace Defra.PTS.Web.UI.UnitTests.Extensions;

public class ModelStateExtensionsTests
{
    [Fact]
    public void ModelState_HasError_ReturnsTrueWhenErrorExists()
    {
        // Arrange
        var modelState = new Mock<ModelStateDictionary>();

        modelState.Object.AddModelError("Property", "An error has occurred");

        // Act
        var result = modelState.Object.HasError("Property");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ModelState_HasError_ReturnsFalseWhenHasNoError()
    {
        // Arrange
        var modelState = new Mock<ModelStateDictionary>();

        // Act
        var result = modelState.Object.HasError("Property");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ModelState_GetErrors_WhenNoError()
    {
        // Arrange
        var modelState = new Mock<ModelStateDictionary>();

        // Act
        var result = modelState.Object.GetErrors("Property");

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void ModelState_GetErrors_WhenHasErrors()
    {
        // Arrange
        var modelState = new Mock<ModelStateDictionary>();
        modelState.Object.AddModelError("Property", "Error 1: An error has occurred");
        modelState.Object.AddModelError("Property", "Error 2: An error has occurred");

        // Act
        var result = modelState.Object.GetErrors("Property");

        // Assert
        result.Should().Be("Error 1: An error has occurred,Error 2: An error has occurred");
    }
}
