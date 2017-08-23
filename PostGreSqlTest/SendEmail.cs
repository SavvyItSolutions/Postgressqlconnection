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
        public async Task SendOneEmail( string email,string FirstName)
        {
            var myMessage = new SendGridMessage()
            {
                From = new EmailAddress("savvyitsol@gmail.com", "Wineoutlet"),
                Subject = "Wine Hangouts Verification Mail",
                PlainTextContent = "Hello "+ FirstName + "!",
                HtmlContent = "<strong>Hello " + FirstName + "!</strong><br /><br />Please click the following link to download the app from Google Play Store<br /><a href =" + 
                "https://play.google.com/store/apps/details?id=com.savvyitsol.winehangoutz&hl=en> Click here for the android App.</a><br /><br />Or you can download the IOS app from " +
                "<br /><a href =" + " https://itunes.apple.com/in/app/wine-hangouts/id1206442007?mt=8> Click here for the IOS App"
                //HtmlContent = "<strong>Hello, Email!</strong>"
            };
            //myMessage.AddTo(new EmailAddress("soumpunk@gmail.com", "Test User"));
            myMessage.AddTo(new EmailAddress("mohana.indukuri@gmail.com", "Mohana"));
            myMessage.AddTo(new EmailAddress("soumpunk@gmail.com", "Soumik"));
            //myMessage.AddTo(new EmailAddress("justin@wineoutlet.com", "Justin"));
            //myMessage.AddTo(new EmailAddress("dubeyankur@gmail.com", "Ankur"));
            var apiKey = ConfigurationManager.AppSettings["SENDGRID_APIKEY"];
            var client = new SendGridClient(apiKey);
            var response = await client.SendEmailAsync(myMessage);
        }
    }
}
