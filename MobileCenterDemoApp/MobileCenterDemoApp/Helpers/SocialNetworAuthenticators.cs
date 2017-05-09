using System;
using Xamarin.Auth;

namespace MobileCenterDemoApp.Helpers
{
    public static class SocialNetworAuthenticators
    {
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
