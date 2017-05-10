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
            _oAuth2 = Helpers.SocialNetworAuthenticators.FacebookAuth;         
        }


        public async Task<SocialAccount> Login()
        {
            _uiViewController = _oAuth2.GetUI();
            _oAuth2.Completed += async (sender, args) =>
            {
                if (!args.IsAuthenticated)
                {
                    _isComplite = true;
                    return;
                }

                var request = new OAuth2Request("GET", new Uri("https://graph.facebook.com/me"), null, args.Account);
                var response = await request.GetResponseAsync();
                var text = response.GetResponseText();
                var deserializeObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);

                _account = new SocialAccount();

                if (deserializeObject.TryGetValue("name", out string name))
                    _account.UserName = name;

                if (deserializeObject.TryGetValue("id", out string id))
                    _account.UserId = id;

                request = new OAuth2Request("GET", new Uri($"https://graph.facebook.com/v2.9/{_account.UserId}/picture"),
                    new Dictionary<string, string>
                    {
                        {"height", 100.ToString() },
                        {"width", 100.ToString() }
                    }, args.Account);

                response = await request.GetResponseAsync();

                _account.ImageSource = ImageSource.FromStream(response.GetResponseStream);

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