using Microsoft.AspNetCore.Mvc;

namespace ECommerce528.Areas.Identity.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        //public IActionResult ConfirmAccount()
        //{
        //    return View();
        //}

        //public IActionResult ResendConfirmation()
        //{
        //    return View();
        //}

        //public IActionResult ForgetPassword()
        //{
        //    return View();
        //}

        //public IActionResult ResetPassword()
        //{
        //    return View();
        //}

        //public IActionResult ValidOTP()
        //{
        //    return View();
        //}
    }
}
