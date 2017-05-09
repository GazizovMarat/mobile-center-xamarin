using System;
using System.Collections.Generic;
using Microsoft.Azure.Mobile.Analytics;
using MobileCenterDemoApp.Helpers;
using MobileCenterDemoApp.Models;
using MobileCenterDemoApp.Services;
using MobileCenterDemoApp.Views;
using Xamarin.Forms;
// ReSharper disable ExplicitCallerInfoArgument

namespace MobileCenterDemoApp.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        #region Properties

        public bool ShowHeader
            => string.IsNullOrEmpty(ErrorMessage) && !ShowWait;
        public bool ShowError
            => !string.IsNullOrEmpty(ErrorMessage) && !ShowWait;

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

        #endregion


        public Command LoginViaFacebookCommand { get; set; }
        public Command LoginViaTwitterCommand { get; set; }
    
        public LoginViewModel()
        {
            Title = "Count my steps";

            LoginViaFacebookCommand = new Command(LoginViaFacebook);
            LoginViaTwitterCommand = new Command(LoginViaTwitter);
        }

        #region Auth

        private async void LoginViaFacebook()
        {
            Analytics.TrackEvent("Facebook login button clicked",
                new Dictionary<string, string>
                {
                    {"Page", "Login"},
                    {"Category", "Clicks"}
                });

            DataStore.FacebookService.OnError += error => AuthError("Facebook", error);

            SocialAccount account = await DataStore.FacebookService.Login();

            Login(account, "Facebook");
        }

        private async void LoginViaTwitter()
        {
            Analytics.TrackEvent("Twitter login button clicked",
                new Dictionary<string, string>
                {
                    {"Page", "Login"},
                    {"Category", "Clicks"}
                });

            DataStore.TwitterService.OnError += error => AuthError("Twitter", error);

            SocialAccount account = await DataStore.TwitterService.Login();

            Login(account, "Twitter");
        }

        private async void Login(SocialAccount account, string socialNet)
        {
            Analytics.TrackEvent("Trying to login in Facebook/Twitter",
                new Dictionary<string, string>
                {
                    {"Page", "Login"},
                    {"Category", "Request"},
                    {"API", "Social network"},
                    {"Social network", socialNet},
                    {"Result", (account != null).ToString()},
                    {"Error message", account == null ? "Cancel by user" : ""}
                });

            if (account == null)
            {
                ErrorMessage = "Login failed, please try again";
                return;
            }

            DataStore.Account = account;

            #region Init and retrive data from Google Fit/ HealthKit

            string error = "";
            bool success;

            void ErrorHandle(string errorMessage)
            {
                success = false;
                error = errorMessage;
            }

            try
            {
                DataStore.FitnessTracker.OnError += ErrorHandle;
                DataStore.FitnessTracker.Connect();
                await DataStore.ReadTodayInformation();                
                DataStore.FitnessTracker.OnError -= ErrorHandle;
                success = true;
            }
            catch (Exception e)
            {
                success = false;
                error = e.Message;
            }

            string fitnessApi =  Device.RuntimePlatform == Device.Android ? "Google fit" : "HealthKit";

            Analytics.TrackEvent("Trying to retrieve data from HealthKit/Google Fit API.",
                new Dictionary<string, string>
                {
                    {"Page", "Login"},
                    {"Category", "Request"},
                    { "API", DataStore.FitnessTracker?.ApiName ?? fitnessApi },
                    {"Result", success.ToString()},
                    {"Error_message", error}
                });

            #endregion

            App.SwitchMainPage(new MainPage());
        }

        private void AuthError(string socialNet, string message)
        {
            Analytics.TrackEvent("Trying to login in Facebook/Twitter",
                new Dictionary<string, string>
                {
                    {"Page", "Login"},
                    {"Category", "Request"},
                    {"API", "Social network"},
                    {"Social network", socialNet},
                    {"Result", false.ToString()},
                    {"Error message", message}
                }
            );
        }

        #endregion
    }
}