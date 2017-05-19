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
        public event Action<string> OnError;

        public event Action OnConnect;

        private readonly HKHealthStore _healthStore;

        public HealthKitImplementation()
        {
            _healthStore = new HKHealthStore();
        }

        public void Dispose()
        {
            _healthStore.Dispose();
        }

        public string ApiName => "HealthKit";

        public bool IsConnected { get; private set; }

        public async Task<IEnumerable<int>> StepsByPeriod(DateTime start, DateTime end)
        {
            return (await GetDataFromQuery(start, end, HKQuantityTypeIdentifier.StepCount, HKUnit.Count))
                .Select(x => Convert.ToInt32(x));
        }

        public Task<IEnumerable<double>> DistanceByPeriod(DateTime start, DateTime end)
        {
            return GetDataFromQuery(start, end, HKQuantityTypeIdentifier.DistanceWalkingRunning, HKUnit.Meter);
        }

        public async Task<IEnumerable<double>> CaloriesByPeriod(DateTime start, DateTime end)
        {
            double[] basalArray = (await GetDataFromQuery(start, end, HKQuantityTypeIdentifier.BasalEnergyBurned, HKUnit.Meter)).ToArray();
            double[] activeArray = (await GetDataFromQuery(start, end, HKQuantityTypeIdentifier.ActiveEnergyBurned, HKUnit.Meter)).ToArray();

            int resultArrayLenght = basalArray.Length < activeArray.Length ? activeArray.Length : basalArray.Length;
        
            double[] result = new double[resultArrayLenght];

            for (int i = 0; i < resultArrayLenght; i++)
                result[i] = (basalArray.Length > i ? basalArray[i] : 0) + (basalArray.Length > i ? activeArray[i] : 0);

            return result;
        }

        public async Task<IEnumerable<TimeSpan>> ActiveTimeByPeriod(DateTime start, DateTime end)
        {
            return (await GetDataFromQuery(start, end, HKQuantityTypeIdentifier.AppleExerciseTime, HKUnit.Minute)).Select(x => TimeSpan.FromMinutes(x));
        }

        private async Task<IEnumerable<double>> GetDataFromQuery(DateTime start, DateTime end, HKQuantityTypeIdentifier identifier, HKUnit unit)
        {
            List<double> st = new List<double>();
            bool isConplite = false;
            string errorMessage = string.Empty;

            NSCalendar calendar = NSCalendar.CurrentCalendar;
            HKQuantityType sampleType = HKQuantityType.Create(identifier);
            NSPredicate predicate = HKQuery.GetPredicateForSamples(start.ToNsDate(), end.ToNsDate(), HKQueryOptions.None);

            HKSampleQuery query = new HKSampleQuery(sampleType, predicate, 0, new NSSortDescriptor[0],
                (resultQuery, results, error) =>
                {
                    if (error != null)
                    {
                        isConplite = true;
                        errorMessage = error.Description;
                    }

                    foreach (HKQuantitySample sample in results)
                    {
                        st.Add(sample.Quantity.GetDoubleValue(HKUnit.Count));
                    }
                });

            _healthStore.ExecuteQuery(query);

            return await Task.Run(() =>
            {
                while (isConplite)
                    Task.Delay(100);
                return st;

            }); ;
        }

        public async Task Connect()
        {
            NSSet readTypes = NSSet.MakeNSObjectSet(new HKObjectType[]
            {
                HKQuantityType.Create(HKQuantityTypeIdentifier.StepCount),
                HKQuantityType.Create(HKQuantityTypeIdentifier.DistanceWalkingRunning),
                HKQuantityType.Create(HKQuantityTypeIdentifier.BasalEnergyBurned),
                HKQuantityType.Create(HKQuantityTypeIdentifier.ActiveEnergyBurned),
                HKQuantityType.Create(HKQuantityTypeIdentifier.AppleExerciseTime)
            });
            NSSet writeTypes = NSSet.MakeNSObjectSet(new HKObjectType[]
            {
                HKQuantityType.Create(HKQuantityTypeIdentifier.StepCount),
                HKQuantityType.Create(HKQuantityTypeIdentifier.DistanceWalkingRunning),
                HKQuantityType.Create(HKQuantityTypeIdentifier.BasalEnergyBurned),
                HKQuantityType.Create(HKQuantityTypeIdentifier.ActiveEnergyBurned)
            });

            if (HKHealthStore.IsHealthDataAvailable)
            {

                try
                {
                    Tuple<bool, NSError> success = await _healthStore.RequestAuthorizationToShareAsync(writeTypes, readTypes);

                    IsConnected = success.Item1;

                    if (IsConnected)
                    {
                        await StoreSteps();
                        OnConnect?.Invoke();
                    }
                    else
                    {
                        OnError?.Invoke(success.Item2.Description);
                    }
            
                }
                catch (Exception e)
                {

                }
            }
            else
            {
                OnError?.Invoke("Is_Health_Data_not_Available".ToUpper());
            }
        }

        public void Disconnect()
        {
            
        }

        public async Task StoreSteps()
        {
            HKQuantity quan = HKQuantity.FromQuantity(HKUnit.Count, 500);
            var heartRateQuantityType = HKQuantityType.Create(HKQuantityTypeIdentifier.StepCount);
            var heartRateSample = HKQuantitySample.FromType(heartRateQuantityType, quan, NSDate.Now, NSDate.Now, new HKMetadata());
            await _healthStore.SaveObjectAsync(heartRateSample);                
        }



    }
}
