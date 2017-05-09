using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Java.Net;
using MobileCenterDemoApp.Interfaces;
using MobileCenterDemoApp.Models;
using Xamarin.Forms;
// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace MobileCenterDemoApp.Services
{
    public static class DataStore
    {        
        public static SocialAccount Account { get; set; }

        #region Lazy instances

        private static Lazy<IFacebook> _facebookServiceLazy =
            new Lazy<IFacebook>(() => DependencyService.Get<IFacebook>(DependencyFetchTarget.NewInstance));
        private static Lazy<ITwitter> _twitterServiceLazy =
            new Lazy<ITwitter>(() => DependencyService.Get<ITwitter>(DependencyFetchTarget.NewInstance));
        private static Lazy<IFitnessTracker> _fitnessServiceLazy =
            new Lazy<IFitnessTracker>(() => DependencyService.Get<IFitnessTracker>(DependencyFetchTarget.NewInstance));

        #endregion

        #region Services

        public static IFacebook FacebookService => _facebookServiceLazy.Value;
        public static ITwitter TwitterService => _twitterServiceLazy.Value;
        public static IFitnessTracker FitnessTracker => _fitnessServiceLazy.Value;

        #endregion

        #region Fitness data

        public static int TodaySteps { get; private set; }
        public static int TodayCalories { get; private set; }
        public static double TodayDistance { get; private set; }
        public static TimeSpan TodayActiveTime { get; private set; }

        public static double[] FiveDaysSteps { get; private set; }
        public static double[] FiveDaysCalories { get; private set; }
        public static double[] FiveDaysDistance { get; private set; }
        public static TimeSpan[] FiveDaysActiveTime { get; private set; }

        #endregion

        #region Retrieve fitness data

        public static async Task ReadTodayInformation()
        {
            if (FitnessTracker == null)
                throw new NullReferenceException(nameof(FitnessTracker));

            if (!FitnessTracker.IsConnected)
                throw new ConnectException("Connection closed");

            DateTime end = DateTime.UtcNow.AddHours(1);
            DateTime start = DateTime.UtcNow.AddDays(-1);
            TodaySteps = (await FitnessTracker.StepsByPeriod(start, end) ?? new[] { 0 }).Sum();
            TodayCalories = Convert.ToInt32((await FitnessTracker.CaloriesByPeriod(start, end) ?? new[] { 0D })
                .Sum());
            try{TodayDistance = (await FitnessTracker.DistanceByPeriod(start, end) ?? new[] { 0D }).Sum() / 1000D;}
            catch(Exception e) { }
            TodayActiveTime = TimeSpan.FromMinutes(
                (await FitnessTracker.ActiveTimeByPeriod(start, end)).Sum(x => x.TotalMinutes));
        }

        public static async Task ReadStatisticsInformation()
        {
            if (FitnessTracker == null)
                throw new NullReferenceException(nameof(FitnessTracker));

            if (!FitnessTracker.IsConnected)
                throw new ConnectException("Connection closed");

            Task<IEnumerable<T>> Get<T>(Func<DateTime, DateTime, Task<IEnumerable<T>>> func)
                => func(DateTime.UtcNow.AddDays(-4), DateTime.UtcNow.AddHours(1));

            FiveDaysSteps = (await Get(FitnessTracker.StepsByPeriod)).Select(x => (double)x).ToArray();
            FiveDaysCalories = (await Get(FitnessTracker.CaloriesByPeriod)).ToArray();
            FiveDaysDistance = (await Get(FitnessTracker.DistanceByPeriod)).Select(x => x / 1000).ToArray();
            FiveDaysActiveTime = (await Get(FitnessTracker.ActiveTimeByPeriod)).ToArray();

            #region If statistics less than 5 days

            IEnumerable<int> Range(int i)
                => Enumerable.Range(0, 5 - i);

            if (FiveDaysSteps.Length < 5)
                FiveDaysSteps = Range(FiveDaysSteps.Length).Select(x => 0D).Concat(FiveDaysSteps).ToArray();
            if (FiveDaysCalories.Length < 5)
                FiveDaysCalories = Range(FiveDaysCalories.Length).Select(x => 0D).Concat(FiveDaysCalories).ToArray();
            if (FiveDaysDistance.Length < 5)
                FiveDaysDistance = Range(FiveDaysDistance.Length).Select(x => 0D).Concat(FiveDaysDistance).ToArray();
            if (FiveDaysActiveTime.Length < 5)
                FiveDaysActiveTime = Range(FiveDaysActiveTime.Length).Select(x => TimeSpan.FromTicks(0))
                    .Concat(FiveDaysActiveTime).ToArray();

            #endregion

        }

        #endregion

    }
}
