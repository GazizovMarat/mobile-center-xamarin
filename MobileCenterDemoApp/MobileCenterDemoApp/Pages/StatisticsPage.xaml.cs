using System.Collections.Generic;
using Microsoft.Azure.Mobile.Analytics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileCenterDemoApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class StatisticsPage : ContentPage
	{
		public StatisticsPage ()
		{
			InitializeComponent ();
		}

	    private bool _firstAppear = true;

	    protected override void OnAppearing()
	    {
	        if(!_firstAppear)
                return;

	        _firstAppear = false;

            Analytics.TrackEvent("View statistics button clicked", new Dictionary<string, string>
	        {
	            {"Page", "Main" },
	            {"Category", "Clicks" }
	        });
            base.OnAppearing();
	    }
	}
}
