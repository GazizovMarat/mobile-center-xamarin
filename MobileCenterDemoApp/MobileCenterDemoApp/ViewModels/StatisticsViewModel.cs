using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using MobileCenterDemoApp.Helpers;
using MobileCenterDemoApp.Services;
using MobileCenterDemoApp.Views;
using Xamarin.Forms;

namespace MobileCenterDemoApp.ViewModels
{
    public class StatisticsViewModel : ViewModelBase
    {
        public Command CrashCommand { get; set; }
        public Command BackToMainCommand { get; set; }

        private string _chosen = "Steps";
        public string ChosenData
        {
            get => _chosen;
            set
            {
                SetProperty(ref _chosen, value);
                UpdateData();
            }
            
        }

        private ObservableCollection<string> _collection;

        public ObservableCollection<string> DataTypes
        {
            get => _collection;
            set => SetProperty(ref _collection, value);
        }

        private ObservableCollection<string> _data;
        public ObservableCollection<string> Collection
        {
            get => _data;
            set => SetProperty(ref _data, value);
        }

        private async void UpdateData()
        {
            if(!(DataStore.FitnessTracker?.IsConnected ?? false))
                return;


            switch (ChosenData)
            {
                case "Steps":
                    Collection.Clear();

                    foreach (var x in (await DataStore.FitnessTracker.StepsByPeriod(DateTime.UtcNow.AddDays(-5), DateTime.UtcNow))
                        .Select((x, i) => $"{DateTime.UtcNow.AddDays(-4 + i).ToShortDateString()} {x}"))
                    {
                        Collection.Add(x);
                    }
                    break;
                case "Calories":
                    Collection.Clear();

                    foreach (var x in (await DataStore.FitnessTracker.CaloriesByPeriod(DateTime.UtcNow.AddDays(-5), DateTime.UtcNow))
                        .Select((x, i) => $"{DateTime.UtcNow.AddDays(-4 + i).ToShortDateString()} {x}"))
                    {
                        Collection.Add(x);
                    }
                    break;
                case "Distance":
                    Collection.Clear();

                    foreach (var x in (await DataStore.FitnessTracker.DistanceByPeriod(DateTime.UtcNow.AddDays(-5), DateTime.UtcNow))
                        .Select((x, i) => $"{DateTime.UtcNow.AddDays(-4 + i).ToShortDateString()} {x}"))
                    {
                        Collection.Add(x);
                    }
                    break;
            }

            Analytics.TrackEvent($"Google fit, {ChosenData} retrieve success");
        }

        public StatisticsViewModel()
        {
            Collection = new ObservableCollection<string>();
            DataTypes = new ObservableCollection<string>(new[] {"Steps", "Calories", "Distance"});
            CrashCommand = new Command(CrashApp);
            BackToMainCommand = new Command(async () => await Navigation.PopModalAsync());
            UpdateData();
        }

        private static void CrashApp()
        {
            Analytics.TrackEvent("Click crash app");
            Crashes.GenerateTestCrash();
        }
    }
}
