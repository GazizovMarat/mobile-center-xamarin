﻿using System;
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
        private readonly HKHealthStore _healthStore;

        public HealthKitImplementation()
        {
            _healthStore = new HKHealthStore();
        }

        private readonly HKQuantityType StepType = HKQuantityType.Create(HKQuantityTypeIdentifier.StepCount);
        private readonly HKQuantityType BasalCaloriesType = HKQuantityType.Create(HKQuantityTypeIdentifier.BasalEnergyBurned);
        private readonly HKQuantityType ActiveCaloriesType = HKQuantityType.Create(HKQuantityTypeIdentifier.ActiveEnergyBurned);
        private readonly HKQuantityType DistanceType = HKQuantityType.Create(HKQuantityTypeIdentifier.DistanceWalkingRunning);
        private readonly HKQuantityType ActiveTimeType = HKQuantityType.Create(HKQuantityTypeIdentifier.AppleExerciseTime);

        #region IFitnessTracker

        public event Action<string> OnError;

        public event Action OnConnect;

        public string ApiName => "HealthKit";

        public bool IsConnected { get; private set; }

        public async Task<IEnumerable<int>> StepsByPeriod(DateTime start, DateTime end)
        {
            return (await GetDataFromQuery(start, end, StepType, HKUnit.Count))
                .Select(x => Convert.ToInt32(x));
        }

        public Task<IEnumerable<double>> DistanceByPeriod(DateTime start, DateTime end)
        {            
            return GetDataFromQuery(start, end, DistanceType, HKUnit.CreateMeterUnit( HKMetricPrefix.None));
        }

        public async Task<IEnumerable<double>> CaloriesByPeriod(DateTime start, DateTime end)
        {
            double[] basalArray = (await GetDataFromQuery(start, end, BasalCaloriesType, HKUnit.Kilocalorie)).ToArray();
            double[] activeArray = (await GetDataFromQuery(start, end, ActiveCaloriesType, HKUnit.Kilocalorie)).ToArray();

            int resultArrayLenght = basalArray.Length < activeArray.Length ? activeArray.Length : basalArray.Length;
        
            double[] result = new double[resultArrayLenght];

            for (int i = 0; i < resultArrayLenght; i++)
                result[i] = (basalArray.Length > i ? basalArray[i] : 0) + (basalArray.Length > i ? activeArray[i] : 0);

            return result;
        }

        public async Task<IEnumerable<TimeSpan>> ActiveTimeByPeriod(DateTime start, DateTime end)
        {
            return (await GetDataFromQuery(start, end, ActiveTimeType, HKUnit.Minute)).Select(x => TimeSpan.FromMinutes(x));
        }

        public async Task Connect()
        {

            NSSet readTypes = NSSet.MakeNSObjectSet(new HKObjectType[]
            {
                StepType,
                DistanceType,
                BasalCaloriesType,
                ActiveCaloriesType,
                ActiveTimeType
            });

            try
            {
                if (HKHealthStore.IsHealthDataAvailable)
                {
                    Tuple<bool, NSError> success = await _healthStore.RequestAuthorizationToShareAsync(new NSSet(), readTypes);

                    IsConnected = success.Item1;

                    if (IsConnected)
                    {
                        OnConnect?.Invoke();
                    }
                    else
                    {
                        OnError?.Invoke(success.Item2.Description);
                    }
                }
                else
                {
                    OnError?.Invoke("Is_Health_Data_not_Available".ToUpper());
                }
            }
            catch(Exception e)
            {
                OnError?.Invoke(e.Message);
            }
        }

        public void Disconnect()
        {
           
        }

        #endregion IFitnessTracker

        #region IDisposable

        public void Dispose()
        {
            _healthStore.Dispose();
        }

        #endregion IDisposable

        private async Task<IEnumerable<double>> GetDataFromQuery(DateTime start, DateTime end, HKQuantityType quentityType, HKUnit unit)
        {
            List<double> st = new List<double>();
            bool isComplite = false;

            NSCalendar calendar = NSCalendar.CurrentCalendar;
            NSDateComponents interval = new NSDateComponents { Day = 1 };
            NSDate startDate = start.ToNsDate();
            NSDate anchorDate = end.ToNsDate();

            HKStatisticsCollectionQuery query = new HKStatisticsCollectionQuery(
                quentityType,
                null,
                HKStatisticsOptions.CumulativeSum,
                anchorDate,
                interval
            )
            {
                InitialResultsHandler = (localQuery, result, error) =>
                {
                    if (error != null)
                    {
                        OnError?.Invoke(error.Description);
                        return;
                    }

                    result.EnumerateStatistics(startDate, anchorDate, (statistics, stop) =>
                    {
                        HKQuantity quantity = statistics?.SumQuantity();

                        double value = quantity?.GetDoubleValue(unit) ?? 0;

                        st.Add(value);
                    });

                    isComplite = true;
                }
            };

            try
            {
                _healthStore.ExecuteQuery(query);
            }
            catch (Exception e)
            {
                OnError?.Invoke(e.Message);
            }

            return await Task.Run(() =>
            {
                int count = 300;
                while (isComplite && count-- >= 0)
                    Task.Delay(100);
                        
                return st;
            });

        }
    }
}
