using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using Azure.Communication.Email;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using sophieBeautyApi.Models;

namespace sophieBeautyApi.services
{
    public class emailService
    {

        private readonly IConfiguration _config;

        public emailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task Send(booking newBooking)
        {
            try
            {
                var client = new EmailClient(_config["AzureEmailConnString"]);





                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "BookingConfirmation.html");
                string htmlBody = File.ReadAllText(filePath);

                var ukZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
                var treatmentTime = TimeZoneInfo.ConvertTimeFromUtc(newBooking.appointmentDate, ukZone);
                string formattedDate = treatmentTime.ToString("dd/MM/yyyy HH:mm");

                htmlBody = htmlBody.Replace("{{customer_name}}", newBooking.customerName);
                // htmlBody = htmlBody.Replace("{{service_name}}", newBooking.treatmentNames);
                htmlBody = htmlBody.Replace("{{start_datetime}}", formattedDate);
                htmlBody = htmlBody.Replace("{{price}}", "£" + newBooking.cost.ToString());
                htmlBody = htmlBody.Replace("{{duration}}", newBooking.duration.ToString() + " Minutes");
                htmlBody = htmlBody.Replace("{{payment_method}}", "Cash");
                htmlBody = htmlBody.Replace("{{contact_url}}", "mailto:" + _config["emailUsername"]);


                var recipients = new EmailRecipients(new[] { new EmailAddress(newBooking.email) });


                var content = new EmailContent("Booking Confirmation")
                {
                    Html = htmlBody,
                    PlainText = $"Hi {newBooking.customerName}, your booking on {formattedDate} is confirmed."
                };

                var message = new EmailMessage(
                    senderAddress: _config["fromEmail"],  // verified sender
                    recipients: recipients,
                    content: content
                );


                var result = await client.SendAsync(Azure.WaitUntil.Completed, message);



            }
            catch (Exception ex)
            {
                Console.WriteLine("Azure Email Error: " + ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine(ex.InnerException.Message);
            }
        }



    }
}