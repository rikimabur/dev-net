using EshopApplication.Abstractions;

namespace EshopInfrastructure.Email
{
    public sealed class SmtpEmailSender : IEmailSender
    {
        public Task SendAsync(
            string to,
            string subject,
            string body,
            CancellationToken cancellationToken = default)
        {
            // SMTP / SendGrid / SES logic here
            Console.WriteLine($"Sending email to {to}");
            return Task.CompletedTask;
        }
    }
}
