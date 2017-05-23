using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using MobileCenterDemoApp.Helpers;
using MobileCenterDemoApp.Services;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Xamarin.Forms;

namespace MobileCenterDemoApp.ViewModels
{
    public class StatisticsViewModel : ViewModelBase
    {

        #region Properties

        private PlotModel _model;
        public PlotModel Model
        {
            get { return _model; }
            set { SetProperty(ref _model, value); }
        }

        private int _minValue;
        public int MinValue
        {
            get { return _minValue; }
            set { SetProperty(ref _minValue, value); }
        }

        private double _maxValue;
        public double MaxValue
        {
            get { return _maxValue; }
            set { SetProperty(ref _maxValue, value); }
        }

        #endregion

        #region Commands

        public Command ShowStepsCommand { get; }

        public Command ShowCaloriesCommand { get; }

        public Command ShowDistanceCommand { get; }

        public Command ShowActiveTimeCommand { get; }

        public Command CrashCommand { get; set; }

        #endregion

        public double BorderRadius { get; }

        private ChartType _currentChartType;

        public StatisticsViewModel()
        {
            Model = new PlotModel{Title = "Mobile center"};
            CrashCommand = new Command(CrashApp);
            ShowStepsCommand = new Command(() => UpdateData(ChartType.Steps), () => _currentChartType !=  ChartType.Steps );
            ShowCaloriesCommand = new Command(() => UpdateData(ChartType.Calories), () => _currentChartType != ChartType.Calories);
            ShowDistanceCommand = new Command(() => UpdateData(ChartType.Distance), () => _currentChartType != ChartType.Distance);
            ShowActiveTimeCommand = new Command(() => UpdateData(ChartType.ActiveTime), () => _currentChartType != ChartType.ActiveTime);

            UpdateData(ChartType.Steps);

            BorderRadius = PlatformSizes.BorderRadius;
        }

        #region Private methods

        private void RaiseCanExecute()
        {
            ShowStepsCommand.ChangeCanExecute();
            ShowCaloriesCommand.ChangeCanExecute();
            ShowDistanceCommand.ChangeCanExecute();
            ShowActiveTimeCommand.ChangeCanExecute();
        }

        private static void CrashApp()
        {
            Analytics.TrackEvent("Crash application button clicked", new Dictionary<string, string>
            {
                {"Page", "Profile"},
                {"Category", "Clicks"}
            });
            Crashes.GenerateTestCrash(); // Doesn't work in Release
        }

        private bool _isUpdate = false;

        private void UpdateData(ChartType chartType)
        {
            if (_isUpdate)
                return;

            if (!DataStore.StatisticsInit)
                return;

            _isUpdate = true;

            OxyColor lineColor;
            IEnumerable<double> enumerable;

            switch (chartType)
            {
                case ChartType.Steps:
                    enumerable = DataStore.FiveDaysSteps;
                    lineColor = OxyColors.Blue;
                    break;
                case ChartType.Calories:
                    enumerable = DataStore.FiveDaysCalories;
                    lineColor = OxyColors.Orange;
                    break;
                case ChartType.Distance:
                    enumerable = DataStore.FiveDaysDistance;
                    lineColor = OxyColors.Violet;
                    break;
                case ChartType.ActiveTime:
                    enumerable = DataStore.FiveDaysActiveTime.Select(x => x.TotalMinutes);
                    lineColor = OxyColors.Green;
                    break;
                default:
                    _isUpdate = false;
                    return;
            }

            #region Make chart

            double[] dataArray = enumerable.ToArray();

            PlotModel model = new PlotModel { Title = "DAYLY STATISTICS" };
            Model.Axes.Add(new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                StringFormat = "MM/dd",
                Selectable = false,
                Minimum = DateTimeAxis.ToDouble(DateTime.UtcNow.AddDays(-4)),
                Maximum = DateTimeAxis.ToDouble(DateTime.UtcNow),
                MinorIntervalType = DateTimeIntervalType.Days,
                IntervalType = DateTimeIntervalType.Days,
                IsPanEnabled = false,
                IsZoomEnabled = false
            });
            Model.Axes.Add(new LinearAxis
            {
                Minimum = 0,
                Maximum = dataArray.Max(),
                Position = AxisPosition.Left,
                IsPanEnabled = false,
                IsZoomEnabled = false
            });

            var lineSeries = new AreaSeries
            {                
                MarkerType = MarkerType.None,
                MarkerSize = 4,
                LineStyle = LineStyle.Solid,
                Color = lineColor               
            };

            var startDate = DateTime.UtcNow.Date.AddDays(-4);
            foreach (double d in dataArray)
            {
                lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(startDate.Day), d));
                startDate = startDate.AddDays(1);
            }

            model.Series.Add(lineSeries);
            Model = model;

            #endregion

            _isUpdate = false;

            _currentChartType = chartType;
            RaiseCanExecute();
        }


        #endregion

        private enum ChartType
        {
            Steps, Calories, Distance, ActiveTime
        }
    }
}
