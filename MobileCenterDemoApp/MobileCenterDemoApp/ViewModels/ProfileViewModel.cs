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

        private bool _infoLoad;

        public bool InfoLoad
        {
            get => _infoLoad;
            set => SetProperty(ref _infoLoad, value);
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

            if (Tracker == null)
                return;

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

            DateTime end = DateTime.UtcNow.Date.AddDays(1);
            DateTime start = DateTime.UtcNow.Date;
            StepsCount = (await Tracker.StepsByPeriod(start, end) ?? new[] {0}).Sum();
            Calories = (await Tracker.CaloriesByPeriod(start, end) ?? new[] {0D}).Sum();
            Distance = (await Tracker.DistanceByPeriod(start, end) ?? new[] {0D}).Sum() / 1000D;
            double milliseconds = (await Tracker.ActiveTimeByPeriod(start, end)).Max(x => x.TotalMilliseconds);
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(milliseconds);
            Time = $"{timeSpan.Hours}h {timeSpan.Minutes}m";

            Analytics.TrackEvent("Retrieve results from Google fit", new Dictionary<string, string>
            {
                {nameof(StepsCount), StepsCount.ToString() },
                {nameof(StepsCount), Calories.ToString(CultureInfo.InvariantCulture) },
                {nameof(StepsCount), Distance.ToString(CultureInfo.InvariantCulture) },
            });
        }
    }
}
