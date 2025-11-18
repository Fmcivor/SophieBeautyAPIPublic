using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using Azure;
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





                var filePath = Path.Combine(AppContext.BaseDirectory, "BookingConfirmation.html");
                string htmlBody = File.ReadAllText(filePath);

                var ukZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
                var treatmentTime = TimeZoneInfo.ConvertTimeFromUtc(newBooking.appointmentDate, ukZone);
                string formattedDate = treatmentTime.ToString("dd/MM/yyyy HH:mm");

                string treatmentHtml = "";

                foreach (var treatment in newBooking.treatmentNames)
                {
                    treatmentHtml += "<div>" + treatment + "</div>";
                }

                htmlBody = htmlBody.Replace("{{customer_name}}", newBooking.customerName);
                htmlBody = htmlBody.Replace("{{service_name}}", treatmentHtml);
                htmlBody = htmlBody.Replace("{{start_datetime}}", formattedDate);
                htmlBody = htmlBody.Replace("{{price}}", "£" + newBooking.cost.ToString());
                htmlBody = htmlBody.Replace("{{duration}}", newBooking.duration.ToString() + " Minutes");
                htmlBody = htmlBody.Replace("{{payment_method}}", "Cash");
                htmlBody = htmlBody.Replace("{{contact_url}}", "mailto:" + "info@beautybysophieee.com");

                var emailMessage = new EmailMessage(
                    senderAddress: "DoNotReply@shapedbysophiee.com",
                    content: new EmailContent("Booking Confirmation")
                    {
                        PlainText = @"Your booking at shaped by sophiee was successful",
                        Html = htmlBody
                    },
                    recipients: new EmailRecipients(new List<EmailAddress>
                    {
                        new EmailAddress(newBooking.email)
                    }));


                EmailSendOperation emailSendOperation = client.Send(
                    WaitUntil.Started,
                    emailMessage);


            }
            catch (Exception ex)
            {
                Console.WriteLine("Azure Email Error: " + ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine(ex.InnerException.Message);
            }
        }


        public async Task sendCancellation(booking cancelledBooking)
        {
            try
            {
                var client = new EmailClient(_config["AzureEmailConnString"]);

                var filePath = Path.Combine(AppContext.BaseDirectory, "bookingCancelled.html");
                string htmlBody = File.ReadAllText(filePath);

                var ukZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
                var treatmentTime = TimeZoneInfo.ConvertTimeFromUtc(cancelledBooking.appointmentDate, ukZone);
                string formattedDate = treatmentTime.ToString("dd/MM/yyyy HH:mm");

                string treatmentHtml = "";

                foreach (var treatment in cancelledBooking.treatmentNames)
                {
                    treatmentHtml += "<div>" + treatment + "</div>";
                }

                htmlBody = htmlBody.Replace("{{customer_name}}", cancelledBooking.customerName);
                htmlBody = htmlBody.Replace("{{service_name}}", treatmentHtml);
                htmlBody = htmlBody.Replace("{{start_datetime}}", formattedDate);
                htmlBody = htmlBody.Replace("{{price}}", "£" + cancelledBooking.cost.ToString());
                htmlBody = htmlBody.Replace("{{duration}}", cancelledBooking.duration.ToString() + " Minutes");
                htmlBody = htmlBody.Replace("{{payment_method}}", "Cash");
                htmlBody = htmlBody.Replace("{{contact_url}}", "mailto:" + "info@beautybysophieee.com");

                var emailMessage = new EmailMessage(
                    senderAddress: "DoNotReply@shapedbysophiee.com",
                    content: new EmailContent("Booking Confirmation")
                    {
                        PlainText = @"Your booking at shaped by sophiee was successful",
                        Html = htmlBody
                    },
                    recipients: new EmailRecipients(new List<EmailAddress>
                    {
                        new EmailAddress(cancelledBooking.email)
                    }));


                EmailSendOperation emailSendOperation = client.Send(
                    WaitUntil.Started,
                    emailMessage);


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
