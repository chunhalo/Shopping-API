using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace MixLoginProduct.Models
{
    public class EmailHelper
    {
        public bool SendEmailPasswordReset(string userEmail, Uri link)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("safechatproject8@gmail.com");
            mailMessage.To.Add(new MailAddress(userEmail));

            mailMessage.Subject = "Password Reset";
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = link.ToString();

            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential("safechatproject8@gmail.com", "nvhxmffkgslnlhow");
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            
            try
            {

                client.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                // log exception
            }
            return false;
        }

        public bool SendEmail(string userEmail, Uri confirmationlink)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("safechatproject8@gmail.com");
            mailMessage.To.Add(new MailAddress(userEmail));

            mailMessage.Subject = "Email Confirmation";
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = confirmationlink.ToString();

            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential("safechatproject8@gmail.com", "nvhxmffkgslnlhow");
            client.Host = "smtp.gmail.com";
            client.Port = 587;

            try
            {

                client.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                // log exception
            }
            return false;
        }
    }
}
