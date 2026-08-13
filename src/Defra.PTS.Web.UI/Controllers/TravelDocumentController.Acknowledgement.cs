using Defra.PTS.Web.Domain.ViewModels.TravelDocument;
using Defra.PTS.Web.UI.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Defra.PTS.Web.UI.Controllers;

public partial class TravelDocumentController : BaseTravelDocumentController
{
    [HttpGet]
    public IActionResult Acknowledgement()
    {
        var applicationReference =  GetApplicationReference();

        var formSubmissionQueue = GetFormSubmissionQueue();

        if (string.IsNullOrEmpty(applicationReference) && (formSubmissionQueue == null || formSubmissionQueue.Count == 0))
        {
            return RedirectToAction("Index", "TravelDocument");
        }
        
        if (!IsApplicationInProgress() && string.IsNullOrEmpty(applicationReference))
        {
            return RedirectToAction(nameof(PetKeeperUserDetails));
        }

        var formData = GetFormData();

        if (formData != null && !formData.DoesPageMeetPreConditions(formData.Acknowledgement.PageType, out string actionName))
        {
            return RedirectToAction(actionName);
        }

        SetBackUrl(string.Empty);

        var viewModel = formData != null ? formData.Acknowledgement : new AcknowledgementViewModel { Reference = applicationReference};

        // Clear data
        RemoveFormData();

        return View(viewModel);
    }
}
