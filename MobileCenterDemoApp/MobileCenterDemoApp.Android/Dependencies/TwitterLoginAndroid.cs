using System;
using System.Threading.Tasks;
using Android.Content;
using MobileCenterDemoApp.Droid.Dependencies;
using MobileCenterDemoApp.Interfaces;
using MobileCenterDemoApp.Models;
using Xamarin.Auth;
using Xamarin.Forms;

[assembly: Dependency(typeof(TwitterLoginAndroid))]
namespace MobileCenterDemoApp.Droid.Dependencies
{
    public class TwitterLoginAndroid : ITwitter
    {
        public event Action<string> OnError;
        private readonly OAuth1Authenticator _oAuth1;
        private Intent _authUi;
        private bool _isComplite;

        public TwitterLoginAndroid()
        {
            _oAuth1 = Helpers.SocialNetworAuthenticators.TwitterAuth;
        }

        public async Task<SocialAccount> Login()
        {
            if(_authUi == null)
                _authUi = _oAuth1.GetUI(MainActivity.Activity);
            MainActivity.Activity.StartActivity(_authUi);
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
                var uri = (string) jo["profile_image_url"];
                account.ImageSource = ImageSource.FromUri(new Uri(uri));

                _isComplite = true;
            };
            _oAuth1.Error += (sender, args) => OnError?.Invoke(args.Message);

            return await Task.Run(() =>
            {
                while (!_isComplite)
                    Task.Delay(100);

                return account;
            });
        }

    }
}