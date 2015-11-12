using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using CourseAllocation.Models;
using SendGrid;
using System.Net;
using System.Configuration;
using Twilio;

namespace CourseAllocation
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
              //  RequireNonLetterOrDigit = true,
                RequireDigit = true,
             //   RequireLowercase = true,
             //   RequireUppercase = true,
            };
            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug in here.


            /*manager.RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is: {0}"
            });*/
            manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is: {0}"
            });
            manager.EmailService = new EmailService();
          /*  manager.SmsService = new SmsService();
            */
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

   public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            //return Task.FromResult(0);

            return configSendGridasync(message);
        }

        private Task configSendGridasync(IdentityMessage message)
        {
            var myMessage = new SendGridMessage();
            myMessage.AddTo(message.Destination);
            myMessage.From = new System.Net.Mail.MailAddress(
                ConfigurationManager.AppSettings["mailFromAddress"],
                ConfigurationManager.AppSettings["mailFromDisplay"]
                );


            myMessage.Subject = message.Subject;
            myMessage.Text = message.Body;
            myMessage.Html = message.Body;

            var credentials = new NetworkCredential(
                       ConfigurationManager.AppSettings["sendGridAccount"],
                       ConfigurationManager.AppSettings["sendGridPassword"]
                       );

            // Create a Web transport for sending email.
            var transportWeb = new Web(credentials);

            // Send the email.
            if (transportWeb != null)
            {
                return transportWeb.DeliverAsync(myMessage);
            }
            else
            {
                return Task.FromResult(0);
            }
        }
    }

    /* public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            //http://www.asp.net/identity/overview/features-api/two-factor-authentication-using-sms-and-email-with-aspnet-identity
            var Twilio = new TwilioRestClient(
                ConfigurationManager.AppSettings["twilioAccount"], 
                ConfigurationManager.AppSettings["twilioAuthToken"]);

            var result = Twilio.SendMessage(ConfigurationManager.AppSettings["smsFromPhone"], message.Destination, message.Body);

            // Status is one of Queued, Sending, Sent, Failed or null if the number is not valid
            //Trace.WriteLine(result.Status);

            // Twilio doesn't currently have an async API, so return success.
            return Task.FromResult(0);
           
        }
    }*/
}
