﻿using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Fitness;
using Android.OS;
using Android.Views;
using Microsoft.Azure.Mobile.Analytics;

namespace MobileCenterDemoApp.Droid
{
    [Activity(Label = "MobileCenterDemoApp", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private bool _authInProgress;
        public GoogleApiClient MClient { get; private set; }
        private const int OauthRequestCode = 1;

        public static MainActivity Activity { get; private set; }

        public MainActivity()
            => Activity = this;

        #region Implements activity and Google Fit permissions request

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            
            base.OnCreate(bundle);

            Xamarin.Forms.Forms.Init(this, bundle);
            OxyPlot.Xamarin.Forms.Platform.Android.PlotViewRenderer.Init();
            LoadApplication(new App());

            if (bundle != null && bundle.ContainsKey("authInProgress"))
                _authInProgress = bundle.GetBoolean("authInProgress");

            BuildApiClient();
        }

        protected override void OnStart()
        {
            base.OnStart();
            MClient.Connect();
        }

        protected override void OnStop()
        {
            if (MClient.IsConnected)
                MClient.Disconnect();
            base.OnStop();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode != OauthRequestCode)
                return;

            _authInProgress = false;
            if (resultCode == Result.Ok && !MClient.IsConnecting && !MClient.IsConnected)
                MClient.Connect();
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutBoolean("authInProgress", _authInProgress);
            base.OnSaveInstanceState(outState);
        }

        private void BuildApiClient()
        {
            MClient = new GoogleApiClient.Builder(this)
                .AddApi(FitnessClass.HISTORY_API)
                .AddScope(FitnessClass.ScopeActivityRead)
                .AddScope(FitnessClass.ScopeLocationRead)
                .AddOnConnectionFailedListener(ConnectionFailed)
                .Build();
        }

        private void ConnectionFailed(ConnectionResult result)
        {
            if (!result.HasResolution)
            {
                GoogleApiAvailability.Instance.GetErrorDialog(this, result.ErrorCode, 0).Show();
                return;
            }

            if (_authInProgress)
                return;

            _authInProgress = true;

            try
            {
                result.StartResolutionForResult(this, OauthRequestCode);
            }
            catch (IntentSender.SendIntentException)
            {
                Analytics.TrackEvent("GoogleFit SendIntentException");
            }
        }

        #endregion
    }
}