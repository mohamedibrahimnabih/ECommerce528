using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce528.Services.IServices
{
    public interface IAccountService
    {
        bool IsLogined(ClaimsPrincipal User);
        Task SendMailAsync(ApplicationUser user, IUrlHelper url, HttpRequest request, EmailType emailType = EmailType.Register);
    }
}
