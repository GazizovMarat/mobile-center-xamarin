using System;
using System.Collections.ObjectModel;
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

        private readonly IFitnessTracker _ifTracker;

        public MainViewModel()
        {
            ViewStatisticsCommand = new Command(async () => await Navigation.PushModalAsync(new StatisticsPage()));

            _ifTracker = DependencyService.Get<IFitnessTracker>();
            if (_ifTracker != null)
            {
                _ifTracker.Connect();
                _ifTracker.Error += async s => await Navigation.PushModalAsync(new ErrorPage(s));
                if (_ifTracker.IsConnected)
                    Load();
            }
        }

        private void Load()
        {
            if (_ifTracker == null)
                return;

            DateTime end = DateTime.UtcNow.AddSeconds(70);
            DateTime start = end.AddDays(-1);
            StepsCount = _ifTracker.StepsByPeriod(start, end).Sum();
            Calories = _ifTracker.CaloriesByPeriod(start, end).Sum();
            Distance = _ifTracker.DistanceByPeriod(start, end).Sum();
        }
    }
}
