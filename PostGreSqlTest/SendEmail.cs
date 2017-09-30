using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
using NLog;
using System.Net.Http;
using Newtonsoft.Json;

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
                    HtmlContent = "<strong>Hello " + FirstName + "!</strong><br /><br />Welcome to Wine Outlet!<br /><br />Our exciting Wine Outlet mobile app lets you to view the wines available for free tasting and also you can Rate, Review and View your tastings and more. <br /><br />Please click the following link to download the IOS app from <br /><a href =" + "https://itunes.apple.com/us/app/wine-outlet/id1208420358?mt=8> Click here for the iOS App.</a><br /><br />Or Download the Android app from Google Play Store " +
                    "<br /><a href =" + " https://play.google.com/store/apps/details?id=com.savvyitsol.wineoulet&hl=en> Click here for the Android App</a>"

                    //HtmlContent = "<strong>Hello, Email!</strong>"
                };
               // myMessage.AddTo(new EmailAddress(email, FirstName));
                //myMessage.AddTo(new EmailAddress("soumpunk@gmail.com", "Test User"));
                //myMessage.AddTo(new EmailAddress("mohana.indukuri@gmail.com", "Mohana"));
                myMessage.AddTo(new EmailAddress("soumpunk@gmail.com", "Soumik"));
                //myMessage.AddTo(new EmailAddress("justin@wineoutlet.com", "Justin"));
                //myMessage.AddTo(new EmailAddress("dubeyankur@gmail.com", "Ankur"));
                var apiKey = ConfigurationManager.AppSettings["SENDGRID_APIKEY"];
                var client = new SendGridClient(apiKey);
                var response = await client.SendEmailAsync(myMessage);
                if (response.StatusCode == 0)
                {
                    logger.Info("Failed to send email");
                }
                else
                    logger.Info("Mail sent successfully");
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
        public async Task<int> UpdateMail(string email, string userid)
        { 
            HttpClient client = new HttpClient();

            try
            {
                if (email != "" || email != string.Empty)
                {
                    //https://hangoutz.azurewebsites.net/api/Item/SendEmail/kurellasumanth@gmail.com/user/sumanth
                    string ServiceURL = "https://hangoutz.azurewebsites.net/api/Item/";
                    var uri = new Uri(ServiceURL + "SendEmail/" + email + "/user/" + userid);
                    // var content = JsonConvert.SerializeObject(email);
                    var response = await client.GetStringAsync(uri).ConfigureAwait(false);
                    //output = JsonConvert.DeserializeObject<CustomerResponse>(response);
                    logger.Info("Mail Sent" + email);
                }
                else
                    logger.Info("Email is blank for Name :"+userid);
            }
            catch (Exception ex)
            {
                logger.Info("Mail not sent" + email + " Error:" + ex.Message);
            }

            return 1;
        }
    }
}
