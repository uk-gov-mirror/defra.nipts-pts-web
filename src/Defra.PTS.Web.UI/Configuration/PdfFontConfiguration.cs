using PdfSharp.Fonts;

namespace Defra.PTS.Web.UI.Configuration;

/// <summary>
/// Configures PDFsharp font resolution.
/// PDFsharp 6.2 removed the automatic platform font resolution that earlier
/// versions provided, so families such as "Arial" that are referenced by
/// generated PDFs can no longer be resolved out of the box. This causes
/// <c>PdfDocument.Save</c> to throw
/// "No appropriate font found for family name 'Arial'".
/// Re-enabling the Windows font resolver restores the previous behaviour for
/// the application (deployed on Windows/IIS) and for the test runner.
/// </summary>
public static class PdfFontConfiguration
{
    private static readonly object SyncRoot = new();
    private static bool _configured;

    /// <summary>
    /// Ensures PDFsharp can resolve fonts. Safe to call multiple times; the
    /// configuration is only applied once.
    /// </summary>
    public static void Configure()
    {
        if (_configured)
        {
            return;
        }

        lock (SyncRoot)
        {
            if (_configured)
            {
                return;
            }

            GlobalFontSettings.UseWindowsFontsUnderWindows = true;
            GlobalFontSettings.UseWindowsFontsUnderWsl2 = true;

            _configured = true;
        }
    }
}
