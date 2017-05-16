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

        private bool _isComplite;

        public async Task<SocialAccount> Login()
        {
            _isComplite = false;
            OAuth1Authenticator _oAuth1 = Helpers.SocialNetworkAuthenticators.TwitterAuth;
            SocialAccount account = null;
            UIViewController _uiViewController = (UIViewController)_oAuth1.GetUI();
            
            _oAuth1.Completed += async (sender, args) =>
            {
                account = await Helpers.SocialNetworkAuthenticators.OnCompliteTwitterAuth(args);
                _isComplite = true;
            };
            _oAuth1.Error += (sender, args) => OnError?.Invoke(args.Message);

            using (UIWindow window = new UIWindow(UIScreen.MainScreen.Bounds))
            {
                window.RootViewController = _uiViewController;
                window.MakeKeyAndVisible();

                return await Task.Run(() =>
                {
                    while (!_isComplite)
                        Task.Delay(100);

                    return account;
                });
            }
        }

    }
}
