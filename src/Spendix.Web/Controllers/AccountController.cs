using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Spendix.Web.ViewModels.Account;

namespace Spendix.Web.Controllers
{
    [Route("Account")]
    public class AccountController : BaseController
    {
        public AccountController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet, Route("SignIn")]
        public IActionResult SignIn(string returnUrl)
        {
            var vm = new SignInViewModel
            {
                ReturnUrl = returnUrl
            };

            return View(vm);
        }
    }
}