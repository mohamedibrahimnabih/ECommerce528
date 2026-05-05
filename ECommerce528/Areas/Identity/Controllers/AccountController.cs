using ECommerce528.Areas.Admin.Controllers;
using ECommerce528.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace ECommerce528.Areas.Identity.Controllers
{
    [Area(SD.IDENTITY_AREA)]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IAccountService _accountService;
        private readonly IRepository<ApplicationUserOTP> _applicationUserOTPRepository;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, IAccountService accountService, IRepository<ApplicationUserOTP> applicationUserOTPRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _accountService = accountService;
            _applicationUserOTPRepository = applicationUserOTPRepository;
        }

        // Service Layer of application User => UserManager<ApplicationUser>
        // Repo Layer of application User => UserStore<ApplicationUser>

        // Service Layer of identity Role => RoleManager<IdentityUser>
        // Repo Layer of identity Role => RoleStore<ApplicationUser>


        public IActionResult AccessDenied()
        {
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (_accountService.IsLogined(User))
                return RedirectToAction(nameof(HomeController.Index), SD.HOME_CONTROLER, new { area = SD.CUSTOMER_AREA });


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if(!ModelState.IsValid)
                return View(registerVM);

            ApplicationUser user = new()
            {
                FirstName = registerVM.FName,
                LastName = registerVM.FName,
                Email = registerVM.Email,
                UserName = registerVM.UserName,
                Address = registerVM.Address,
            };

            var result = await _userManager.CreateAsync(user, registerVM.Password);
            // Password must contain (lower case, upper case, digits, special char, more than 6 char)

            if(!result.Succeeded)
            {
                foreach (var item in result.Errors)
                    ModelState.AddModelError(string.Empty, item.Description);

                return View(registerVM);
            }

            // Send Email Confirmation
            await _accountService.SendMailAsync(user, Url, Request);

            await _userManager.AddToRoleAsync(user, RoleNames.CUSTOMER);

            TempData["success_notification"] = "Add Account Successfully, check you email";

            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Confirm(string token, string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is null) return NotFound();

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if(!result.Succeeded)
                TempData["error_notification"] = String.Join(",", result.Errors.Select(e => e.Description));

            TempData["success_notification"] = "Confirm Email Successfully, You can login now";

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (_accountService.IsLogined(User))
                return RedirectToAction(nameof(HomeController.Index), SD.HOME_CONTROLER, new { area = SD.CUSTOMER_AREA });

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
                return View(loginVM);

            var user = await _userManager.FindByEmailAsync(loginVM.EmailORUserName) ?? await _userManager.FindByNameAsync(loginVM.EmailORUserName);

            if (user is null)
            {
                ModelState.AddModelError(nameof(LoginVM.EmailORUserName), "Invalid User Name Or Email");
                ModelState.AddModelError(nameof(LoginVM.Password), "Invalid Password");

                return View(loginVM);
            }

            #region Old way
            //var result = await _userManager.CheckPasswordAsync(user, loginVM.Password);

            //if(!result)
            //{
            //    ModelState.AddModelError(nameof(LoginVM.EmailORUserName), "Invalid User Name Or Email");
            //    ModelState.AddModelError(nameof(LoginVM.Password), "Invalid Password");

            //    return View(loginVM);
            //}

            //if(!user.EmailConfirmed)
            //{
            //    ModelState.AddModelError(nameof(LoginVM.EmailORUserName), "Confirm Your Email First");

            //    return View(loginVM);
            //} 
            #endregion

            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.RememberMe, true);

            if(!result.Succeeded)
            {
                ModelState.AddModelError(nameof(LoginVM.EmailORUserName), "Invalid User Name Or Email");
                ModelState.AddModelError(nameof(LoginVM.Password), "Invalid Password");

                return View(loginVM);
            }

            if(result.IsNotAllowed)
            {
                ModelState.AddModelError(nameof(LoginVM.EmailORUserName), "Confirm Your Email First");

                return View(loginVM);
            }

            TempData["success_notification"] = $"Welcome Back {user.FirstName} {user.LastName}";

            return RedirectToAction(nameof(HomeController.Index), SD.HOME_CONTROLER, new { area = SD.CUSTOMER_AREA });
        }

        [HttpGet]
        public IActionResult ResendEmailConfirmation()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationVM resendEmailConfirmationVM)
        {
            if (!ModelState.IsValid)
                return View(resendEmailConfirmationVM);

            var user = await _userManager.FindByEmailAsync(resendEmailConfirmationVM.EmailORUserName) ?? await _userManager.FindByNameAsync(resendEmailConfirmationVM.EmailORUserName);

            if(user is not null && !user.EmailConfirmed)
                await _accountService.SendMailAsync(user, Url, Request, EmailType.ResendConfirmation);

            TempData["success_notification"] = $"Resend Email Confirmation successfully, please check yoy email";

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM forgetPasswordVM)
        {
            if (!ModelState.IsValid)
                return View(forgetPasswordVM);

            var user = await _userManager.FindByEmailAsync(forgetPasswordVM.EmailORUserName) ?? await _userManager.FindByNameAsync(forgetPasswordVM.EmailORUserName);

            if(user is not null)
            {
                await _accountService.SendMailAsync(user, Url, Request, EmailType.ForgetPassword);
            }

            TempData["success_notification"] = "Send OTP number successfully, Please check your email";
            TempData["userId"] = user?.Id;

            return RedirectToAction(nameof(ValidateOTP));
        }

        [HttpGet]
        public IActionResult ValidateOTP()
        {
            if(TempData.Peek("userId") is null)
                return NotFound();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ValidateOTP(VlidateOTPVM vlidateOTPVM)
        {
            if (!ModelState.IsValid)
                return View(vlidateOTPVM);

            //if(!TempData.ContainsKey("userId"))
            //    return NotFound();

            var userId = TempData.Peek("userId")?.ToString();

            if (userId is null) return NotFound();

            //var totalOTPs = (await _applicationUserOTPRepository.GetAsync(e => e.ApplicationUserId == userId && (DateTime.Now - e.CreateAt).TotalHours < 24)).Count();

            //if(totalOTPs > 5)
            //{
            //    TempData["error_notification"] = "You have exceeded the maximum number of OTP requests. Please try again later.";
            //    return RedirectToAction(nameof(ForgetPassword));
            //}

            var otp = await _applicationUserOTPRepository.GetOneAsync(e => e.ApplicationUserId == userId && e.OTP == vlidateOTPVM.OTP && !e.IsUsed && e.ValidTo >= DateTime.Now);

            if (otp is null)
            {
                ModelState.AddModelError(nameof(VlidateOTPVM.OTP), "Invalid OTP");
                return View(vlidateOTPVM);
            }

            otp.IsUsed = true;
            await _applicationUserOTPRepository.CommitAsync();

            return RedirectToAction(nameof(NewPassword));
        }

        [HttpGet]
        public IActionResult NewPassword()
        {
            if (TempData.Peek("userId") is null)
                return NotFound();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NewPassword(NewPasswordVM newPasswordVM)
        {
            if (!ModelState.IsValid)
                return View(newPasswordVM);

            var userId = TempData.Peek("userId")?.ToString();

            if (userId is null) return NotFound();

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null) return NotFound();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user, token, newPasswordVM.Password);

            TempData["success_notification"] = "Change Password Successfully";
            TempData["userId"] = null;

            return RedirectToAction(nameof(Login));
        }
    }
}
