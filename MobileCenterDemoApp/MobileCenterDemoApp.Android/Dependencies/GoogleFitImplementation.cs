using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Gms.Common.Apis;
using Android.Gms.Fitness;
using Android.Gms.Fitness.Data;
using Android.Gms.Fitness.Request;
using Android.Gms.Fitness.Result;
using Google.XamarinSamples.XamFit;
using Java.IO;
using Java.Util.Concurrent;
using MobileCenterDemoApp.Droid.Dependencies;
using MobileCenterDemoApp.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(GoogleFitImplementation))]
namespace MobileCenterDemoApp.Droid.Dependencies
{
    public class GoogleFitImplementation : IFitnessTracker
    {
        private static GoogleApiClient Client => MainActivity.Activity.MClient;

        public bool IsConnected => Client != null && Client.IsConnected;

        public async Task<int[]> StepsByPeriod(DateTime start, DateTime end)
        {            
            DataReadRequest stepRequest = CreateRequest(DataType.TypeStepCountDelta, DataType.AggregateStepCountDelta, start, end);
            IResult stepResult = await FitnessClass.HistoryApi.ReadData(Client, stepRequest);
            return GetIntFromResult(stepResult);
        }

        public async Task<float[]> DistanceByPeriod(DateTime start, DateTime end)
        {
            DataReadRequest distanceRequest = CreateRequest(DataType.TypeDistanceDelta, DataType.AggregateDistanceDelta, start, end);
            IResult distanceResult = await FitnessClass.HistoryApi.ReadData(Client, distanceRequest);
            return GetFloatFromResult(distanceResult);
        }

        public async Task<int[]> CaloriesByPeriod(DateTime start, DateTime end)
        {
            DataReadRequest caloriesRequest = CreateRequest(DataType.TypeCaloriesExpended, DataType.AggregateCaloriesExpended, start, end);
            IResult caloriesResult = await FitnessClass.HistoryApi.ReadData(Client, caloriesRequest);
            return GetIntFromResult(caloriesResult);
        }

        public void Connect()
        {
            if(Client == null)
                throw new NotActiveException();

            if(!Client.IsConnecting && !Client.IsConnected)
                return;

            Client.Connect();
        }

        public void Disconnect()
        {
            if (Client == null)
                throw new NotActiveException();

            if (!Client.IsConnected)
                return;

            Client.Disconnect();
        }

        public void Dispose() => Client?.Dispose();

        #region Implements the Google Fit data access        

        DataReadRequest CreateRequest(DataType input, DataType output, DateTime start, DateTime end)
            => new DataReadRequest.Builder()
                .Aggregate(input, output)
                .BucketByTime(1, TimeUnit.Days)
                .SetTimeRange(TimeUtility.DatetimeInMillis(start), TimeUtility.DatetimeInMillis(end), TimeUnit.Milliseconds)
                .Build();

        int[] GetIntFromResult(IResult result)
        {
            IList<Bucket> buckets = ((DataReadResult)result).Buckets;
            if (!buckets.Any())
                return null;
            return buckets.Select(ExtractInt).ToArray();
        }

        float[] GetFloatFromResult(IResult result)
        {
            IList<Bucket> buckets = ((DataReadResult)result).Buckets;
            if (!buckets.Any())
                return null;
            return buckets.Select(ExtractFloat).ToArray();
        }

        private int ExtractInt(Bucket bucket)
            => (from ds in bucket.DataSets
                from p in ds.DataPoints
                from f in p.DataType.Fields
                select p.GetValue(f).AsInt()).Sum();

        private float ExtractFloat(Bucket bucket)
            => (from ds in bucket.DataSets
                from p in ds.DataPoints
                from f in p.DataType.Fields
                select p.GetValue(f).AsFloat()).Sum();


        #endregion
    }
}