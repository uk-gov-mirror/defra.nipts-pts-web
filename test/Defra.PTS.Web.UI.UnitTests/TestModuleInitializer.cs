using System.Runtime.CompilerServices;
using Defra.PTS.Web.UI.Configuration;

namespace Defra.PTS.Web.UI.UnitTests;

internal static class TestModuleInitializer
{
    /// <summary>
    /// Ensures PDFsharp font resolution is configured once before any test runs.
    /// Tests that build sample PDFs with <c>XFont</c> require this, matching the
    /// application's own runtime configuration.
    /// </summary>
    [ModuleInitializer]
    public static void Initialize()
    {
        PdfFontConfiguration.Configure();
    }
}
