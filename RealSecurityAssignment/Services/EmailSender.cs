using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MyCompany.Services;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace RealSecurityAssignment.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _config;

        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor,
                           ILogger<EmailSender> logger, IConfiguration configuration)
        {
            Options = optionsAccessor.Value;
            _logger = logger;
            _config = configuration;
        }

        public AuthMessageSenderOptions Options { get; } //Set with Secret Manager.

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            await Execute(subject, message, toEmail);
        }

        public async Task Execute(string subject, string message, string toEmail)
        {
            var client = new SendGridClient(_config["SendGrid:Key"]);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("samuelyuoh@gmail.com", "Security Assignment"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(toEmail));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);
            var response = await client.SendEmailAsync(msg);
            _logger.LogInformation(response.IsSuccessStatusCode
                                   ? $"Email to {toEmail} queued successfully!"
                                   : $"{response.StatusCode.ToString()}");
        }
        public async Task Executeotp(string subject, string message, string toEmail)
        {
            var client = new SendGridClient(_config["SendGrid:Key"]);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("samuelyuoh@gmail.com", "OTP"),
                Subject = subject,
                PlainTextContent = "Your OTP is "+ message,
                HtmlContent = "Your OTP is " + message
            };
            msg.AddTo(new EmailAddress(toEmail));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);
            var response = await client.SendEmailAsync(msg);
            _logger.LogInformation(response.IsSuccessStatusCode
                                   ? $"Email to {toEmail} queued successfully!"
                                   : $"{response.StatusCode.ToString()}");
        }
    }
}