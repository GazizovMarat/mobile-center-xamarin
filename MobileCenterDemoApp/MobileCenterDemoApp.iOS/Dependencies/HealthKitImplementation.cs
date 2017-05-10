using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MobileCenterDemoApp.iOS.Dependencies;
using MobileCenterDemoApp.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(HealthKitImplementation))]
namespace MobileCenterDemoApp.iOS.Dependencies
{
    class HealthKitImplementation : IFitnessTracker
    {
        public void Dispose()
        {
            
        }

        public string ApiName { get; } = "HealthKit";
        public bool IsConnected { get; } = true;
        public Task<IEnumerable<int>> StepsByPeriod(DateTime start, DateTime end)
        {
            return Task.FromResult(Enumerable.Range(0, 5));
        }

        public Task<IEnumerable<double>> DistanceByPeriod(DateTime start, DateTime end)
        {
            return Task.FromResult(Enumerable.Range(0, 5).Select(x => (double)x));
        }

        public Task<IEnumerable<double>> CaloriesByPeriod(DateTime start, DateTime end)
        {
            return Task.FromResult(Enumerable.Range(0, 5).Select(x => (double)x));
        }

        public Task<IEnumerable<TimeSpan>> ActiveTimeByPeriod(DateTime start, DateTime end)
        {
            return Task.FromResult(Enumerable.Range(0, 5).Select(x => TimeSpan.FromHours(x)));
        }

        public Task Connect()
        {
            OnConnect?.Invoke();
            return Task.Delay(1);
        }

        public void Disconnect()
        {
            
        }

        public event Action<string> OnError;
        public event Action OnConnect;
    }
}
