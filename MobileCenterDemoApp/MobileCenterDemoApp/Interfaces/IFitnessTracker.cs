using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileCenterDemoApp.Interfaces
{
    public interface IFitnessTracker : IDisposable
    {
        /// <summary>
        /// Fitness API name
        ///     Google Fit or HealthKit
        /// </summary>
        string ApiName { get; }

        /// <summary>
        /// Connections status
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Steps count for period
        /// </summary>
        /// <param name="start">Start period</param>
        /// <param name="end">End period</param>
        /// <returns>Steps count group by days</returns>
        void StepsByPeriod(DateTime start, DateTime end, Action<IEnumerable<int>> dataHandler);

        /// <summary>
        /// Distanse for period
        /// </summary>
        /// <param name="start">Start period</param>
        /// <param name="end">End period</param>
        /// <returns>Distanse group by days</returns>
        void DistanceByPeriod(DateTime start, DateTime end, Action<IEnumerable<double>> dataHandler);

        /// <summary>
        /// Calories for period
        /// </summary>
        /// <param name="start">Start period</param>
        /// <param name="end">End period</param>
        /// <returns>Calories group by days</returns>
        void CaloriesByPeriod(DateTime start, DateTime end, Action<IEnumerable<double>> dataHandler);

        /// <summary>
        /// Active time for period
        /// </summary>
        /// <param name="start">Start period</param>
        /// <param name="end">End period</param>
        /// <returns>Active time group by days</returns>
        void ActiveTimeByPeriod(DateTime start, DateTime end, Action<IEnumerable<TimeSpan>> dataHandler);

        /// <summary>
        /// Connect to APi
        /// </summary>
        Task Connect();

        /// <summary>
        /// Disconnect from API
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Event raise when get some error
        ///     Send: error message
        /// </summary>
        event Action<string> OnError;

        /// <summary>
        /// Event raise when connected to API
        /// </summary>
        event Action OnConnect;
    }
}
