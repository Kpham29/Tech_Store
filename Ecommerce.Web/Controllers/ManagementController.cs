using System.Net.Http.Headers;
using Ecommerce.Web.Helper;
using Ecommerce.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Controllers;

public class ManagementController : Controller
{
    private readonly IApiHelperService _api;
    
    public ManagementController(IApiHelperService api)
    {
        _api = api;
    }

    private bool IsAuthorized()
    {
        var role = HttpContext.Request.Cookies["role"];
        return role == "Admin" || role == "Staff";
    }

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        if (!IsAuthorized()) return RedirectToAction("Login", "Auth");

        var profile = await _api.GetAsync<CustomerProfileViewModel>("api/customer/profile");
        if (profile == null)
        {
            ViewBag.Error = "Session expired or data could not be loaded.";
            return RedirectToAction("Login", "Auth");
        }
        return View(profile);
    }

    [HttpPost]
    public async Task<IActionResult> Profile(CustomerProfileViewModel profile, IFormFile? avatarFile)
    {
        if (!IsAuthorized()) return RedirectToAction("Login", "Auth");

        try
        {
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(profile.FullName ?? ""), "FullName");
            content.Add(new StringContent(profile.Phone ?? ""), "Phone");
            content.Add(new StringContent(profile.Address ?? ""), "Address");

            if (avatarFile != null)
            {
                var streamContent = new StreamContent(avatarFile.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(avatarFile.ContentType);
                content.Add(streamContent, "file", avatarFile.FileName);
            }

            var result = await _api.PutMultipartAsync<object>("api/customer/profile", content);
            ViewBag.Success = "Update profile successfully";
            var updatedProfile = await _api.GetAsync<CustomerProfileViewModel>("api/customer/profile");
            
            // Update avatar in cookie if it was changed
            if (updatedProfile != null && !string.IsNullOrEmpty(updatedProfile.Avatar))
            {
                Response.Cookies.Append("avatar", updatedProfile.Avatar);
            }

            return View(updatedProfile);
        }
        catch (Exception e)
        {
            ViewBag.Error = e.Message;
            return View(profile);
        }
    }
}
