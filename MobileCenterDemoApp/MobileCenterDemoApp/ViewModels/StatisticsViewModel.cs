using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using MobileCenterDemoApp.Helpers;
using Xamarin.Forms;

namespace MobileCenterDemoApp.ViewModels
{
    public class StatisticsViewModel : ViewModelBase
    {
        public Command CrashCommand { get; set; }
        public Command BackToMainCommand { get; set; }

        public StatisticsViewModel()
        {
            CrashCommand = new Command(CrashApp);
            BackToMainCommand = new Command(() => Navigation.PopModalAsync());
        }

        private static void CrashApp()
        {
            Analytics.TrackEvent("Click crash app");
            Crashes.GenerateTestCrash();
        }
    }
}
