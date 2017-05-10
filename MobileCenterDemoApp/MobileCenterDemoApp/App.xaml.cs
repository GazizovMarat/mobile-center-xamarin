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
		    MobileCenter.Start("ios=3a5b14df-1962-41e0-968a-22ecd75d9927;android=ca8acbe9-ff0d-4e3f-ad22-fe4a8e8f8fb8",
		        typeof(Analytics), typeof(Crashes));

            MainPage = new LoginPage();
        }

	    public static void SwitchMainPage(Page page)
	    {
	        Current.MainPage = page;
	    }

	    protected override async void OnStart()
	    {
	        await DataStore.FitnessTracker.Connect();
            base.OnStart();
	    }
	}

}
