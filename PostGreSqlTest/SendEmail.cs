using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace PostGreSqlTest
{
    public class SendEmail
    {
        public Task SendOneEmail( string email)
        {
            var myMessage = new SendGridMessage()
            {
                From = new EmailAddress("savvyitsol@gmail.com", "Wineoutlet"),
                Subject = "Wine Hangouts Verification Mail",
                PlainTextContent = "Hello, Email!",
                HtmlContent = "<strong>Hello, Email!</strong><br /><br />Please click the following link to download the app from Google Play Store<br /><a href =" + "https://play.google.com/store/apps/details?id=com.savvyitsol.winehangoutz&hl=en> Click here for the android App.</a><br /><br />Or you can download the IOS app from " +
                "<br /><a href =" + " https://itunes.apple.com/in/app/wine-hangouts/id1206442007?mt=8> Click here for the IOS App"
            };
            //ActivationCode=" + activationCode) + 
           // myMessage.AddCc(new EmailAddress("dubeyankur@gmail.com","Check"));
            myMessage.AddTo(new EmailAddress("soumpunk@gmail.com", "Test User"));

            var apiKey = ConfigurationManager.AppSettings["SENDGRID_APIKEY"];
            var client = new SendGridClient(apiKey);
            var response = client.SendEmailAsync(myMessage);
            return response;
        }
    }
}
