using Microsoft.AspNetCore.Mvc;
using Spendix.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spendix.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        public BaseController(IServiceProvider serviceProvider)
        {
        }

        public IActionResult SetAlertMessageAndRedirect(string action, string controller, string alertMessage, AlertMessageType alertMessageType)
        {
            return SetAlertMessageAndRedirect(action, controller, alertMessage, null, alertMessageType);
        }

        public IActionResult SetAlertMessageAndRedirect(string action, string controller, string alertMessage, string alertSubMessage, AlertMessageType alertMessageType)
        {
            TempData["AlertMessage"] = alertMessage;
            TempData["AlertSubMessage"] = alertSubMessage;
            TempData["AlertMessageType"] = alertMessageType;

            return RedirectToAction(action, controller);
        }
    }
}