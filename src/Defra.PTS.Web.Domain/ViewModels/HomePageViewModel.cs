using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Web.Domain.ViewModels;

[ExcludeFromCodeCoverage]
public class HomePageViewModel
{
    public string EnteredPassword { get; set; } = default;
    public bool MagicWordEnabled { get; set; } = default;
}
