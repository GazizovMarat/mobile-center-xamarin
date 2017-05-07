using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileCenterDemoApp.Interfaces
{
    public interface IFitnessTracker : IDisposable
    {
        bool IsConnected { get; }

        Task<IEnumerable<int>> StepsByPeriod(DateTime start, DateTime end);
        Task<IEnumerable<double>> DistanceByPeriod(DateTime start, DateTime end);
        Task<IEnumerable<double>> CaloriesByPeriod(DateTime start, DateTime end);
        Task<IEnumerable<TimeSpan>> ActiveTimeByPeriod(DateTime start, DateTime end);

        void Connect();
        void Disconnect();
    }
}
