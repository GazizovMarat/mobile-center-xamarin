using System;
using Xamarin.Auth;
using Xamarin.Social.Services;

namespace MobileCenterDemoApp.Helpers
{
    public static class SocialNetworServices
    {
        public static TwitterService TwitterService
            => new TwitterService
            {
                ConsumerKey = "RpQDj4XFdHRvHp4l3uOKkyDJq",
                ConsumerSecret = "qqOILC0EPMvOFdsYXbE5zkgccU5Dsuo8P7PwcDR3cGoRLRm21c",
                CallbackUrl = new Uri("")
            };

        public static FacebookService FacebookService
            => new FacebookService
            {
                ClientId = "1945815635652325",
                ClientSecret = "f5638047a74faae2250f6436a065f26c",
                RedirectUrl = new Uri("http://localhost/facebook"),
                Scope = "public_profile"
            };
        

        public static OAuth2Authenticator FacebookAuth => new OAuth2Authenticator(
            clientId: "1945815635652325",
            scope: "public_profile",
            authorizeUrl: new Uri("https://m.facebook.com/dialog/oauth/"),
        redirectUrl: new Uri("http://localhost/facebook"));

        public static OAuth1Authenticator TwitterAuth => new OAuth1Authenticator(
            consumerKey: "RpQDj4XFdHRvHp4l3uOKkyDJq",
            consumerSecret: "qqOILC0EPMvOFdsYXbE5zkgccU5Dsuo8P7PwcDR3cGoRLRm21c",
            requestTokenUrl: new Uri("https://api.twitter.com/oauth/request_token"),
            authorizeUrl: new Uri("https://api.twitter.com/oauth/authorize"),
            accessTokenUrl: new Uri("https://api.twitter.com/oauth/access_token"),
            callbackUrl: new Uri("https://www.visualstudio.com/vs/mobile-center/")
        );


    }
}
