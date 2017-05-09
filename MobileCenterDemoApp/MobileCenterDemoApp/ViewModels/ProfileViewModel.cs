using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Analytics;
using MobileCenterDemoApp.Helpers;
using MobileCenterDemoApp.Interfaces;
using MobileCenterDemoApp.Services;
using MobileCenterDemoApp.Views;
using Xamarin.Forms;

namespace MobileCenterDemoApp.ViewModels
{
    public class ProfileViewModel : ViewModelBase
    {
        public string Username => DataStore.Account?.UserName;

        private bool _infoLoaded;
        public bool InfoLoaded
        {
            get => _infoLoaded;
            set => SetProperty(ref _infoLoaded, value);
        }

        private bool _loadError;
        public bool LoadError
        {
            get => _loadError;
            set => SetProperty(ref _loadError, value);
        }

        private int _stepsCount;
        public int StepsCount
        {
            get => _stepsCount;
            set => SetProperty(ref _stepsCount, value);
        }

        private double _calories;
        public double Calories
        {
            get => _calories;
            set => SetProperty(ref _calories, value);
        }
        private double _distance;
        public double Distance
        {
            get => _distance;
            set => SetProperty(ref _distance, value);
        }

        private string _time;
        public string Time
        {
            get => _time;
            set => SetProperty(ref _time, value);
        }

        public ImageSource AccountImageSource => DataStore.Account.ImageSource;

        public Command ViewStatisticsCommand { get; set; }

        private static IFitnessTracker Tracker => DataStore.FitnessTracker;

        public ProfileViewModel()
        {
            ViewStatisticsCommand = new Command(async () => await Navigation.PushModalAsync(new StatisticsPage()));

            InfoLoaded = false;

            if(Tracker == null)
                return;

            Tracker.OnError += Error;
            Tracker.OnConnect += async () => await Load();
            if (!Tracker.IsConnected)
                Tracker.Connect();

            if (Tracker.IsConnected)
                Task.Run(Load);
            else
                Task.Run(() => Navigation.PushModalAsync(new ErrorPage("Connection to google fit failed")));
        }

        private async Task Load()
        {
            if (Tracker == null || !Tracker.IsConnected)
                return;

            DateTime end = DateTime.UtcNow.AddHours(1);
            DateTime start = DateTime.UtcNow.AddDays(-1);
            StepsCount = (await Tracker.StepsByPeriod(start, end) ?? new[] {0}).Sum();
            Calories = (await Tracker.CaloriesByPeriod(start, end) ?? new[] {0D}).Sum();
            Distance = (await Tracker.DistanceByPeriod(start, end) ?? new[] {0D}).Sum() / 1000D;
            double milliseconds = (await Tracker.ActiveTimeByPeriod(start, end)).Sum(x => x.TotalMilliseconds);
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(milliseconds);
            Time = $"{timeSpan.Hours}h {timeSpan.Minutes}m";

            InfoLoaded = true;

            Analytics.TrackEvent("Retrieve results from Google fit", new Dictionary<string, string>
            {
                {nameof(StepsCount), StepsCount.ToString() },
                {nameof(StepsCount), Calories.ToString(CultureInfo.InvariantCulture) },
                {nameof(StepsCount), Distance.ToString(CultureInfo.InvariantCulture) },
            });

        }

        private async void Error(string error)
        {
            Analytics.TrackEvent("Trying to retrieve data from HealthKit/Google Fit API.",
                new Dictionary<string, string>
                {
                    {"Page", "Main"},
                    {"Category", "Request"},
                    {
                        "API",
                        Tracker?.ApiName ?? (Device.RuntimePlatform == Device.Android ? "Google fit" : "HealthKit")
                    },
                    {"Result", false.ToString()},
                    {"Error_message", error}
                });
            await Navigation.PushModalAsync(new ErrorPage(error));
            if(!ErrorPage.ShowHomePage)
                MainPage.SwitchStatistics();
        }
    }
}
