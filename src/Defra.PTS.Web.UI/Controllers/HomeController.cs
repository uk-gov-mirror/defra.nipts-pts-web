using Defra.PTS.Web.Domain.Models;
using Defra.PTS.Web.Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Web.UI.Controllers;

[AllowAnonymous]
public class HomeController : BaseController
{
    private readonly IOptions<PtsSettings> _ptsSettings;
    [BindProperty] public string EnteredPassword { get; set; } = default;

    public HomeController(IOptions<PtsSettings> ptsSettings) : base()
    {
        ArgumentNullException.ThrowIfNull(ptsSettings);

        _ptsSettings = ptsSettings;
    }

    public IActionResult Index()
    {
        if (HttpContext != null && HttpContext.Session != null)
        {
            HttpContext.Session.SetString("SessionActive", "yes");
        }

        var magicWordData = GetMagicWordFormData(true);

        ViewData.Add("HasUserPassedPasswordCheck", magicWordData.HasUserPassedPasswordCheck);
        ViewData.Add("MagicWordEnabled", _ptsSettings.Value.MagicWordEnabled);

        if (_ptsSettings.Value.MagicWordEnabled && !magicWordData.HasUserPassedPasswordCheck)
        {
            var model = new HomePageViewModel { MagicWordEnabled = _ptsSettings.Value.MagicWordEnabled };
            return View(model);
        }

        return RedirectToAction("Index", "TravelDocument");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SubmitMagicWord(HomePageViewModel model)
    {
        if (model.EnteredPassword == _ptsSettings.Value.MagicWord)
        {
            SaveMagicWordFormData(new MagicWordViewModel { HasUserPassedPasswordCheck = true });

            return RedirectToAction("Index", "TravelDocument");
        }

        else
        {
            model.MagicWordEnabled = _ptsSettings.Value.MagicWordEnabled;
            ModelState.AddModelError(nameof(EnteredPassword), "Enter the correct password");
            return View("Index", model);
        }

    }

    [HttpGet]
    public IActionResult ApplicationTimeout()
    {
        return View();
    }

    [ExcludeFromCodeCoverage]
    public override HttpContext GetHttpContext()
    {
        throw new NotImplementedException();
    }
}