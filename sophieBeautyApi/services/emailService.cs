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

        public async Task send(booking newBooking)
        {

            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(_config["emailUsername"], _config["emailAppPassword"]);


            MailMessage message = new MailMessage();
            message.To.Add(newBooking.email);
            message.From = new MailAddress(_config["emailUsername"]);
            message.Subject = "Booking Confirmation";
            message.IsBodyHtml = true;

            var filePath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "BookingConfirmation.html");
            string htmlBody = File.ReadAllText(filePath);
            message.Body = htmlBody;

            var ukZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            var treatmentTime = TimeZoneInfo.ConvertTimeFromUtc(newBooking.appointmentDate, ukZone);
            string formattedDate = treatmentTime.ToString("dd/MM/yyyy HH:mm");


            htmlBody.Replace("{{customer_name}}", newBooking.customerName);
            htmlBody.Replace("{{service_name}}", newBooking.treatmentName);
            htmlBody.Replace("{{start_datetime}}", formattedDate);
            htmlBody.Replace("{{price}}", "£" + newBooking.cost.ToString());
            htmlBody.Replace("{{duration}}", newBooking.duration.ToString() + " Minutes");
            htmlBody.Replace("{{payment_method}}", "Cash");
            htmlBody.Replace("{{contact_url}}", "mailto:" + _config["emailUsername"]);

            client.SendMailAsync(message);


        }


    }
}