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
            _oAuth1 = Helpers.SocialNetworkAuthenticators.TwitterAuth;
        }

        public async Task<SocialAccount> Login()
        {
            _authUi = (Intent)_oAuth1.GetUI(MainActivity.Activity);
            MainActivity.Activity.StartActivity(_authUi);
            SocialAccount account = null;
            _oAuth1.Completed += async (sender, args) =>
            {
                account = await Helpers.SocialNetworkAuthenticators.OnCompliteTwitterAuth(args);
                _isComplite = true;
            };
            _oAuth1.Error += (sender, args) => OnError?.Invoke(args.Message);

            return await Task.Run(() =>
            {
                while (!_isComplite)
                    Task.Delay(100);

                _authUi.Dispose();

                return account;
            });
        }

    }
}