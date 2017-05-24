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

        #region IFitnessTracker

        public event Action<string> OnError;

        public event Action OnConnect;

        public string ApiName => "HealthKit";

        public bool IsConnected { get; private set; }

        public async Task<IEnumerable<int>> StepsByPeriod(DateTime start, DateTime end)
        {
            return (await GetDataFromQuery(start, end, HKQuantityTypeIdentifier.StepCount, HKUnit.Count))
                .Select(x => Convert.ToInt32(x));
        }

        public Task<IEnumerable<double>> DistanceByPeriod(DateTime start, DateTime end)
        {            
            return GetDataFromQuery(start, end, HKQuantityTypeIdentifier.DistanceWalkingRunning, HKUnit.CreateMeterUnit(HKMetricPrefix.None));
        }

        public async Task<IEnumerable<double>> CaloriesByPeriod(DateTime start, DateTime end)
        {
            double[] basalArray = (await GetDataFromQuery(start, end, HKQuantityTypeIdentifier.BasalEnergyBurned, HKUnit.Calorie)).ToArray();
            double[] activeArray = (await GetDataFromQuery(start, end, HKQuantityTypeIdentifier.ActiveEnergyBurned, HKUnit.Calorie)).ToArray();

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
            try
            {
                if (HKHealthStore.IsHealthDataAvailable)
                {
                    Tuple<bool, NSError> success = await _healthStore.RequestAuthorizationToShareAsync(writeTypes, readTypes);

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
              
        private async Task<IEnumerable<double>> GetDataFromQuery(DateTime start, DateTime end, HKQuantityTypeIdentifier identifier, HKUnit unit)
        {
            List<double> st = new List<double>();
            bool isComplite = false;
            string errorMessage = string.Empty;

            try
            {
                NSCalendar calendar = NSCalendar.CurrentCalendar;
                HKQuantityType sampleType = HKQuantityType.Create(identifier);
                NSPredicate predicate = HKQuery.GetPredicateForSamples(start.ToNsDate(), end.ToNsDate(), HKQueryOptions.None);
                NSDateComponents comp = new NSDateComponents();
                comp.Day = 1;

                HKSampleQuery query = new HKSampleQuery(sampleType, predicate, 0, new NSSortDescriptor[0]
                                                              , (queryResult, results, error) =>
                                                              {
                                                                  if (error != null)
                                                                  {
                                                                      isComplite = true;
                                                                      errorMessage = error.Description;
                                                                  }
                                                                  foreach (HKQuantitySample x in results)
                                                                  {
                                                                      st.Add(x.Quantity.GetDoubleValue(unit));
                                                                  }
                                                              });


                _healthStore.ExecuteQuery(query);

            }
            catch(Exception e)
            {
                OnError?.Invoke(e.Message);
            }
            return await Task.Run(() =>
            {
                int count = 30;
                while (isComplite && count-- >= 0)
                    Task.Delay(100);
                return st;
            }); ;
        }
    }
}
