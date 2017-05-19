using MobileCenterDemoApp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Forms;

namespace MobileCenterDemoApp.Helpers
{
    public static class SocialNetworkAuthenticators
    {
        public static OAuth2Authenticator FacebookAuth => new OAuth2Authenticator(
            clientId: "120712398481198",
            scope: "public_profile",
            authorizeUrl: new Uri("https://m.facebook.com/dialog/oauth/"),           
            redirectUrl: new Uri("https://localhost/facebook"));

        public static OAuth1Authenticator TwitterAuth => new OAuth1Authenticator(
            consumerKey: "RpQDj4XFdHRvHp4l3uOKkyDJq",
            consumerSecret: "qqOILC0EPMvOFdsYXbE5zkgccU5Dsuo8P7PwcDR3cGoRLRm21c",
            requestTokenUrl: new Uri("https://api.twitter.com/oauth/request_token"),
            authorizeUrl: new Uri("https://api.twitter.com/oauth/authorize"),
            accessTokenUrl: new Uri("https://api.twitter.com/oauth/access_token"),
            callbackUrl: new Uri("com.MobileCenterDemoApp://auth")
        );

        public static async Task<SocialAccount> OnCompliteFacebookAuth(AuthenticatorCompletedEventArgs args)
        {
            if (!args.IsAuthenticated)
            {                
                return null;
            }

            OAuth2Request request = new OAuth2Request("GET", new Uri("https://graph.facebook.com/me"), null, args.Account);
            Response response = await request.GetResponseAsync();
            string text = response.GetResponseText();
            Dictionary<string, string> deserializeObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);

            SocialAccount account = new SocialAccount();
            string name;
            if (deserializeObject.TryGetValue("name", out name))
                account.UserName = name;

            string id;
            if (deserializeObject.TryGetValue("id", out id))
                account.UserId = id;

            request = new OAuth2Request("GET", new Uri($"https://graph.facebook.com/v2.9/{account.UserId}/picture"),
                new Dictionary<string, string>
                {
                        {"height", 100.ToString() },
                        {"width", 100.ToString() }
                }, args.Account);
            response = await request.GetResponseAsync();
            account.ImageSource = ImageSource.FromStream(response.GetResponseStream);

            return account;
        }

        public static async Task<SocialAccount> OnCompliteTwitterAuth(AuthenticatorCompletedEventArgs args)
        {
            if (!args.IsAuthenticated)
            {
                return null;
            }

            SocialAccount account = new SocialAccount
            {
                UserId = args.Account.Properties["user_id"],
                UserName = args.Account.Properties["screen_name"]
            };

            OAuth1Request request = new OAuth1Request("GET",
                new Uri("https://api.twitter.com/1.1/account/verify_credentials.json"),
                null, args.Account);
            Response response = await request.GetResponseAsync();

            JObject jo = JObject.Parse(response.GetResponseText());
            string uri = (string)jo["profile_image_url"];
            account.ImageSource = ImageSource.FromUri(new Uri(uri));

            return account;
        }
    }
}
