namespace MobileCenterDemoApp.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Azure.Mobile.Analytics;
    using Microsoft.Azure.Mobile.Crashes;
    using Helpers;
    using Services;
    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Series;
    using Xamarin.Forms;

    public class StatisticsViewModel : ViewModelBase
    {
        #region Properties

        private PlotModel _model;

        /// <summary>
        /// Model for line chart
        /// </summary>
        public PlotModel Model
        {
            get
            {
                return _model;
            }
            set
            {
                SetProperty(ref _model, value);
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Switch to steps statistics command
        /// </summary>
        public Command ShowStepsCommand { get; }

        /// <summary>
        /// Switch to calories statistics command
        /// </summary>
        public Command ShowCaloriesCommand { get; }

        /// <summary>
        /// Switch to distance statistics command
        /// </summary>
        public Command ShowDistanceCommand { get; }

        /// <summary>
        /// Switch to activity time statistics command
        /// </summary>
        public Command ShowActiveTimeCommand { get; }

        /// <summary>
        /// Crash application command
        /// </summary>
        public Command CrashCommand { get; set; }

        #endregion

        /// <summary>
        /// Button border radius
        /// </summary>
        public double BorderRadius { get; }

        /// <summary>
        /// Current chart type
        /// </summary>
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

        /// <summary>
        /// Update view
        /// </summary>
        private void RaiseCanExecute()
        {
            ShowStepsCommand.ChangeCanExecute();
            ShowCaloriesCommand.ChangeCanExecute();
            ShowDistanceCommand.ChangeCanExecute();
            ShowActiveTimeCommand.ChangeCanExecute();
        }

        /// <summary>
        /// Crash application with Mobile Center
        /// </summary>
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

        /// <summary>
        /// Update chart model
        /// </summary>
        /// <param name="chartType">Chart information type</param>
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

        /// <summary>
        /// Chart data types
        /// </summary>
        private enum ChartType
        {
            /// <summary>
            /// Steps count 
            /// </summary>
            Steps,

            /// <summary>
            /// Calories count
            /// </summary>
            Calories,

            /// <summary>
            /// Distance
            /// </summary>
            Distance,

            /// <summary>
            /// Activity time
            /// </summary>
            ActiveTime
        }
    }
}
