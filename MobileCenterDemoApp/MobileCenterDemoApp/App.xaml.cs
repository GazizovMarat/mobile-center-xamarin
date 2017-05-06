using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using MobileCenterDemoApp.Views;
using Xamarin.Forms;

namespace MobileCenterDemoApp
{
	public partial class App
	{
		public App ()
		{
			InitializeComponent();
            MainPage = new NavigationPage(new LoginPage());

		    MobileCenter.Start("ca8acbe9-ff0d-4e3f-ad22-fe4a8e8f8fb8", typeof(Analytics), typeof(Crashes));
        }
	}

}
