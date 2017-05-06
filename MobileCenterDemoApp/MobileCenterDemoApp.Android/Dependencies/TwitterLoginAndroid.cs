using System;
using System.Threading.Tasks;
using Android.Content;
using MobileCenterDemoApp.Droid.Dependencies;
using MobileCenterDemoApp.Interfaces;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Social.Services;

[assembly: Dependency(typeof(TwitterLoginAndroid))]
namespace MobileCenterDemoApp.Droid.Dependencies
{
    public class TwitterLoginAndroid : ITwitter
    {
        public event Action<Account> OnConnect;

        public TwitterService Service { get; }
        private readonly Intent _authUi;
        private bool _isLogin;
        private Account _account;

        public TwitterLoginAndroid()
        {
            Service = Helpers.SocialNetworServices.TwitterService;
            _authUi = Service.GetAuthenticateUI(MainActivity.Activity, AuthSuccess);
            
        }

        private void AuthSuccess(Account account)
        {
            _isLogin = true;
            _account = account;
            OnConnect?.Invoke(account);
        }

        public async Task<Account> Login()
        {
            MainActivity.Activity.StartActivityForResult(_authUi, 42);
            return await Task.Run(() =>
            {
                while (!_isLogin)
                    Task.Delay(100);

                return _account;
            });
        }
    }
}