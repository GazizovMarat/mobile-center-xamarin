using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content;
using MobileCenterDemoApp.Droid.Dependencies;
using MobileCenterDemoApp.Interfaces;
using MobileCenterDemoApp.Models;
using MobileCenterDemoApp.Services;
using Newtonsoft.Json;
using Xamarin.Auth;
using Xamarin.Forms;

[assembly:Dependency(typeof(FacebookLoginAndroid))]
namespace MobileCenterDemoApp.Droid.Dependencies
{
    public class FacebookLoginAndroid : IFacebook
    {

        private readonly OAuth2Authenticator _oAuth2;

        private readonly Intent _authUi;
        private bool _isComplite;
        private SocialAccount _account;

        public FacebookLoginAndroid()
        {
            _oAuth2 = Helpers.SocialNetworServices.FacebookAuth;
            _authUi = _oAuth2.GetUI(MainActivity.Activity);            
        }


        public async Task<SocialAccount> Login()
        {
            MainActivity.Activity.StartActivity(_authUi);
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
                var deserializeObject = JsonConvert.DeserializeObject<Dictionary<string,string>>(text);
                
                _account = new SocialAccount();

                if (deserializeObject.TryGetValue("name", out string name))
                    _account.UserName = name;

                if (deserializeObject.TryGetValue("id", out string id))
                    _account.UserId = id;

                // 
                request = new OAuth2Request("GET", new Uri($"https://graph.facebook.com/v2.9/{_account.UserId}/picture"), 
                    new Dictionary<string, string>
                    {
                        {"height", 100.ToString() },
                        {"width", 100.ToString() }
                    }, args.Account);
                response = await request.GetResponseAsync();

                _account.ImageSource = ImageSource.FromStream(response.GetResponseStream);
                

                _isComplite = true;

                DataStore.OAuth2 = _oAuth2;
            };

            await Task.Run(() => { while (!_isComplite) Task.Delay(100); });
            return _account;
        }
    }
}