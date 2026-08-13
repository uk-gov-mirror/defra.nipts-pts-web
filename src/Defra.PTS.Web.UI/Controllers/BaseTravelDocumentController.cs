using Defra.PTS.Web.Application.Extensions;
using Defra.PTS.Web.CertificateGenerator.Models;
using Defra.PTS.Web.Domain.Enums;
using Defra.PTS.Web.Domain.ViewModels.TravelDocument;
using Defra.PTS.Web.UI.Configuration;
using Defra.PTS.Web.UI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NUglify.JavaScript.Syntax;
using PdfSharp.Pdf;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Web.UI.Controllers;

[Authorize]
public class BaseTravelDocumentController : BaseController
{
    #region Application

    public void EnsureApplicationPagePreConditions(TravelDocumentFormPageType pageType)
    {
        if (!IsApplicationInProgress())
        {
            RedirectToAction(nameof(Index)).ExecuteResult(this.ControllerContext);
        }

        var formData = GetFormData();
        if (!formData.DoesPageMeetPreConditions(pageType, out string actionName))
        {
            RedirectToAction(actionName).ExecuteResult(this.ControllerContext);
        }
    }

    public virtual bool IsApplicationInProgress()
    {
        var formData = GetFormData();

        var magicWordData = GetMagicWordFormData();

        if (magicWordData != null && !magicWordData.HasUserPassedPasswordCheck)
            return false;

        if (formData != null && formData.IsApplicationInProgress)
            return true;
        else
            return false;
    }

    protected void SetApplicationIsSubmitted(bool isSubmitted = true)
    {
        var formData = GetFormData();

        formData.IsSubmitted = isSubmitted;

        SaveFormData(formData);
    }

    protected void SetApplicationInProgress(bool isApplicationInProgress = true)
    {
        var formData = GetFormData();

        formData.IsApplicationInProgress = isApplicationInProgress;

        SaveFormData(formData);
    }


    #endregion Application

    #region SaveFormData

    protected void SaveFormData(AcknowledgementViewModel model)
    {
        var formData = GetFormData();
        
        formData.Acknowledgement = model;        

        SaveFormData(formData);
        SaveApplicationReference(model.Reference);
    }

    protected void SaveFormData(DeclarationViewModel model)
    {
        var formData = GetFormData();

        formData.Declaration = model;

        SaveFormData(formData);
    }

    protected void SaveFormData(PetAgeViewModel model)
    {
        var formData = GetFormData();

        formData.PetAge = model;

        SaveFormData(formData);
    }

    protected void SaveFormData(PetBreedViewModel model)
    {
        var formData = GetFormData();

        formData.PetBreed = model;

        SaveFormData(formData);
    }

    public virtual void SaveFormData(PetColourViewModel model)
    {
        var formData = GetFormData();

        formData.PetColour = model;

        SaveFormData(formData);
    }

    protected void SaveFormData(PetFeatureViewModel model)
    {
        var formData = GetFormData();

        formData.PetFeature = model;

        SaveFormData(formData);
    }

    protected void SaveFormData(PetGenderViewModel model)
    {
        var formData = GetFormData();

        formData.PetGender = model;

        SaveFormData(formData);
    }

    protected void SaveFormData(PetKeeperAddressManualViewModel model)
    {
        var formData = GetFormData();

        formData.PetKeeperAddressManual = model;

        SaveFormData(formData);
    }

    protected void SaveFormData(PetKeeperAddressViewModel model)
    {
        var formData = GetFormData();

        formData.PetKeeperAddress = model;

        SaveFormData(formData);
    }

    protected void SaveFormData(PetKeeperNameViewModel model)
    {
        var formData = GetFormData();

        formData.PetKeeperName = model;

        SaveFormData(formData);
    }

    protected void SaveFormData(PetKeeperPhoneViewModel model)
    {
        var formData = GetFormData();

        formData.PetKeeperPhone = model;

        SaveFormData(formData);
    }

    protected void SaveFormData(PetKeeperPostcodeViewModel model)
    {
        var formData = GetFormData();

        formData.PetKeeperPostcode = model;

        SaveFormData(formData);
    }

    protected void SaveFormData(PetKeeperUserDetailsViewModel model)
    {
        var formData = GetFormData();

        formData.PetKeeperUserDetails = model;

        SaveFormData(formData);
    }

    protected void SaveFormData(PetMicrochipDateViewModel model)
    {
        var formData = GetFormData();

        formData.PetMicrochipDate = model;

        SaveFormData(formData);
    }

    protected void SaveFormData(PetMicrochipViewModel model)
    {
        var formData = GetFormData();

        formData.PetMicrochip = model;

        SaveFormData(formData);
    }

    protected void SaveFormData(PetNameViewModel model)
    {
        var formData = GetFormData();

        formData.PetName = model;

        SaveFormData(formData);
    }

    protected void SaveFormData(PetSpeciesViewModel model)
    {
        var formData = GetFormData();

        formData.PetSpecies = model;

        SaveFormData(formData);
    }

    protected void SaveFormData(TravelDocumentViewModel model)
    {
        TempData.SetTravelDocument(model);
    }
    #endregion SaveFormData

    #region TravelDocument
    public virtual TravelDocumentViewModel GetFormData(bool createIfNull = false)
    {
        return TempData.GetTravelDocument(createIfNull);
    }

    protected void RemoveFormData()
    {
        TempData.RemoveTravelDocument();
    }
    #endregion TravelDocument

    #region FormSubmissionQueue
    public void AddToFormSubmissionQueue(Guid id)
    {
        TempData.AddToFormSubmissionQueue(id);
    }
    public void RemoveFromFormSubmissionQueue(Guid id)
    {
        TempData.RemoveFromFormSubmissionQueue(id);
    }
    
    public bool IsInFormSubmissionQueue(Guid id)
    {
        return TempData.IsInFormSubmissionQueue(id);
    }

    [ExcludeFromCodeCoverage]
    public override HttpContext GetHttpContext()
    {
        throw new NotImplementedException();
    }
    #endregion FormSubmissionQueue

    #region ApplicationReference
    protected void SaveApplicationReference(string applicationReference)
    {
        TempData.SetApplicationReference(applicationReference);
    }
    public string GetApplicationReference()
    {
        return TempData.GetApplicationReference();
    }

    public virtual List<Guid> GetFormSubmissionQueue()
    {
        return TempData.GetFormSubmissionQueue();
    }

    #endregion ApplicationReference
    public void SetCYACheck(string value)
    {
        TempData.Set("CYA", value);
    }

    public virtual bool GetCYACheck()
    {
        var CYA = TempData.Peek("CYA");
        if (CYA != null && CYA.ToString().Contains("Yes"))
        {
            return true;
        }
        return false;
    }

    public IActionResult CYARedirect(string actionResult)
    {
        if (GetCYACheck())
        {
            return RedirectToAction(nameof(Declaration));
        }
        return RedirectToAction(actionResult);
    }

    public async virtual Task<IActionResult> SetFileTitle(CertificateResult response, string fileName, string fileTitle)
    {
        // Ensure PDFsharp can resolve fonts before manipulating/saving the PDF.
        PdfFontConfiguration.Configure();

        // Convert response.Content (Stream) to a MemoryStream
        using (var inputStream = new MemoryStream())
        {
            await response.Content.CopyToAsync(inputStream);
            inputStream.Position = 0;  // Reset position to the beginning

            // Use MemoryStream with PdfReader.Open
            using (var pdfDocument = PdfSharp.Pdf.IO.PdfReader.Open(inputStream, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Modify))
            {
                // Set metadata and viewer preferences
                pdfDocument.Info.Title = fileTitle;                

                //Set language - may need a toggle for Welsh?
                pdfDocument.Language = "en-GB";

                pdfDocument.Language = "en-GB";

                // Manually add ViewerPreferences dictionary
                PdfDictionary viewerPreferences = new PdfDictionary();
                viewerPreferences.Elements["/DisplayDocTitle"] = new PdfBoolean(true);
                viewerPreferences.Elements["/UseDocumentStructure"] = new PdfBoolean(true);

                // Access the catalog dictionary and set ViewerPreferences
                PdfDictionary catalog = pdfDocument.Internals.Catalog;
                catalog.Elements["/ViewerPreferences"] = viewerPreferences;

                // Set "Use Document Structure" for tab order in each page
                foreach (var page in pdfDocument.Pages)
                {
                    if (page.Elements.ContainsKey("/Tabs"))
                    {
                        page.Elements["/Tabs"] = new PdfName("/S");  // Set tab order to "Structure"
                    }
                    else
                    {
                        page.Elements.Add("/Tabs", new PdfName("/S"));  // Add the Tabs key if it doesn't exist
                    }
                }

                using (var outputStream = new MemoryStream())
                {
                    pdfDocument.Save(outputStream);
                    outputStream.Position = 0;
                    return File(outputStream.ToArray(), response.MimeType, fileName);
                }
            }
        }
    }

    public IActionResult HandleException(Exception ex)
    {
        if (ex is HttpRequestException httpRequestException && httpRequestException.StatusCode.HasValue)
        {
            // Log the error if necessary
            return RedirectToAction("HandleError", "Error", new { code = (int)httpRequestException.StatusCode.Value });
        }

        // Log the error if necessary
        return RedirectToAction("HandleError", "Error", new { code = 500 });
    }
}
