using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using MobileCenterDemoApp.Interfaces;
using MobileCenterDemoApp.Services;
using MobileCenterDemoApp.Views;
using Xamarin.Forms;

namespace MobileCenterDemoApp
{
	public partial class App
	{
		public App ()
		{
			InitializeComponent();
            
		    MainPage = new LoginPage(); ;

            MobileCenter.Start("ca8acbe9-ff0d-4e3f-ad22-fe4a8e8f8fb8", typeof(Analytics), typeof(Crashes));		    
        }

	    protected override void OnResume()
	    {
	        if (!DataStore.FitnessTracker?.IsConnected ?? true)
	            DataStore.FitnessTracker?.Connect();

            base.OnResume();
	    }

	    protected override void OnSleep()
	    {
	        if (DataStore.FitnessTracker?.IsConnected ?? false)
	            DataStore.FitnessTracker?.Disconnect();

	        base.OnSleep();
	    }
	}

}
