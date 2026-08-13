using Defra.PTS.Web.Application.Constants;
using Defra.PTS.Web.Application.DTOs.Services;
using Defra.PTS.Web.Application.Features.DynamicsCrm.Commands;
using Defra.PTS.Web.Application.Features.TravelDocument.Queries;
using Defra.PTS.Web.Application.Features.Users.Commands;
using Defra.PTS.Web.Application.Features.Users.Queries;

using Defra.PTS.Web.Application.Services.Interfaces;
using Defra.PTS.Web.Domain.Models;
using Defra.PTS.Web.Domain.ViewModels;
using Defra.PTS.Web.UI.Constants;
using Defra.PTS.Web.UI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Microsoft.Extensions.Localization;
using Defra.PTS.Web.UI.Helpers;
using Defra.PTS.Web.Domain.Enums;

namespace Defra.PTS.Web.UI.Controllers;

[Authorize]
public partial class TravelDocumentController : BaseTravelDocumentController
{

    private readonly IMediator _mediator;
    private readonly ILogger<TravelDocumentController> _logger;
    private readonly PtsSettings _ptsSettings;
    private readonly ISelectListLocaliser _selectListLocaliser;
    private readonly IStringLocalizer<ISharedResource> _localizer;

    public TravelDocumentController(
          IValidationService validationService,
          IMediator mediator,
          ILogger<TravelDocumentController> logger,
          IOptions<PtsSettings> ptsSettings,
          ISelectListLocaliser breedHelper,
          IStringLocalizer<ISharedResource> localizer
          )
    {
        ArgumentNullException.ThrowIfNull(validationService);
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(ptsSettings);

        _mediator = mediator;
        _logger = logger;
        _selectListLocaliser = breedHelper;
        _localizer = localizer;
        _ptsSettings = ptsSettings.Value;
   
    }

    [ExcludeFromCodeCoverage]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            if (HttpContext != null && HttpContext.Session != null)
            {
                HttpContext.Session.SetString("SessionActive", "yes");
            }

            if (HttpContext.Session.TryGetValue("ManagementLinkClicked", out byte[] managementLinkClicked) && System.Text.Encoding.UTF8.GetString(managementLinkClicked) == "true")
            {
                return RedirectToAction("CheckIdm2SignOut", "User");
            }

            if (HttpContext.Request.Cookies.TryGetValue(".AspNetCore.Culture", out string language) && language == "c=cy|uic=cy")
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("cy");
            }

            var magicWordData = GetMagicWordFormData(true);

            if (_ptsSettings.MagicWordEnabled && magicWordData != null && !magicWordData.HasUserPassedPasswordCheck)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                SaveMagicWordFormData(new MagicWordViewModel { HasUserPassedPasswordCheck = true });

                SetBackUrl(string.Empty);

                await AddOrUpdateUser();
                await InitializeUserDetails();

                var validStatuses = new List<string>
                {
                    AppConstants.ApplicationStatus.APPROVED,
                    AppConstants.ApplicationStatus.AWAITINGVERIFICATION,
                    AppConstants.ApplicationStatus.SUSPENDED
                };

                var invalidStatuses = new List<string>
                {
                    AppConstants.ApplicationStatus.REVOKED,
                    AppConstants.ApplicationStatus.UNSUCCESSFUL
                };

                var userId = CurrentUserId();
                var response = await _mediator.Send(new GetApplicationsQueryRequest(userId, validStatuses));
                var invalidResponse = await _mediator.Send(new GetApplicationsQueryRequest(userId, invalidStatuses));

                SaveIsUserSuspendedFormData(response.Applications.Any(x => x.Status == "Suspended"));

                // Pass a flag to the view indicating whether invalid documents exist
                ViewBag.HasInvalidDocuments = invalidResponse.Applications.Count > 0;

                return View(response.Applications);
            }
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [ExcludeFromCodeCoverage]
    [HttpGet]
    public async Task<IActionResult> InvalidDocuments()
    {
   
        try 
        {
            SetBackUrl(WebAppConstants.Pages.TravelDocument.Index);

            await AddOrUpdateUser();
            await InitializeUserDetails();

            var statuses = new List<string>()
            {
                AppConstants.ApplicationStatus.REVOKED,
                AppConstants.ApplicationStatus.UNSUCCESSFUL
            };

            var userId = CurrentUserId();
            var response = await _mediator.Send(new GetApplicationsQueryRequest(userId, statuses));

            var sortedApplications = response.Applications // Sort by DateOfApplication or DocumentIssueDate, whichever is the latter
                .OrderByDescending(x => 
                    new[] { x.DateOfApplication, x.DocumentIssueDate }
                        .Max(d => d ?? DateTime.MinValue)
                )
                .ToList();

            return View(sortedApplications);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ApplicationDetailRecord(string id, string status)
    {
        
        HttpContext.Session.SetString("ApplicationId", id);        

        if (status == AppConstants.ApplicationStatus.APPROVED)
            return RedirectToAction(nameof(ApplicationCertificate));
        else
            return RedirectToAction(nameof(ApplicationDetails));


    }

    #region Private Methods
    [ExcludeFromCodeCoverage]
    private async Task AddOrUpdateUser()
    {
        var userInfo = GetCurrentUserInfo();

        var response = await _mediator.Send(new AddUserRequest(userInfo));
        if (response.IsSuccess)
        {
            var identity = HttpContext.User.Identities.FirstOrDefault();
            identity?.AddClaim(new Claim(WebAppConstants.IdentityKeys.PTSUserId, response.UserId.ToString()));
        }
    }

    [ExcludeFromCodeCoverage]
    private async Task<UserDetailDto> InitializeUserDetails()
    {
        // Save user
        var userInfo = GetCurrentUserInfo();

        try
        {
            await _mediator.Send(new AddAddressRequest(userInfo));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to save address details, {Message}", ex.Message);
        }
        var contactId = CurrentUserContactId();
        var response = await _mediator.Send(new GetUserDetailQueryRequest(contactId));
        if (response != null && response.UserDetail != null)
        {

            HttpContext.Session.SetString("FullName", response.UserDetail?.FullName);
            return response.UserDetail;
        }

        return null;
    }


    public override HttpContext GetHttpContext()
    {
        return HttpContext;
    }

    public Guid CurrentUserContactId()
    {
        if (GetHttpContext().User.Identity.IsAuthenticated)
        {
            var contactId = GetHttpContext().User.GetLoggedInContactId();
            return contactId;
        }

        return Guid.Empty;
    }

    public User GetCurrentUserInfo()
    {
        var identity = GetHttpContext().User.Identities.FirstOrDefault();
        if (identity == null)
        {
            return new User();
        }

        var claims = identity.Claims;

        var user = new User();
        foreach (var claim in claims)
        {
            switch (claim.Type)
            {
                case "contactId":
                    user.ContactId = claim.Value;
                    break;
                case "uniqueReference":
                    user.UniqueReference = claim.Value;
                    break;
                case "firstName":
                    user.FirstName = claim.Value;
                    break;
                case "lastName":
                    user.LastName = claim.Value;
                    break;
                case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress":
                    user.EmailAddress = claim.Value;
                    break;
                case "http://schemas.microsoft.com/ws/2008/06/identity/claims/role":
                    user.Role = claim.Value;
                    break;
                default:
                    break;
            }
        }

        return user;
    }

    #endregion Private Methods
}
