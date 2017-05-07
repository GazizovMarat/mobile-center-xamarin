using System;
using ImageCircle.Forms.Plugin.Abstractions;
using Microsoft.Azure.Mobile.Analytics;
using MobileCenterDemoApp.Helpers;
using MobileCenterDemoApp.Interfaces;
using MobileCenterDemoApp.Models;
using MobileCenterDemoApp.Services;
using MobileCenterDemoApp.ViewHelpers;
using MobileCenterDemoApp.Views;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace MobileCenterDemoApp.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        public bool ShowHeader => string.IsNullOrEmpty(ErrorMessage) && !ShowWait;
        public bool ShowError => !string.IsNullOrEmpty(ErrorMessage) && !ShowWait;

        private bool _showWait;
        public bool ShowWait
        {
            get => _showWait;
            set
            {
                SetProperty(ref _showWait, value);
                OnPropertyChanged(nameof(ShowHeader));
                OnPropertyChanged(nameof(ShowError));
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                SetProperty(ref _errorMessage, value); 
                OnPropertyChanged(nameof(ShowHeader));
                OnPropertyChanged(nameof(ShowError));
            }

        }        

        private IValueConverter _reverseConverter;

        public IValueConverter ReverseConverter
        {
            get => _reverseConverter;
            set => SetProperty(ref _reverseConverter, value);
        }

        public Command LoginViaFacebookCommand { get; set; }
        public Command LoginViaTwitterCommand { get; set; }
        public Command LoginCommand { get; set; }

        public LoginViewModel()
        {
            Title = "Count my steps";
            
            LoginViaFacebookCommand = new Command(LoginViaFacebook);
            LoginViaTwitterCommand = new Command(LoginViaTwitter);
            LoginCommand = new Command(() => Login(new SocialAccount
            {
                UserName = "Larry Gardner", ImageSource = ImageSource.FromUri(new Uri("http://abs.twimg.com/sticky/default_profile_images/default_profile_normal.png"))
                , UserId = "859674503487606784"
            }));
        }

        #region Auth

        private async void LoginViaFacebook()
        {
            Analytics.TrackEvent("Try login via facebook");

            if (DataStore.FacebookService == null)
            {
                IFacebook facebookService = DependencyService.Get<IFacebook>();
                if (facebookService == null)
                {
                    ErrorMessage = "Facebook auth does not implement for current platform";
                    return;
                }

                DataStore.FacebookService = facebookService;
            }
            ShowWait = true;
            Login(await DataStore.FacebookService.Login());
        }

        private async void LoginViaTwitter()
        {
            Analytics.TrackEvent("Try login via twitter");
            if (DataStore.TwitterService == null)
            {
                ITwitter twitterService = DependencyService.Get<ITwitter>();
                if (twitterService == null)
                {
                    ErrorMessage = "Twitter auth does not implement for current platform";
                    return;
                }
                DataStore.TwitterService = twitterService;
            }
            ShowWait = true;
            Login(await DataStore.TwitterService.Login());
        }

        private void Login(SocialAccount account)
        {
            ShowWait = false;
            if (account == null)
            {
                ErrorMessage = "Login failed, please try again";
                return;
            }

            DataStore.Account = account;

            Analytics.TrackEvent($"Success login ({DataStore.Account.UserName})");
            Application.Current.MainPage = new NavigationPage(new MainPage());
        }

        #endregion
    }
}
