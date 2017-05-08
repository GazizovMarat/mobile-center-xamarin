using System;
using System.Collections.Generic;
using Microsoft.Azure.Mobile.Analytics;
using MobileCenterDemoApp.Helpers;
using MobileCenterDemoApp.Interfaces;
using MobileCenterDemoApp.Models;
using MobileCenterDemoApp.Services;
using MobileCenterDemoApp.Views;
using Xamarin.Forms;
// ReSharper disable ExplicitCallerInfoArgument

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
                UserName = "Larry Gardner",
                ImageSource = ImageSource.FromUri(new Uri("http://abs.twimg.com/sticky/default_profile_images/default_profile_normal.png")),
                UserId = "859674503487606784"
            }, "DEBUG"));
        }

        #region Auth

        private async void LoginViaFacebook()
        {
            Analytics.TrackEvent("Facebook login button clicked", 
                new Dictionary<string, string>
            {
                {"Page", "Login" },
                {"Page", "Clicks" }
            });

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
            Login(await DataStore.FacebookService.Login(), "Facebook");
        }

        private async void LoginViaTwitter()
        {
            Analytics.TrackEvent("Twitter login button clicked", 
                new Dictionary<string, string>
            {
                {"Page", "Login" },
                {"Page", "Clicks" }
            });

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
            Login(await DataStore.TwitterService.Login(), "Twitter");
        }

        private void Login(SocialAccount account, string socialNet)
        {
            Analytics.TrackEvent("Trying to login in Facebook/Twitter", 
                new Dictionary<string, string>
                {
                    {"Page", "Login"},
                    {"Category", "Request"},
                    {"API", "Social network" },
                    {"Social network", socialNet},
                    {"Result", account == null ? "failure" : "success" },
                    {"Error message", "" }
                }
        );

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
