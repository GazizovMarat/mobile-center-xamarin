using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MobileCenterDemoApp.Interfaces;
using MobileCenterDemoApp.Models;
using UIKit;
using Xamarin.Auth;
using Xamarin.Forms;


namespace MobileCenterDemoApp.iOS.Dependencies
{
    // ReSharper disable once InconsistentNaming
    public class TwitterLoginIOS : ITwitter
    {
        public event Action<string> OnError;
        private readonly OAuth1Authenticator _oAuth1;
        private UIViewController _uiViewController;
        private bool _isComplite;

        public TwitterLoginIOS()
        {
            _oAuth1 = Helpers.SocialNetworAuthenticators.TwitterAuth;
        }

        public async Task<SocialAccount> Login()
        {
            SocialAccount account = null;
            _oAuth1.Completed += async (sender, args) =>
            {
                if (!args.IsAuthenticated)
                {
                    _isComplite = true;
                    return;
                }

                account = new SocialAccount
                {
                    UserId = args.Account.Properties["user_id"],
                    UserName = args.Account.Properties["screen_name"]
                };

                var request = new OAuth1Request("GET",
                    new Uri("https://api.twitter.com/1.1/account/verify_credentials.json"),
                    null, args.Account);
                var response = await request.GetResponseAsync();

                var jo = Newtonsoft.Json.Linq.JObject.Parse(response.GetResponseText());
                var uri = (string)jo["profile_image_url"];
                account.ImageSource = ImageSource.FromUri(new Uri(uri));

                _isComplite = true;
            };
            _oAuth1.Error += (sender, args) => OnError?.Invoke(args.Message);

            UIWindow windows = AppDelegate.UiWindow;
            windows.RootViewController = _uiViewController;
            windows.MakeKeyAndVisible();

            return await Task.Run(() =>
            {
                while (!_isComplite)
                    Task.Delay(100);

                return account;
            });
        }

    }
}
