using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Spendix.Core;
using Spendix.Core.Entities;
using Spendix.Core.Repos;
using Spendix.Web.ViewModels.Account;

namespace Spendix.Web.Controllers
{
    [Authorize]
    [Route("Account")]
    public class AccountController : BaseController
    {
        private readonly SpendixDbContext spendixDbContext;
        private readonly UserAccountRepo userAccountRepo;

        public AccountController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            spendixDbContext = serviceProvider.GetService<SpendixDbContext>();
            userAccountRepo = serviceProvider.GetService<UserAccountRepo>();
        }

        [AllowAnonymous]
        [HttpGet, Route("SignIn")]
        public IActionResult SignIn(string returnUrl)
        {
            var vm = new SignInViewModel
            {
                ReturnUrl = returnUrl
            };

            return View(vm);
        }

        [AllowAnonymous]
        [HttpGet, Route("SignUp")]
        public IActionResult SignUp()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost, Route("SignUp")]
        public async Task<IActionResult> SignUp(IFormCollection form)
        {
            var userAccount = new UserAccount
            {
                EmailAddress = form["emailAddress"],
                Password = BCrypt.Net.BCrypt.HashPassword(form["password"])
            };

            userAccountRepo.PrepareEntityForCommit(userAccount);

            await spendixDbContext.SaveChangesAsync();

            await SignInUserAccount(userAccount);

            return RedirectToAction("Dashboard", "Home");
        }

        #region Api

        [AllowAnonymous]
        [HttpPost, Route("/api/Account/UniqueEmail")]
        public async Task<IActionResult> UniqueEmail(string email)
        {
            var isUniqueEmail = await userAccountRepo.FindIsUniqueEmailAddress(email);

            return Json(isUniqueEmail);
        }

        #endregion Api

        private async Task SignInUserAccount(UserAccount userAccount)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, userAccount.EmailAddress),
                new Claim("UserAccountId", userAccount.UserAccountId.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTime.UtcNow.AddDays(7),
                IsPersistent = true

                //AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.

                //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                // The time at which the authentication ticket expires. A
                // value set here overrides the ExpireTimeSpan option of
                // CookieAuthenticationOptions set with AddCookie.

                //IsPersistent = true,
                // Whether the authentication session is persisted across
                // multiple requests. Required when setting the
                // ExpireTimeSpan option of CookieAuthenticationOptions
                // set with AddCookie. Also required when setting
                // ExpiresUtc.

                //IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.

                //RedirectUri = <string>
                // The full path or absolute URI to be used as an http
                // redirect response value.
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
        }
    }
}