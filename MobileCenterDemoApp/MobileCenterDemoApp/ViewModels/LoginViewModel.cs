using Microsoft.Azure.Mobile.Analytics;
using MobileCenterDemoApp.Helpers;
using MobileCenterDemoApp.Interfaces;
using MobileCenterDemoApp.Services;
using MobileCenterDemoApp.Views;
using Xamarin.Auth;
using Xamarin.Forms;

namespace MobileCenterDemoApp.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private bool _errorExists;

        public bool ErrorNotExists => !_errorExists;

        private string _errorMessage;

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                SetProperty(ref _errorMessage, value);
                SetProperty(ref _errorExists, string.IsNullOrWhiteSpace(value));
            }
        }

        public Command LoginViaFacebookCommand { get; set; }
        public Command LoginViaTwitterCommand { get; set; }
        public Command JustEnter { get; set; }

        public LoginViewModel()
        {
            Title = "Count my steps";            

            LoginViaFacebookCommand = new Command(LoginViaFacebook);
            LoginViaTwitterCommand = new Command(LoginViaTwitter);

        }

        private async void LoginViaFacebook()
        {
            Analytics.TrackEvent("Try login via facebook");

            IFacebook facebookService = DependencyService.Get<IFacebook>();
            DataStore.FacebookService = facebookService.Service;
            Login(await facebookService.Login());
        }

        private async void LoginViaTwitter()
        {
            Analytics.TrackEvent("Try login via twitter");

            ITwitter twitterService = DependencyService.Get<ITwitter>();
            DataStore.TwitterService = twitterService.Service;
            Login(await twitterService.Login());
        }
        
        private void Login(Account account)
        {
            if (account == null)
            {
                ErrorMessage = "Login failed, please try again";
                return;
            }

            DataStore.Account = account;
            Analytics.TrackEvent($"Success login ({DataStore.Account.Username})");
            Application.Current.MainPage = new NavigationPage(new MainPage());
        }
    }
}
