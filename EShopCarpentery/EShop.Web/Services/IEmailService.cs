

using EShop.Web.Models.Email;

namespace EShop.Web.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailMessage allMails);
    }
}
