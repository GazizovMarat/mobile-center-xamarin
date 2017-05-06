using System;
using System.Collections.Generic;
using Android.Security.Keystore;
using Microsoft.Azure.Mobile.Analytics;
using MobileCenterDemoApp.Services;
using Xamarin.Social;
using Xamarin.Social.Services;

namespace MobileCenterDemoApp.Helpers
{
    public class SocialNetworServices
    {
        public static TwitterService TwitterService
            => new TwitterService
            {
                ConsumerKey = "S2b5nyV5G1GzLxaNsyiRATTfX",
                ConsumerSecret = "Wao0SjuBmi8gJ203nvsU5rhSYGtdv80nO5pOysSeF0OWpJVCK8",
                CallbackUrl = new Uri("http://a2r5es95w1.org/")
            };

        public static FacebookService FacebookService
            => new FacebookService
            {
                ClientId = "120712398481198",
                ClientSecret = "4d28516d4fe6f9b50c03c8bed20d5d91",
                RedirectUrl = new Uri("http://a2r5es95w1.org/"),
                Scope = "publish_actions"
            };

        public static async void Share(Item item)
        {
            if (DataStore.FacebookService != null)
                await DataStore.FacebookService.ShareItemAsync(item, DataStore.Account);
            else if (DataStore.TwitterService != null)
                await DataStore.TwitterService.ShareItemAsync(item, DataStore.Account);
            else
                throw new UserNotAuthenticatedException();

            Analytics.TrackEvent("Share", new Dictionary<string, string> {{"Text", item.Text}});
        }
    }
}
