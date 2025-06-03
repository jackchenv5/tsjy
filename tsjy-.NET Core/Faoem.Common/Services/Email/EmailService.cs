using Faoem.Common.Exceptions;
using Faoem.Common.Options;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Faoem.Common.Services.Email;

internal class EmailService(IConfiguration configuration) : IEmailService
{
    public async Task SendAsync(MimeMessage mimeMessage)
    {
        var emailOptions = configuration.GetSection("Email").Get<EmailOptions>();
        if (emailOptions is null)
        {
            throw new AppException("Email options are not found.");
        }

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(emailOptions.Host, emailOptions.Port);
            await client.AuthenticateAsync(emailOptions.Username, emailOptions.Password);
            mimeMessage.From.Add(new MailboxAddress(emailOptions.From, emailOptions.Username));

            await client.SendAsync(mimeMessage);
            await client.DisconnectAsync(true);
        }
        catch (Exception e)
        {
            if (e is ObjectDisposedException)
            {
                // 成功发送但是断开连接时出现错误
                return;
            }

            throw new AppException("Failed to send email.", 500);
        }
    }

    public async Task<bool> ValidDomainAsync(string email)
    {
        var emailOptions = configuration.GetSection("Email").Get<EmailOptions>();
        var blackList = emailOptions?.BlackList?.Select(s => s.ToLower());
        var whiteList = emailOptions?.WhiteList?.Select(s => s.ToLower());

        var domain = email.Split('@')[1].ToLower();

        if (whiteList is not null)
        {
            // 白名单优先
            // 存在白名单时，邮件域必须在白名单中
            return await Task.FromResult(whiteList.Contains(domain));
        }

        // 白名单为空时
        if (blackList is null)
        {
            // 如果黑名单为空，则默认通过
            return await Task.FromResult(true);
        }

        // 黑名单不为空时，邮件域不能在黑名单中
        return await Task.FromResult(!blackList.Contains(domain));
    }
}