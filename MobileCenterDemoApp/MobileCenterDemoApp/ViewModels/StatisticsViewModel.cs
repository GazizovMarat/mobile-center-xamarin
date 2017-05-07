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
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Xamarin.Forms;

namespace MobileCenterDemoApp.ViewModels
{
    public class StatisticsViewModel : ViewModelBase
    {
        public Command CrashCommand { get; set; }

        private PlotModel _model;
        public PlotModel Model
        {
            get => _model;
            set => SetProperty(ref _model, value);
        }

        public int StartDate { get; } = DateTime.Now.AddDays(-5).Day;
        public int EndDate { get; } = DateTime.Now.Day;

        private int _minValue;
        public int MinValue
        {
            get => _minValue;
            set => SetProperty(ref _minValue, value);
        }

        private double _maxValue;
        public double MaxValue
        {
            get => _maxValue;
            set => SetProperty(ref _maxValue, value);
        }

        private async void UpdateData(string data)
        {            
            if(!(DataStore.FitnessTracker?.IsConnected ?? false))
                return;

            Model.Axes.Clear();
            Model.Series.Clear();

            
            Task<IEnumerable<T>> Get<T>(Func<DateTime, DateTime, Task<IEnumerable<T>>> func) => func(DateTime.UtcNow.AddDays(-5), DateTime.UtcNow);
            var startDate = DateTime.UtcNow.AddDays(-5);
            var lineSeries = new LineSeries();
            switch (data)
            {
                case "Steps":
                    IEnumerable<int> steps = (await Get(DataStore.FitnessTracker.StepsByPeriod)).ToArray();
                    MaxValue = steps.Max();
                    foreach (var x in steps)
                    {
                        lineSeries.Points.Add(new DataPoint(startDate.Day, x));
                        startDate = startDate.AddDays(1);
                    }
                    break;
                case "Calories":
                    IEnumerable<double> calories = (await Get(DataStore.FitnessTracker.CaloriesByPeriod)).ToArray();
                    MaxValue = calories.Max();
                    foreach (var x in await Get(DataStore.FitnessTracker.CaloriesByPeriod))
                    {
                        lineSeries.Points.Add(new DataPoint(startDate.Day, x));
                        startDate = startDate.AddDays(1);
                    }
                    break;
                case "Distance":
                    IEnumerable<double> distance = (await Get(DataStore.FitnessTracker.DistanceByPeriod)).ToArray();
                    MaxValue = distance.Max();
                    foreach (var x in distance)
                    {
                        lineSeries.Points.Add(new DataPoint(startDate.Day, x));
                        startDate = startDate.AddDays(1);
                    }
                    break;
                case "time":
                    IEnumerable<double> times = (await Get(DataStore.FitnessTracker.ActiveTimeByPeriod)).Select(x => x.TotalMinutes).ToArray();
                    MaxValue = times.Max();
                    foreach (var x in times)
                    {
                        lineSeries.Points.Add(new DataPoint(startDate.Day, x));
                        startDate = startDate.AddDays(1);
                    }
                    break;
            }
            Model.Axes.Add(new LinearAxis
            {
                Minimum = DateTime.Now.AddDays(-5).Day,
                Maximum = DateTime.Now.Day,
                Position = AxisPosition.Bottom
            });
            Model.Axes.Add(new LinearAxis
            {
                Minimum = 0,
                Maximum = MaxValue,
                Position = AxisPosition.Left
            });

            Model.Series.Add(lineSeries);

            _currentData = data;
            RaisCanExecute();
            Analytics.TrackEvent("Google fit, retrieve success");
        }

        public Command ShowStepsCommand { get; }
        public Command ShowCaloriesCommand { get; }
        public Command ShowDistanceCommand { get; }
        public Command ShowActiveTimeCommand { get; }

        private string _currentData;

        public StatisticsViewModel()
        {
            Model = new PlotModel{Title = "TEST"};
            CrashCommand = new Command(CrashApp);
            ShowStepsCommand = new Command(() => UpdateData("Steps"), () => _currentData != "Steps");
            ShowCaloriesCommand = new Command(() => UpdateData("Calories"), () => _currentData != "Calories");
            ShowDistanceCommand = new Command(() => UpdateData("Distance"), () => _currentData != "Distance");
            ShowActiveTimeCommand = new Command(() => UpdateData("time"), () => _currentData != "time");
        }

        private void RaisCanExecute()
        {
            ShowStepsCommand.ChangeCanExecute();
            ShowCaloriesCommand.ChangeCanExecute();
            ShowDistanceCommand.ChangeCanExecute();
            ShowActiveTimeCommand.ChangeCanExecute();
        }

        private static void CrashApp()
        {
            Analytics.TrackEvent("Click crash app");
            Crashes.GenerateTestCrash();
        }
    }
}
