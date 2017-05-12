using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MobileCenterDemoApp.iOS.Dependencies;
using MobileCenterDemoApp.Interfaces;
using MobileCenterDemoApp.Models;
using Newtonsoft.Json;
using UIKit;
using Xamarin.Auth;
using Xamarin.Forms;
using MobileCenterDemoApp.Helpers;

[assembly: Dependency(typeof(FacebookLoginIOS))]
namespace MobileCenterDemoApp.iOS.Dependencies
{
    // ReSharper disable once InconsistentNaming
    public class FacebookLoginIOS : IFacebook
    {
        private readonly OAuth2Authenticator _oAuth2;

        private bool _isComplite;
        private SocialAccount _account;
        private UIViewController _uiViewController;

        public FacebookLoginIOS()
        {
            _oAuth2 = Helpers.SocialNetworkAuthenticators.FacebookAuth;         
        }


        public async Task<SocialAccount> Login()
        {
            _uiViewController = _oAuth2.GetUI();
            _oAuth2.Completed += async (sender, args) =>
            {
                _account = await SocialNetworkAuthenticators.OnCompliteFacebookAuth(args);
                _isComplite = true;
            };
            _oAuth2.Error += (sender, args) => OnError?.Invoke(args.Message);

            UIWindow windows = AppDelegate.UiWindow;
            windows.RootViewController = _uiViewController;
            windows.MakeKeyAndVisible();
            await Task.Run(() => { while (!_isComplite) Task.Delay(100); });
            return _account;
        }

        public event Action<string> OnError;
    }
}