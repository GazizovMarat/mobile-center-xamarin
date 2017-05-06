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
using Java.Text;
using Newtonsoft.Json;

[assembly: Dependency(typeof(GoogleFitImplementation))]
namespace MobileCenterDemoApp.Droid.Dependencies
{
    public class GoogleFitImplementation : IFitnessTracker
    {
        private static GoogleApiClient Client => MainActivity.Activity.MClient;

        public bool IsConnected => Client != null && Client.IsConnected;

        public int[] StepsByPeriod(DateTime start, DateTime end)
        {
            using (DataReadRequest stepRequest = CreateRequest(DataType.TypeStepCountDelta, DataType.AggregateStepCountDelta, start, end, TimeUnit.Days))
            using (IResult stepResult = ReadData(stepRequest))
                return GetIntFromResult(stepResult);
        }

        public float[] DistanceByPeriod(DateTime start, DateTime end)
        {
            using (DataReadRequest distanceRequest = CreateRequest(DataType.TypeDistanceDelta, DataType.AggregateDistanceDelta, start, end, TimeUnit.Minutes))
            using (IResult distanceResult = ReadData(distanceRequest))
                return GetFloatFromResult(distanceResult);
        }


        public float[] CaloriesByPeriod(DateTime start, DateTime end)
        {
            using (DataReadRequest caloriesRequest = new DataReadRequest.Builder()
                .Aggregate(DataType.TypeCaloriesExpended, DataType.AggregateCaloriesExpended)
                .BucketByActivityType(1, TimeUnit.Seconds)
                .SetTimeRange(TimeUtility.DatetimeInMillis(start), TimeUtility.DatetimeInMillis(end), TimeUnit.Milliseconds)
                .Build())
            using (DataReadResult caloriesResult = (DataReadResult) ReadData(caloriesRequest))
                return GetFloatFromResult(caloriesResult);
        }

        private IResult ReadData(DataReadRequest request)
        {
            try
            {
                return (IResult)FitnessClass.HistoryApi.ReadData(Client, request).Await(1, TimeUnit.Minutes);
            }
            catch (Exception e)
            {
                Error?.Invoke(e.Message);
                return null;
            }
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

        public event Action<string> Error;

        public void Dispose() => Client?.Dispose();

        #region Implements the Google Fit data access        

        DataReadRequest CreateRequest(DataType input, DataType output, DateTime start, DateTime end, TimeUnit unit)
            => new DataReadRequest.Builder()
                .Aggregate(input, output)
                .BucketByTime(1, TimeUnit.Days)
                .SetTimeRange(TimeUtility.DatetimeInMillis(start), TimeUtility.DatetimeInMillis(end), TimeUnit.Milliseconds)                
                .Build();

        int[] GetIntFromResult(IResult result)
        {
            if (result == null)
                return null;

            IList<Bucket> buckets = ((DataReadResult)result).Buckets;
            if (!buckets.Any())
                return null;
            return buckets.Select(ExtractInt).ToArray();
        }

        float[] GetFloatFromResult(IResult result)
        {
            if (result == null)
                return null;

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