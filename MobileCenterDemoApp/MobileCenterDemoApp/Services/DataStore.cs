using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public static event Action DataFill;

        public static int TodaySteps { get; private set; }
        public static int TodayCalories { get; private set; }
        public static double TodayDistance { get; private set; }
        public static TimeSpan TodayActiveTime { get; private set; }

        public static double[] FiveDaysSteps { get; private set; }
        public static double[] FiveDaysCalories { get; private set; }
        public static double[] FiveDaysDistance { get; private set; }
        public static TimeSpan[] FiveDaysActiveTime { get; private set; }

        public static bool StatisticsInit => FiveDaysSteps != null;

        #endregion

        #region Retrieve fitness data

        public static async Task ReadTodayInformation()
        {
            if (FitnessTracker == null)
                throw new NullReferenceException(nameof(FitnessTracker));

            if (!FitnessTracker.IsConnected)
                throw new Exception("Connection closed");
            
            await ReadStatisticsInformation();
            TodaySteps = Convert.ToInt32(Math.Round(FiveDaysSteps.Last()));
            TodayCalories = Convert.ToInt32(Math.Round(FiveDaysCalories.Last()));
            TodayDistance = Math.Round(FiveDaysDistance.Last(), 2);
            TodayActiveTime = FiveDaysActiveTime.Last();

            DataFill?.Invoke();
        }

        public static async Task ReadStatisticsInformation()
        {
            if (FitnessTracker == null)
                throw new NullReferenceException(nameof(FitnessTracker));

            if (!FitnessTracker.IsConnected)
                throw new Exception("Connection closed");

            if (StatisticsInit)
                return;

            Task<IEnumerable<T>> Get<T>(Func<DateTime, DateTime, Task<IEnumerable<T>>> func)
                => func(DateTime.UtcNow.AddDays(-5), DateTime.UtcNow.AddMinutes(1));

            IEnumerable<T> SkipLast<T>(IEnumerable<T> t) => t.Reverse().Skip(1).Reverse();

            FiveDaysSteps = SkipLast(await Get(FitnessTracker.StepsByPeriod)).Select(x => (double) x).ToArray();
            FiveDaysCalories = SkipLast(await Get(FitnessTracker.CaloriesByPeriod)).ToArray();
            FiveDaysDistance = SkipLast(await Get(FitnessTracker.DistanceByPeriod)).Select(x => x / 1000).ToArray();
            FiveDaysActiveTime = SkipLast(await Get(FitnessTracker.ActiveTimeByPeriod)).ToArray();

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
                FiveDaysActiveTime = Range(FiveDaysActiveTime.Length)
                    .Select(x => TimeSpan.FromTicks(0))
                    .Concat(FiveDaysActiveTime)
                    .ToArray();

            #endregion

        }

        #endregion

    }
}
