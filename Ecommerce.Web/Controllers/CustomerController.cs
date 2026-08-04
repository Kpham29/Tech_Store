using System.Net.Http.Headers;
using Ecommerce.Web.Helper;
using Ecommerce.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Controllers;

public class CustomerController : Controller
{
    private readonly IApiHelperService _api;
    
    public CustomerController(IApiHelperService api)
    {
        _api = api;
    }
    

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        if (string.IsNullOrEmpty(Request.Cookies["jwtToken"]))
        {
            return RedirectToAction("Login", "Auth");
        }

        var profile = await _api.GetAsync<CustomerProfileViewModel>("api/customer/profile");
        if (profile == null)
        {
            ViewBag.Error = "Phiên đăng nhập đã hết hạn hoặc không thể tải dữ liệu.";
            return RedirectToAction("Login", "Auth");
        }
        return View(profile);
    }

    [HttpPost]
    public async Task<IActionResult> Profile(CustomerProfileViewModel profile, IFormFile? avatarFile)
    {
        try
        {
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(profile.Email ?? ""), "Email");
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
            return View(updatedProfile);
        }
        catch (Exception e)
        {
            ViewBag.Error = e.Message;
            return View(profile);
        }
    }
    
    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(model.OldPassword ?? ""), "OldPassword");
            content.Add(new StringContent(model.NewPassword ?? ""), "NewPassword");
            content.Add(new StringContent(model.ConfirmNewPassword ?? ""), "ConfirmNewPassword");

            var result = await _api.PutMultipartAsync<object>("api/customer/change-password", content);

            ViewBag.Success = "Password changed successfully!";
            return View(new ChangePasswordViewModel());
        }
        catch (Exception e)
        {
            ViewBag.Error = e.Message;
            return View(model);
        }
    }
}