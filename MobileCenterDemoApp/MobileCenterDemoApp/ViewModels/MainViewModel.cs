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
    public class MainViewModel : ViewModelBase
    {
        public string Username => DataStore.UserName;

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

        private float _calories;
        public float Calories
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
        
        public Command ViewStatisticsCommand { get; set; }

        private static IFitnessTracker Tracker => DataStore.FitnessTracker;

        public MainViewModel()
        {
            ViewStatisticsCommand = new Command(async () => await Navigation.PushModalAsync(new StatisticsPage()));

            if (!Tracker.IsConnected)
            {
                Tracker.Connect();
            }
            if (Tracker.IsConnected)
                Task.Run(Load);
        }

        private async Task Load()
        {
            if (Tracker == null || !Tracker.IsConnected)
                return;

            DateTime end = DateTime.UtcNow;
            DateTime start = end.AddDays(-1);
            StepsCount = (await Tracker.StepsByPeriod(start, end) ?? new[] {0}).Sum();
            Calories = (await Tracker.CaloriesByPeriod(start, end) ?? new[] {0F}).Sum();
            Distance = (await Tracker.DistanceByPeriod(start, end) ?? new[] {0F}).Sum();

            Analytics.TrackEvent("Retrieve results from Google fit", new Dictionary<string, string>
            {
                {nameof(StepsCount), StepsCount.ToString() },
                {nameof(StepsCount), Calories.ToString(CultureInfo.InvariantCulture) },
                {nameof(StepsCount), Distance.ToString(CultureInfo.InvariantCulture) },
            });
        }
    }
}
