using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using MobileCenterDemoApp.Services;
using MobileCenterDemoApp.Pages;
using Xamarin.Forms;
using Microsoft.Azure.Mobile.Push;

namespace MobileCenterDemoApp
{
    public partial class App : Application
    {
        private const string AppKeyForAndroid = "ca8acbe9-ff0d-4e3f-ad22-fe4a8e8f8fb8";
        private const string AppKeyForIos = "3a5b14df-1962-41e0-968a-22ecd75d9927";
        private static bool _alreadyInit = false;

        public App()
        {
            InitializeComponent();

            if (_alreadyInit)
            {
                MainPage = DataStore.Account != null
                    ? (Page)new MainPage()
                    : (Page)new LoginPage();
            }
            else
            {
                MobileCenter.Start($"ios={AppKeyForIos};android={AppKeyForAndroid}", typeof(Analytics), typeof(Crashes), typeof(Push));

                Push.PushNotificationReceived += MobileCenterPush;

                MainPage = new LoginPage();

                _alreadyInit = true;

            }
        }

        private void MobileCenterPush(object sender, PushNotificationReceivedEventArgs e)
        {
            var summary = $"Push notification received:" +
                        $"\n\tNotification title: {e.Title}" +
                        $"\n\tMessage: {e.Message}";

            // If there is custom data associated with the notification,
            // print the entries
            if (e.CustomData != null)
            {
                summary += "\n\tCustom data:\n";
                foreach (var key in e.CustomData.Keys)
                {
                    summary += $"\t\t{key} : {e.CustomData[key]}\n";
                }
            }
            System.Diagnostics.Debug.WriteLine(summary);
        }

        public static void SwitchMainPage(Page page)
        {
            Current.MainPage = page;
        }

        protected override async void OnResume()
        {
            if (!DataStore.FitnessTracker.IsConnected)
                await DataStore.FitnessTracker.Connect();

            base.OnResume();
        }

        protected override async void OnStart()
        {
            if (!DataStore.FitnessTracker.IsConnected)
                await DataStore.FitnessTracker.Connect();

            base.OnStart();
        }

        protected override void OnSleep()
        {
            if (DataStore.FitnessTracker.IsConnected)
                DataStore.FitnessTracker.Disconnect();

            base.OnSleep();
        }
    }
}
