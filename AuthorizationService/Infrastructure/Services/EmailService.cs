using System.Threading.Tasks;
using AuthorizationService.Core.Interfaces;

namespace AuthorizationService.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Task.CompletedTask;
        }
    }
}
