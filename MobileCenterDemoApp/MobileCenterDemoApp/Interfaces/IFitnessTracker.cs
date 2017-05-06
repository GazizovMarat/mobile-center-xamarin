using System;
using System.Threading.Tasks;

namespace MobileCenterDemoApp.Interfaces
{
    public interface IFitnessTracker : IDisposable
    {
        bool IsConnected { get; }

        Task<int[]> StepsByPeriod(DateTime start, DateTime end);
        Task<float[]> DistanceByPeriod(DateTime start, DateTime end);
        Task<float[]> CaloriesByPeriod(DateTime start, DateTime end);

        void Connect();
        void Disconnect();
    }
}
