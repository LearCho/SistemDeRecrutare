using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Owin;
using SistemRecrutare.Facebook;
using SistemRecrutare.Models;

namespace SistemRecrutare
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            //var cookieOptions = new CookieAuthenticationOptions
            //{
            //    LoginPath = new PathString("/Account/LoginFB")
            //};

            //app.UseCookieAuthentication(cookieOptions);

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Home/Index"),
                SlidingExpiration = true
                //Provider = new CookieAuthenticationProvider
                //{
                //    // Enables the application to validate the security stamp when the user logs in.
                //    // This is a security feature which is used when you change a password or add an external login to your account.  
                //    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, Utilizator>(
                //        validateInterval: TimeSpan.FromMinutes(30),
                //        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                //}
            });            
           // app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
           // app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //var facebookOptions = new FacebookAuthenticationOptions()
            //{
            //    AppId = "1191785347835839",
            //    AppSecret = "1b710adf3960e87e2b181eb339da5be5",
            //    BackchannelHttpHandler = new FacebookBackChannelHandler(),
            //    UserInformationEndpoint = "http://graph.facebook.com/v2.4/me?fields=id,email"
            //};
            //facebookOptions.Scope.Add("email");
            //app.UseFacebookAuthentication(facebookOptions);

            //app.UseFacebookAuthentication(
            //   appId: "1191785347835839",
            //   appSecret: "1b710adf3960e87e2b181eb339da5be5");

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "397772867081-1nimjg1gg1ohg266ul9rca9jamtsm1kq.apps.googleusercontent.com",
                ClientSecret = "UwJMPG6nggk-gltFvEmYNkew",
                CallbackPath = new PathString("/GoogleLoginCallback")
            });
        }
    }
}