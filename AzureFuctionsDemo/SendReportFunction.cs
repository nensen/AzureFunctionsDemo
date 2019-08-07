using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Net;
using System.Threading.Tasks;

namespace AzureFuctionsDemo
{
    public static class SendReport
    {
        [FunctionName("SendReport")]
        public static void Run([TimerTrigger("0 */2 * * * *")]TimerInfo myTimer, ILogger log)
        {
            // Format: {second} {minute} {hour} {day} {month} {day-of-week}.
            // 0 */2 * * * * => run every 2 min
            // 0 30 8 * * 1-5 => run from Monday to Friday at 08:30 AM

            SendReportEmail(log).Wait();

            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }

        private static async Task SendReportEmail(ILogger log)
        {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("reportmaker@reportmaker.com");
            var to = new EmailAddress("reportreciever@reportreciever.com");
            var subject = "Here is your daily report.";
            var htmlContent = "<strong>Wow such report, such fun.</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);

            var response = await client.SendEmailAsync(msg);

            if (response.StatusCode != HttpStatusCode.Accepted ||
                response.StatusCode != HttpStatusCode.OK)
            {
                log.LogError("Send email failed.");
            }
        }
    }
}