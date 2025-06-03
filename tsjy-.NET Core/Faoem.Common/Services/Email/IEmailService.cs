using MimeKit;

namespace Faoem.Common.Services.Email;

public interface IEmailService
{
    public Task SendAsync(MimeMessage mimeMessage);
    public Task<bool> ValidDomainAsync(string email);
}