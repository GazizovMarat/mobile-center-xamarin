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
        Task<IEnumerable<int>> StepsByPeriod(DateTime start, DateTime end);

        /// <summary>
        /// Distanse for period
        /// </summary>
        /// <param name="start">Start period</param>
        /// <param name="end">End period</param>
        /// <returns>Distanse group by days</returns>
        Task<IEnumerable<double>> DistanceByPeriod(DateTime start, DateTime end);

        /// <summary>
        /// Calories for period
        /// </summary>
        /// <param name="start">Start period</param>
        /// <param name="end">End period</param>
        /// <returns>Calories group by days</returns>
        Task<IEnumerable<double>> CaloriesByPeriod(DateTime start, DateTime end);

        /// <summary>
        /// Active time for period
        /// </summary>
        /// <param name="start">Start period</param>
        /// <param name="end">End period</param>
        /// <returns>Active time group by days</returns>
        Task<IEnumerable<TimeSpan>> ActiveTimeByPeriod(DateTime start, DateTime end);

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
