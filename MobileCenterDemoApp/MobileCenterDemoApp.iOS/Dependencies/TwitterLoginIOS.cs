using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MobileCenterDemoApp.Interfaces;
using UIKit;
using Xamarin.Auth;
using Xamarin.Social.Services;

namespace MobileCenterDemoApp.iOS.Dependencies
{
    // ReSharper disable once InconsistentNaming
    public class TwitterLoginIOS : ITwitter
    {
        public event Action<Account> OnConnect;
        public TwitterService Service { get; }

        public TwitterLoginIOS()
        {
            Service = Helpers.SocialNetworServices.TwitterService;
            UIViewController uiViewController = Service.GetAuthenticateUI(a => OnConnect?.Invoke(a));
            UIWindow windows = AppDelegate.UiWindow;
            windows.RootViewController = uiViewController;
            windows.MakeKeyAndVisible();
        }

        public Task<Account> Login()
        {
            throw new NotImplementedException();
        }
    }
}
