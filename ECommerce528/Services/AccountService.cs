using Azure.Core;
using ECommerce528.Areas.Identity.Controllers;
using ECommerce528.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce528.Services
{
    public enum EmailType
    {
        Register,
        ResendConfirmation
    }

    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public AccountService(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public bool IsLogined(ClaimsPrincipal User)
        {
            if (User is not null && User.Identity.IsAuthenticated)
                return true;

            return false;
        }

        public async Task SendMailAsync(ApplicationUser user, IUrlHelper url, HttpRequest request, EmailType emailType = EmailType.Register)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var link = url.Action(nameof(AccountController.Confirm), SD.ACCOUNT_CONTROLER, new { area = SD.IDENTITY_AREA, token, user.Id }, request.Scheme);

            string subject = string.Empty;
            string message = string.Empty;

            switch (emailType)
            {
                case EmailType.Register:
                    {
                        subject = "Confirmation Your Account in Ecommerce APP";
                        message = $"<h1>Confirm Your Account By Clicking <a href='{link}'>Here</a></h1>";
                    }
                    break;
                case EmailType.ResendConfirmation:
                    {
                        subject = "Resend - Confirmation Your Account in Ecommerce APP";
                        message = $"<h1>Confirm Your Account By Clicking <a href='{link}'>Here</a></h1>";
                    }
                    break;
            }

            await _emailSender.SendEmailAsync(user.Email!, subject, message);
        }
    }
}
