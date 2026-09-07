using System.Net.Http.Headers;
using Ecommerce.BusinessObject.Helpers;
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
            ViewBag.Error = "Session expired or data could not be loaded.";
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
            return View(updatedProfile);
        }
        catch (Exception e)
        {
            ViewBag.Error = e.Message;
            return View(profile);
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> ChangePassword()
    {
        try 
        {
            var profile = await _api.GetAsync<CustomerProfileViewModel>("api/customer/profile");
            if (profile != null)
            {
                ViewBag.Avatar = profile.Avatar;
                ViewBag.Username = profile.Username;
            }
        }
        catch {}
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            try 
            {
                var profile = await _api.GetAsync<CustomerProfileViewModel>("api/customer/profile");
                if (profile != null)
                {
                    ViewBag.Avatar = profile.Avatar;
                    ViewBag.Username = profile.Username;
                }
            }
            catch {}
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
            try 
            {
                var profile = await _api.GetAsync<CustomerProfileViewModel>("api/customer/profile");
                if (profile != null)
                {
                    ViewBag.Avatar = profile.Avatar;
                    ViewBag.Username = profile.Username;
                }
            }
            catch {}
            return View(new ChangePasswordViewModel());
        }
        catch (Exception e)
        {
            ViewBag.Error = e.Message;
            try 
            {
                var profile = await _api.GetAsync<CustomerProfileViewModel>("api/customer/profile");
                if (profile != null)
                {
                    ViewBag.Avatar = profile.Avatar;
                    ViewBag.Username = profile.Username;
                }
            }
            catch {}
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Cart()
    {
        try
        {
            var token = Request.Cookies["jwtToken"];
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { message = "You have not logged in yet" });
                
            var cart = await _api.GetAsync<CartViewModel>("api/carts");
            if (cart != null && cart.CartItems.Any())
            {
                cart.DeliveryFee = 20000;
            }
            return View(cart);
        }
        catch
        {
            return RedirectToAction("Login", "Auth", new { message = "The session expired!" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Checkout(int? productId, int? quantity)
    {
        var model = new CheckoutViewModel();
        model.UserProfile = await _api.GetAsync<CustomerProfileViewModel>("api/customer/profile");

        if (productId.HasValue && quantity.HasValue)
        {
            var pro = await _api.GetAsync<ProductDetailViewModel>($"api/products/{productId}");
            model.CartItems.Add(new CartItemViewModel
            {
                ProductId = pro.ProductId,
                ProductName = pro.ProductName,
                Price = pro.Price,
                Quantity = quantity.Value,
                Thumbnail = pro.Thumbnail
            });
            model.SubTotal = model.CartItems.Sum(x => pro.Price * quantity.Value);
            model.DirectBuyProductId = productId;
        }
        else
        {
            var cart = await _api.GetAsync<CartViewModel>("api/carts");
            if(cart != null) 
            {
                model.CartItems = cart.CartItems;
                model.SubTotal = cart.SubTotal;
            }
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult OnlinePayment(string txn, decimal amount, string url)
    {
        ViewBag.TransactionId = txn;
        ViewBag.Amount = amount;
        ViewBag.QrUrl = url;
        return View();
    }

    [HttpGet]
    public IActionResult PaymentSuccess(string txn)
    {
        ViewBag.TransactionId = txn;
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> OrderHistory(int pageIndex = 1, int pageSize = 10)
    {
        var token = Request.Cookies["jwtToken"];
        if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { message = "You have not logged in yet" });
        try
        {
            try 
            {
                var profile = await _api.GetAsync<CustomerProfileViewModel>("api/customer/profile");
                if (profile != null)
                {
                    ViewBag.Avatar = profile.Avatar;
                    ViewBag.Username = profile.Username;
                }
            }
            catch {}

            var url = $"api/order/customer-history?pageIndex={pageIndex}&pageSize={pageSize}";
            var pageResult = await _api.GetAsync<PagedResult<OrderHistoryViewModel>>(url);
            if (pageResult == null)
                pageResult = new PagedResult<OrderHistoryViewModel>();
            return View(pageResult);
        }
        catch (Exception e)
        {
            return RedirectToAction("Login", "Auth", new { message = "Session expired or an error occurred!" });
        }
    }
}