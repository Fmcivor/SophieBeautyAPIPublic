using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
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
                using (SmtpClient client = new SmtpClient("smtppro.zoho.eu", 587))
                {
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(
                        _config["emailUsername"],
                        _config["emailAppPassword"]
                    );

                    using (MailMessage message = new MailMessage())
                    {
                        message.To.Add(newBooking.email);
                        message.From = new MailAddress(_config["emailUsername"]);
                        message.Subject = "Booking Confirmation";
                        message.IsBodyHtml = true;

                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "BookingConfirmation.html");
                        string htmlBody = File.ReadAllText(filePath);

                        var ukZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
                        var treatmentTime = TimeZoneInfo.ConvertTimeFromUtc(newBooking.appointmentDate, ukZone);
                        string formattedDate = treatmentTime.ToString("dd/MM/yyyy HH:mm");

                        htmlBody = htmlBody.Replace("{{customer_name}}", newBooking.customerName);
                        htmlBody = htmlBody.Replace("{{service_name}}", newBooking.treatmentName);
                        htmlBody = htmlBody.Replace("{{start_datetime}}", formattedDate);
                        htmlBody = htmlBody.Replace("{{price}}", "£" + newBooking.cost.ToString());
                        htmlBody = htmlBody.Replace("{{duration}}", newBooking.duration.ToString() + " Minutes");
                        htmlBody = htmlBody.Replace("{{payment_method}}", "Cash");
                        htmlBody = htmlBody.Replace("{{contact_url}}", "mailto:" + _config["emailUsername"]);

                        message.Body = htmlBody;

                        await client.SendMailAsync(message);
                    }
                }

            }
            catch (SmtpException smtpEx)
            {
                // SMTP-specific errors (authentication, connection issues)
                Console.WriteLine("SMTP Error: " + smtpEx.Message);
                if (smtpEx.InnerException != null)
                    Console.WriteLine("Inner Exception: " + smtpEx.InnerException.Message);
            }
            catch (IOException ioEx)
            {
                // File reading errors
                Console.WriteLine("IO Error reading HTML template: " + ioEx.Message);
            }
            catch (Exception ex)
            {
                // General errors
                Console.WriteLine("Unexpected error sending email: " + ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
            }
        }



    }
}