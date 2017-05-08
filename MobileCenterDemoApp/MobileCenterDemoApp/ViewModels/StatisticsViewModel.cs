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
            if (!(DataStore.FitnessTracker?.IsConnected ?? false))
                return;

            Task<IEnumerable<T>> Get<T>(Func<DateTime, DateTime, Task<IEnumerable<T>>> func) 
                => func(DateTime.UtcNow.AddDays(-4), DateTime.UtcNow.AddHours(1));

            OxyColor lineColor;
            IEnumerable<double> enumerable;
            switch (data)
            {
                case "Steps":
                    enumerable = (await Get(DataStore.FitnessTracker.StepsByPeriod)).Select(x => (double) x);
                    lineColor= OxyColors.Blue;
                    break;
                case "Calories":
                    enumerable = (await Get(DataStore.FitnessTracker.CaloriesByPeriod));
                    lineColor = OxyColors.Orange;
                    break;
                case "Distance":
                    enumerable = (await Get(DataStore.FitnessTracker.DistanceByPeriod)).Select(x => x/1000);
                    lineColor = OxyColors.Violet;
                    break;
                case "Active time":
                    enumerable = (await Get(DataStore.FitnessTracker.ActiveTimeByPeriod)).Select(x => x.TotalMinutes);
                    lineColor = OxyColors.Green;
                    break;
                default:
                    return;                   
            }

            double[] dataArray = enumerable.ToArray();

            PlotModel model = new PlotModel { Title = "DAYLY STATISTICS" };

            Model.Axes.Add(new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                StringFormat = "MMM/dd",
                Minimum = DateTimeAxis.ToDouble(DateTime.UtcNow.Date.AddDays(-4)),
                Maximum = DateTimeAxis.ToDouble(DateTime.UtcNow.Date),
            });
            Model.Axes.Add(new LinearAxis
            {
                Minimum = 0,
                Maximum = data.Max(),
                Position = AxisPosition.Left,
            });
            
            var lineSeries = new AreaSeries
            {
                MarkerType = MarkerType.None,
                MarkerSize = 4,
                LineStyle = LineStyle.Solid,
                Color = lineColor,
                TrackerFormatString = "{4}"
            };

            if (dataArray.Length < 5)
                dataArray = Enumerable.Range(0, 5 - dataArray.Length).Select(x => 0D).Concat(dataArray).ToArray();

            var startDate = DateTime.UtcNow.Date.AddDays(-4);
            foreach (double d in dataArray)
            {
                lineSeries.Points.Add(new DataPoint(Axis.ToDouble(startDate.Day), d));
                startDate = startDate.AddDays(1);
            }

            model.Series.Add(lineSeries);
            Model = model;

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
            ShowActiveTimeCommand = new Command(() => UpdateData("Active time"), () => _currentData != "Active time");
            UpdateData("Steps");
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
