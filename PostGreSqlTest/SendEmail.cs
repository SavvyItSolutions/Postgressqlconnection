using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
using NLog;

namespace PostGreSqlTest
{
    public class SendEmail
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        public async Task SendOneEmail(string email, string FirstName)
        {
            try
            {

                var myMessage = new SendGridMessage()
                {
                    From = new EmailAddress("savvyitsol@gmail.com", "Wineoutlet"),
                    Subject = "Welcome to Wine Outlet!",
                    PlainTextContent = "Hello " + FirstName + ",",
                    HtmlContent = "<strong>Hello " + FirstName + "!</strong><br /><br />Welcome to Wine Outlet!<br /><br />Our exciting Wine Outlet mobile app lets you to view the wines available for free tasting and also you can Rate, Review and View your tastings and more. <br /><br />Please click the following link to download the IOS app from <br /><a href =" +
                    "https://itunes.apple.com/us/app/wine-outlet/id1208420358?mt=8> Click here for the iOS App.</a><br /><br />Or Download the Android app from Google Play Store " +
                    "<br /><a href =" + " https://play.google.com/store/apps/details?id=com.savvyitsol.wineoulet&hl=en> Click here for the Android App</a>"

                    //HtmlContent = "<strong>Hello, Email!</strong>"
                };
                myMessage.AddTo(new EmailAddress(email, FirstName));
                //myMessage.AddTo(new EmailAddress("soumpunk@gmail.com", "Test User"));
                myMessage.AddBcc(new EmailAddress("mohana.indukuri@gmail.com", "Mohana"));
                myMessage.AddBcc(new EmailAddress("soumpunk@gmail.com", "Soumik"));
                //myMessage.AddTo(new EmailAddress("justin@wineoutlet.com", "Justin"));
                //myMessage.AddTo(new EmailAddress("dubeyankur@gmail.com", "Ankur"));
                var apiKey = ConfigurationManager.AppSettings["SENDGRID_APIKEY"];
                var client = new SendGridClient(apiKey);
                var response = await client.SendEmailAsync(myMessage);
                //log email sent
                logger.Info("Mail Sent" + email);
            }
            catch (Exception ex)
            {
                logger.Info("Mail not sent" + email + " Error:" + ex.Message);
                //Email sent threw error : ex.message 
                // throw;
            }
        }
    }
}
