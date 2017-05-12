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
            _oAuth1 = Helpers.SocialNetworkAuthenticators.TwitterAuth;
        }

        public async Task<SocialAccount> Login()
        {
            SocialAccount account = null;
            _uiViewController = _oAuth1.GetUI();

            _oAuth1.Completed += async (sender, args) =>
            {
                account = await Helpers.SocialNetworkAuthenticators.OnCompliteTwitterAuth(args);

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
