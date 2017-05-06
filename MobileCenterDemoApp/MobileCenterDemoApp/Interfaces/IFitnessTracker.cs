using System;
using System.Threading.Tasks;

namespace MobileCenterDemoApp.Interfaces
{
    public interface IFitnessTracker : IDisposable
    {
        bool IsConnected { get; }

        int[] StepsByPeriod(DateTime start, DateTime end);
        float[] DistanceByPeriod(DateTime start, DateTime end);
        float[] CaloriesByPeriod(DateTime start, DateTime end);

        void Connect();
        void Disconnect();

        event Action<string> Error;
    }
}
