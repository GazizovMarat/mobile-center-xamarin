namespace MobileCenterDemoApp
{
    using Microsoft.Azure.Mobile;
    using Microsoft.Azure.Mobile.Analytics;
    using Microsoft.Azure.Mobile.Crashes;
    using MobileCenterDemoApp.Services;
    using MobileCenterDemoApp.Pages;
    using MobileCenterDemoApp.Helpers;
    using Xamarin.Forms;
    using MobileCenterDemoApp.Models;
    using System;
    using System.Threading.Tasks;

    public partial class App : Application
    {
        private static bool _alreadyInit = false;

        public App()
        {
            InitializeComponent();

            
                MobileCenter.Start($"ios={KeysAndSecrets.MobileCenterAppKeyForIos};android={KeysAndSecrets.MobileCenterAppKeyForAndroid}", typeof(Analytics), typeof(Crashes));
                MainPage = new MainPage();
            if(Crashes.HasCrashedInLastSession)
            {
                ErrorReport report = null;
                Task.WaitAll(Task.Run(async () => report = await Crashes.GetLastSessionCrashReportAsync()));
                return;
            }
            return;

            if (_alreadyInit)
            {
                MainPage = DataStore.Account != null
                    ? (Page)new MainPage()
                    : (Page)new LoginPage();
            }
            else
            {
                
                
                MainPage = new LoginPage();

                _alreadyInit = true;
            }
        }

        /// <summary>
        /// Switch app main page
        /// </summary>
        /// <param name="page"></param>
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
