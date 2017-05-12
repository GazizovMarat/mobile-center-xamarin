using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using HealthKit;
using MobileCenterDemoApp.iOS.Dependencies;
using MobileCenterDemoApp.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(HealthKitImplementation))]
namespace MobileCenterDemoApp.iOS.Dependencies
{
    public class HealthKitImplementation : IFitnessTracker
    {

        private HKHealthStore _healthStore;

        public HealthKitImplementation()
        {
            _healthStore = new HKHealthStore();
            var types = new[]
            {
                HKQuantityType.Create(HKQuantityTypeIdentifier.StepCount),
                HKQuantityType.Create(HKQuantityTypeIdentifier.DistanceWalkingRunning),
                HKQuantityType.Create(HKQuantityTypeIdentifier.BasalEnergyBurned),
                HKQuantityType.Create(HKQuantityTypeIdentifier.ActiveEnergyBurned),
                HKQuantityType.Create(HKQuantityTypeIdentifier.AppleExerciseTime)
            };

        }

        public void Dispose()
        {
            
        }

        public string ApiName { get; } = "HealthKit";
        public bool IsConnected { get; private set; }

        public Task<IEnumerable<int>> StepsByPeriod(DateTime start, DateTime end)
        {
            return Task.FromResult((GetDataFromQuery(start, end, HKQuantityTypeIdentifier.StepCount, HKUnit.Count)).Select(x => Convert.ToInt32(x)));
        }

        public Task<IEnumerable<double>> DistanceByPeriod(DateTime start, DateTime end)
        {
            return Task.FromResult(GetDataFromQuery(start, end, HKQuantityTypeIdentifier.DistanceWalkingRunning, HKUnit.Meter));
        }

        public Task<IEnumerable<double>> CaloriesByPeriod(DateTime start, DateTime end)
        {
            double[] basalArray = GetDataFromQuery(start, end, HKQuantityTypeIdentifier.BasalEnergyBurned, HKUnit.Meter).ToArray();
            double[] activeArray = GetDataFromQuery(start, end, HKQuantityTypeIdentifier.ActiveEnergyBurned, HKUnit.Meter).ToArray();

            int resultArrayLenght = basalArray.Length < activeArray.Length ? activeArray.Length : basalArray.Length;
        
            double[] result = new double[resultArrayLenght];

            for (int i = 0; i < resultArrayLenght; i++)
                result[i] = (basalArray.Length > i ? basalArray[i] : 0) + (basalArray.Length > i ? activeArray[i] : 0);

            return Task.FromResult<IEnumerable<double>>(result);
        }

        public Task<IEnumerable<TimeSpan>> ActiveTimeByPeriod(DateTime start, DateTime end)
        {
            return Task.FromResult(GetDataFromQuery(start, end, HKQuantityTypeIdentifier.AppleExerciseTime, HKUnit.Minute).Select(x => TimeSpan.FromMinutes(x)));
        }

        private IEnumerable<double> GetDataFromQuery(DateTime start, DateTime end, HKQuantityTypeIdentifier identifier, HKUnit unit)
        {
            List<double> st = new List<double>();
            HKStatisticsCollectionQuery query = new HKStatisticsCollectionQuery(
                HKQuantityType.Create(identifier),
                null,
                HKStatisticsOptions.CumulativeSum,
                DateTime.Now.Date.AddDays(1).AddSeconds(-1).ToNsDate(),
                new NSDateComponents { Day = 1 })
            {
                InitialResultsHandler = (collectionQuery, result, error) =>
                {
                    NSDate startDate = start.ToNsDate();
                    NSDate endDate = end.ToNsDate();

                    result.EnumerateStatistics(startDate, endDate, (statistics, stop) =>
                    {
                        HKQuantity quantity = statistics.SumQuantity();
                        double value = quantity.GetDoubleValue(unit);
                        st.Add(value);
                    });
                }
            };

            _healthStore.ExecuteQuery(query);

            return st;
        }

        public async Task Connect()
        {
            var types = new[]
            {
                HKQuantityType.Create(HKQuantityTypeIdentifier.StepCount),
                HKQuantityType.Create(HKQuantityTypeIdentifier.DistanceWalkingRunning),
                HKQuantityType.Create(HKQuantityTypeIdentifier.BasalEnergyBurned),
                HKQuantityType.Create(HKQuantityTypeIdentifier.ActiveEnergyBurned),
                HKQuantityType.Create(HKQuantityTypeIdentifier.AppleExerciseTime)
            };

            NSSet<HKQuantityType> readTypes = new NSSet<HKQuantityType>((HKQuantityType[])types.Clone());
            NSSet<HKSampleType> writeTypes = new NSSet<HKSampleType>((HKQuantityType[])types.Clone());

            Tuple<bool, NSError> result = await _healthStore.RequestAuthorizationToShareAsync(writeTypes, readTypes);

            IsConnected = result.Item1;

            if (IsConnected)
                OnConnect?.Invoke();
            else
                OnError?.Invoke(result.Item2.Description);           
        }

        public void Disconnect()
        {
            
        }

        public event Action<string> OnError;
        public event Action OnConnect;
    }
}
