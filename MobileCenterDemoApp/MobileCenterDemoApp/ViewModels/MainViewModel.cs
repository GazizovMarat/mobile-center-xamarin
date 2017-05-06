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

        private int _calories;
        public int Calories
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

        private ObservableCollection<string> _messages;
        public ObservableCollection<string> Collection
        {
            get => _messages;
            set => SetProperty(ref _messages, value);
        }

        public Command ViewStatisticsCommand { get; set; }

        private IFitnessTracker _ifTracker;

        public MainViewModel()
        {
            try
            {
                ViewStatisticsCommand = new Command(Statistics);

                _ifTracker = DependencyService.Get<IFitnessTracker>();
                if (_ifTracker != null )
                {
                    _ifTracker.Connect();

                    if(_ifTracker.IsConnected)
                        Task.Run(Load);
                }
            }
            catch (Exception e)
            {
                Analytics.TrackEvent(e.Message);
            }
        }

        private async Task Load()
        {
            StepsCount = (await _ifTracker.StepsByPeriod(DateTime.Now.AddDays(-1), DateTime.Now)).First();
            Distance = (await _ifTracker.DistanceByPeriod(DateTime.Now.AddDays(-1), DateTime.Now)).First();
            Calories = (await _ifTracker.CaloriesByPeriod(DateTime.Now.AddDays(-1), DateTime.Now)).First();
        }

        private async void Statistics()
        {
            try
            {
                var statisticsPage = new StatisticsPage();
                await Navigation.PushModalAsync(statisticsPage);
            }
            catch (Exception)
            {
                
            }
        }
    }
}
