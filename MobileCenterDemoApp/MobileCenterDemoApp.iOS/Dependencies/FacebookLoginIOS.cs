using System;
using MobileCenterDemoApp.iOS.Dependencies;
using MobileCenterDemoApp.Interfaces;
using UIKit;
using Xamarin.Auth;
using Xamarin.Social.Services;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(FacebookLoginIOS))]
namespace MobileCenterDemoApp.iOS.Dependencies
{
    // ReSharper disable once InconsistentNaming
    public class FacebookLoginIOS : IFacebook
    {
        public event Action<Account> OnConnect;
        public FacebookService Service { get; }

        public FacebookLoginIOS()
        {
            Service = Helpers.SocialNetworServices.FacebookService;
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