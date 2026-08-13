using Defra.PTS.Web.Domain.Enums;
using Defra.PTS.Web.Domain.Models;
using Defra.PTS.Web.Domain.ViewModels.TravelDocument;
using Defra.PTS.Web.UI.Constants;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace Defra.PTS.Web.UI.Controllers;

[ExcludeFromCodeCoverage]
public class ContentController : BaseController
{
    private readonly IOptions<GoogleTagManager> _googleTagManager;
    private readonly string cookiePolicy = "cookie_policy";
    private readonly string cookiesSeenMessage = "seen_cookie_message";
    private readonly string cookiesReject = "reject";

    public ContentController(IOptions<GoogleTagManager> googleTagManager) : base()
    {
        ArgumentNullException.ThrowIfNull(googleTagManager);
        _googleTagManager = googleTagManager;
    }

    public IActionResult Help() => RenderHistoryContent();


    public IActionResult AccessibilityStatement() => RenderHistoryContent();


    private ViewResult RenderHistoryContent()
    {
        // shared logic
        SetBackUrl(WebAppConstants.HistoryBack);

        return View();
    }


    [HttpGet]
    public IActionResult Cookies(bool saved = false)
    {
        //Cookies Page
        SetBackUrl(WebAppConstants.HistoryBack);

        //Retrieve GA cookie values from request
        var model = new CookiesModel()
        {
            GaCookieAcceptYesNo = Request.Cookies[cookiePolicy],
            MeasurementId = "_ga_" + _googleTagManager.Value.MeasurementId,
            SeenCookieMessage = Request.Cookies[cookiesSeenMessage] == "yes",
        };


        if (!model.SeenCookieMessage)
            model.GaCookieAcceptYesNo = cookiesReject;

        model.Saved = saved;

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SetCookie(CookiesModel model)
    {
        AddCookies(model);

        return RedirectToAction(nameof(Cookies), new {saved = true});
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SetCookieViaBanner(CookiesModel model)
    {
        AddCookies(model);
        // Return the same view without redirecting
        return NoContent();
    }

    private void AddCookies(CookiesModel model)
    {
        CookieOptions cookieOptions = new CookieOptions()
        {
            Expires = DateTime.UtcNow.AddMonths(12),
            IsEssential = true,
            Secure = true
        };
        Response.Cookies.Delete(cookiesSeenMessage);
        Response.Cookies.Delete(cookiePolicy);
        Response.Cookies.Append(cookiesSeenMessage, "yes", cookieOptions);

        if (model.GaCookieAcceptYesNo == cookiesReject)
        {
            Response.Cookies.Append(cookiePolicy, cookiesReject, cookieOptions);
            Response.Cookies.Delete(model.MeasurementId, new CookieOptions { Path = "/", Domain = _googleTagManager.Value.Domain, Secure = true });
            Response.Cookies.Delete("_ga", new CookieOptions { Path = "/", Domain = _googleTagManager.Value.Domain, Secure = true });
        }
        else
        {
            Response.Cookies.Append(cookiePolicy, "accept", cookieOptions);
        }
    }

    public IActionResult TermsAndConditions()
    {
        //T and Cs
        SetBackUrl(WebAppConstants.HistoryBack);

        return View();
    }

    [ExcludeFromCodeCoverage]
    public override HttpContext GetHttpContext()
    {
        throw new NotImplementedException();
    }
}